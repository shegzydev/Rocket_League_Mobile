using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class NetworkManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SendUpdate()
    {
        foreach (var entry in PhysicsManager.Instance.networkers)
        {
            RaiseEventOptions options = new RaiseEventOptions()
            {
                TargetActors = new int[] { }
            };
            PhotonNetwork.RaiseEvent(101, entry, options, SendOptions.SendReliable);
        }
    }
}
