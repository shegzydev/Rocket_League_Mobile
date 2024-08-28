using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MP_manager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] Button Server;
    [SerializeField] Button Join;

    [SerializeField] Button roomButton;
    [SerializeField] Transform holder;
    List<Button> joinList;

    [SerializeField] GameObject car;
    void Start()
    {
        joinList = new List<Button>();

        Server.onClick.AddListener(() =>
        {
            RoomOptions roomOptions = new RoomOptions();

            roomOptions.CustomRoomPropertiesForLobby = new string[] { "Game" };
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
            {
                {"Game","Soccer"}
            };

            PhotonNetwork.CreateRoom("Game", roomOptions);
        });

        Join.onClick.AddListener(() =>
        {
            PhotonNetwork.JoinRoom(currentRoom);
        });
    }

    void Update()
    {
        Server.interactable = PhotonConnector.inLobby;
        Join.interactable = PhotonConnector.inLobby && currentRoom != "nil";
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }

    string currentRoom = "nil";
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        joinList.ForEach(x => Destroy(x.gameObject));
        joinList.Clear();

        roomList.ForEach(x =>
        {
            var room = Instantiate(roomButton, holder);

            room.onClick.AddListener(() =>
            {
                joinList.ForEach((x) => x.interactable = true);
                currentRoom = x.Name;
                room.interactable = false;
            });
            room.GetComponentInChildren<Text>().text = x.Name;

            joinList.Add(room);
        });
    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"Room {PhotonNetwork.CurrentRoom.Name} created!");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined room {PhotonNetwork.CurrentRoom.Name}!");
        if (!PhotonConnector.isMaster)
            CreateCar();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} joined room!");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} left room!");
    }

    public void CreateCar()
    {
        GameObject myCar = PhotonNetwork.Instantiate(car.name, car.transform.position, car.transform.rotation);
    }
}
