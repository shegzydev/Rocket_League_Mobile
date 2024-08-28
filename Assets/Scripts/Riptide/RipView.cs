using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RipView : MonoBehaviour
{
    public int ID;
    public bool IsMine;
    public int Owner;

    public Car car;

    void Start()
    {
        car = GetComponent<Car>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RPC(MessageId id, object data)
    {
        ClientManager.instance.SendMessageToServer(id, data);
    }

    public void ServerRPC(MessageId id, object data)
    {
        ServerManager.instance.BroadcastMessage(MessageId.UpdateCar, data);
    }
}
