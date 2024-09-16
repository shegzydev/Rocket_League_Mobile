using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;

public class TideView : MonoBehaviour
{
    public ushort ID;
    public bool IsMine;
    public int Owner;

    IEnumerable<Type> derivedTypes;

    [System.Serializable]
    public struct rpc
    {
        public string name;
        public MethodInfo method;
        public ParameterInfo[] parameters;
    }

    [SerializeField] List<rpc> RPCs;

    NetObject netObject;
    public void Start()
    {
        netObject = GetComponent<NetObject>();
        RPCs = new List<rpc>();

        Assembly assembly = Assembly.GetAssembly(typeof(NetObject));
        derivedTypes = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(NetObject)));

        foreach (Type derivedType in derivedTypes)
        {
            MethodInfo[] methods = derivedType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var method in methods)
            {
                if (method.GetCustomAttribute(typeof(TideRPC)) != null)
                {
                    RPCs.Add(new rpc { name = method.Name, method = method, parameters = method.GetParameters() });
                }
            }
        }

        RPCs.ForEach(x => print(x.name));
    }

    void Update()
    {

    }

    public void RPC(string methodName, params object[] parameters)
    {
        var data = new rpcData()
        {
            rpcID = ID,
            methodName = methodName,
            toSend = parameters
        };

        if (ClientManager.instance.Active)
        {
            ClientManager.instance.SendMessageToServer(MessageId.rpc_C, data);
        }
        else
        {
            ServerManager.instance.BroadcastMessage(MessageId.rpc_S, data);
        }
    }

    public void CallRPC(string methodName, object[] parameters)
    {
        foreach (var entry in RPCs)
        {
            if (entry.name == methodName)
            {
                entry.method.Invoke(netObject, parameters);
            }
        }
    }

    public bool canServerUpdate = false;
    public void OpenServerUpdate()
    {
        canServerUpdate = true;
    }

    public TideView tideView => this;
}

[System.Serializable]
public class rpcData
{
    public int rpcID;
    public string methodName;
    public object[] toSend;
}

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class TideRPC : Attribute
{
    // You can add properties to the attribute if needed
}