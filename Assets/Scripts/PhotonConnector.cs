using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PhotonConnector : MonoBehaviourPunCallbacks
{
    static PhotonConnector instance;
    [SerializeField] Text m_text;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != this)
            {
                DestroyImmediate(gameObject);
            }
        }
    }

    void Start()
    {
        StartCoroutine(TriggerConnection());
    }

    IEnumerator TriggerConnection()
    {
        while (true)
        {
            yield return new WaitWhile(() => PhotonNetwork.IsConnected);
            Debug.Log("trigger");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log($"Connected to server {PhotonNetwork.IsConnected}");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined lobby");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        //Debug.Log(cause.ToString());
        //PhotonNetwork.Reconnect();
    }

    void Update()
    {
        m_text.text = (inLobby || inRoom ? $"{PhotonNetwork.GetPing()} ms" : "Connecting to server") + $"__________{Mathf.RoundToInt(1.0f / Time.smoothDeltaTime)} FPS";
    }

    public static void Connect()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<GameObject>("Connect")).GetComponent<PhotonConnector>();
        }
        instance.TriggerConnection();
    }

    bool insideLobby() => PhotonNetwork.IsConnected && PhotonNetwork.InLobby;

    public static bool inRoom => PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom;
    public static bool isMaster => PhotonNetwork.IsConnectedAndReady && PhotonNetwork.IsMasterClient;
    public static bool inLobby => PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InLobby;
}
