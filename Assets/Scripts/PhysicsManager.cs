using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    int frameNumber;

    List<NetObject> netObjects;
    public List<Networker> networkers;

    public static int rollFrames = 0;
    public static int startFrame = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        networkers = new List<Networker>();
        netObjects = new List<NetObject>();

        StartCoroutine(postFixedUpdate());
    }

    void Update()
    {

    }

    private void FixedUpdate()
    {

    }

    IEnumerator postFixedUpdate()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            frameNumber++;
        }
    }


    static PhysicsManager instance;
    public static PhysicsManager Instance => instance;
    public static int currentFrame => instance.frameNumber;
    public static void Add(Networker netObject)
    {
        if (instance == null) instance.networkers = new List<Networker>();
        instance.networkers.Add(netObject);
    }
    public static void Remove(Networker netObject)
    {
        instance.networkers.Remove(netObject);
    }
}

[System.Serializable]
public class gameState
{
    int frame;
    SnapShot[] snapShots;

    public gameState(int size)
    {
        frame = -1;
        snapShots = new SnapShot[size + 1];
    }

    public SnapShot getSnap(int index)
    {
        return snapShots[index];
    }

    public int getFrame()
    {
        return frame;
    }

    public void setSnap(int index, SnapShot snap)
    {
        snapShots[index] = snap;
    }
    public gameState setFrame(int f)
    {
        frame = f;
        return this;
    }
}