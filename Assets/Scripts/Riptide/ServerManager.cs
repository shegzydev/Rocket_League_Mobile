using Riptide;
using Riptide.Transports.Udp;
using Riptide.Utils;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;


public class ServerManager : MonoBehaviour
{
    public static ServerManager instance;
    private Server server;

    int clientCount;

    [SerializeField] Button Host;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        Host.onClick.AddListener(() =>
        {
            Host.interactable = false;
            Connect();
        });
    }

    void Connect()
    {
        server = new Server();

        server.ClientConnected += Server_ClientConnected;
        server.ClientDisconnected += Server_ClientDisconnected;
        server.ConnectionFailed += Server_ConnectionFailed;

        server.Start(7777, 10); // Port 7777, maximum 10 clients
    }

    private void Server_ConnectionFailed(object sender, ServerConnectionFailedEventArgs e)
    {
        Host.interactable = true;
    }

    private void Server_ClientDisconnected(object sender, ServerDisconnectedEventArgs e)
    {
        Debug.Log($"Client {e.Client.Id} disconnected.");

        BroadcastMessage(MessageId.RemoveCar, e.Client.Id);
        GameManager.instance.deleteCar(e.Client.Id);
    }

    private void Server_ClientConnected(object sender, ServerConnectedEventArgs e)
    {
        Debug.Log($"Client {e.Client.Id} connected.");

        foreach (var dude in GameManager.instance.ripViews)
        {
            BroadcastTo(e.Client.Id, MessageId.Instantiate, (ushort)dude.ID);
        }
        BroadcastMessage(MessageId.Instantiate, e.Client.Id);
        GameManager.instance.CreateCar(e.Client.Id);
    }

    private void OnApplicationQuit()
    {
        server?.Stop();
    }

    public void update()
    {
        server?.Update(); // Process incoming messages
    }

    public void BroadcastMessage(MessageId id, object data)
    {
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)id);

        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream())
        {
            formatter.Serialize(ms, data);
            message.Add(ms.ToArray());
        }

        server.SendToAll(message);
    }

    public void BroadcastTo(ushort clientID, MessageId id, object data)
    {
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)id);

        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream())
        {
            formatter.Serialize(ms, data);
            message.Add(ms.ToArray());
        }
        server.Send(message, (ushort)clientID);
    }

    [MessageHandler((ushort)MessageId.UserName)]
    private static void UpdateUsernames(ushort fromClientId, Message message)
    {
        string name = (string)Utility.Deserialize(message);
        Debug.Log($"Received message from client {fromClientId}: {name}");
    }

    [MessageHandler((ushort)MessageId.Initialize)]
    private static void Intialize(ushort fromClientId, Message message)
    {
        string name = (string)Utility.Deserialize(message);
        Debug.Log($"Received message from client {fromClientId}: {name}");
    }

    [MessageHandler((ushort)MessageId.PingRequest)]
    private static void HandlePingRequest(ushort fromClientId, Message message)
    {
        float sentTime = message.GetFloat();

        // Create a ping response message
        Message response = Message.Create(MessageSendMode.Unreliable, MessageId.PingResponse);
        response.Add(sentTime); // Send the original time back to the client

        instance.server.Send(response, fromClientId);
    }

    [MessageHandler((ushort)MessageId.addedCar)]
    private static void addedCar(ushort fromClientId, Message message)
    {
        var id = (ushort)Utility.Deserialize(message);
        foreach (var entry in GameManager.instance.ripViews)
        {
            if(entry.ID == id)
            {
                entry.car.OpenServerUpdate();
                break;
            }
        }
    }

    [MessageHandler((ushort)MessageId.SendInput)]
    private static void HandleInput(ushort fromClientId, Message message)
    {
        var input = (s_Input)Utility.Deserialize(message);
        Vector3 v = new Vector3(input.input[0], input.input[1], input.input[2]);

        foreach (var item in GameManager.instance.ripViews)
        {
            if (item.ID != fromClientId) continue;
            item.car.SendInput(v, input.frameNumber);
        }

    }
    //public bool connected => server.
}
