using Riptide;
using Riptide.Transports.Udp;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum MessageId : ushort
{
    UserName = 1,
    Initialize = 2,
    PingRequest = 3,
    PingResponse = 4,
    Instantiate = 5,
    SendInput = 6,
    UpdateCar = 7,
    RemoveCar = 8,
    addedCar = 9
}

public class ClientManager : MonoBehaviour
{
    public static ClientManager instance;
    private Client client;

    [SerializeField] Button Join;
    bool connected;
    float ping;

    [SerializeField] Text pingtext;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Join.onClick.AddListener(() =>
        {
            Join.interactable = false;
            Connect();
        });
    }

    public void Connect()
    {
        client = new Client();
        client.Connected += OnConnected;
        client.Disconnected += OnDisconnected;
        client.ConnectionFailed += OnConnectionFailed;

        client.Connect("127.0.0.1:7777"); // Connect to the server at localhost
        Debug.Log("Client connecting...");
    }

    private void OnApplicationQuit()
    {
        client?.Disconnect();
    }

    private void OnConnected(object sender, System.EventArgs e)
    {
        Debug.Log("Connected to server.");
        connected = true;
        client.TimeoutTime = 240000;

        SendMessageToServer(MessageId.Initialize, "Shegzy");
        SendPingRequest();
    }

    private void OnDisconnected(object sender, System.EventArgs e)
    {
        Debug.Log("Disconnected from server.");
        connected = false;
    }

    private void OnConnectionFailed(object sender, System.EventArgs e)
    {
        Debug.Log("Failed to connect to server.");
        Join.interactable = true;
    }

    public void update()
    {
        client?.Update(); // Process incoming messages
    }

    private void SendPingRequest()
    {
        Message message = Message.Create(MessageSendMode.Unreliable, MessageId.PingRequest);
        message.Add(Time.time); // Add the current time to the message
        client.Send(message);
    }

    public void SendMessageToServer(MessageId id, object data)
    {
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)id);

        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream())
        {
            formatter.Serialize(ms, data);
            message.Add(ms.ToArray());
        }
        client.Send(message);
    }

    [MessageHandler((ushort)MessageId.PingResponse)]
    private static void HandlePingResponse(Message message)
    {
        float sentTime = message.GetFloat();
        instance.ping = (Time.time - sentTime) * 1000.0f; // Ping in milliseconds

        instance.pingtext.text = instance.ping + " ms";
        instance.SendPingRequest();
    }

    //Message Reception
    [MessageHandler((ushort)MessageId.UserName)]
    private static void UpdateUsernames(Message message)
    {
        string s = (string)Utility.Deserialize(message);
        Debug.Log(s);
    }

    [MessageHandler((ushort)MessageId.Instantiate)]
    private static void Instantiate(Message message)
    {
        var id = (ushort)Utility.Deserialize(message);
        GameManager.instance.CreateCar(id);
        instance.SendMessageToServer(MessageId.addedCar, id);
    }

    [MessageHandler((ushort)MessageId.UpdateCar)]
    private static void UpdateCar(Message message)
    {
        if (GameManager.instance.ripViews == null || GameManager.instance.ripViews.Count == 0) return;
        var state = (State)Utility.Deserialize(message);
        GameManager.instance.ripViews?.ForEach(x => x.car?.UpdateCar(state));
    }

    [MessageHandler((ushort)MessageId.RemoveCar)]
    private static void RemoveCar(Message message)
    {
        var ID = (ushort)Utility.Deserialize(message);
        GameManager.instance.deleteCar(ID);
    }

    public int getID => client.Id;
    public bool Active => connected;
    public int Ping => (int)ping;
}

public class Utility
{
    public static object Deserialize(Message m)
    {
        var receivedData = m.GetBytes();
        object o = null;
        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream(receivedData))
        {
            o = formatter.Deserialize(ms);
        }
        return o;
    }
}