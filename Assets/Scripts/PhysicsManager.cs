using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PhysicsObject
{
    public GameObject attached { get; }
    public void preSim(int framenumber, int correctionFrames = -1);
    public void postSim(int framenumber);
}

public class PhysicsManager : MonoBehaviour
{
    int frameNumber;
    double accum = 0;
    float timeStep = 1 / 60f;

    public List<PhysicsObject> physicsObjects;

    int correctionFrames;

    int step;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Physics.simulationMode = SimulationMode.Script;
        physicsObjects = new List<PhysicsObject>();
    }

    void Update()
    {
        tryUpdatePhysics();
    }

    void tryUpdatePhysics()
    {
        if (Physics.simulationMode != SimulationMode.Script) return;

        
        accum += Time.deltaTime;
        while (accum >= timeStep)
        {
            //Left Over

            for (int i = 0; i < correctionFrames; i++)
            {
                foreach (var p in physicsObjects)
                {
                    if (p != null || p.attached != null)
                    {
                        p.preSim(frameNumber, i);
                    }
                }
                Physics.Simulate(timeStep);
            }
            correctionFrames = 0;

            //Real Physics

            step++;
            if (step % 6 == 0)
            {
                ServerManager.instance?.update();
            }
            ClientManager.instance?.update();


            foreach (var p in physicsObjects)
            {
                if (p != null || p.attached != null)
                {
                    p.preSim(frameNumber);
                }
            }

            Physics.Simulate(timeStep);

            foreach (var p in physicsObjects)
            {
                if (p != null || p.attached != null)
                {
                    p.postSim(frameNumber);
                }
            }

            accum -= timeStep;
            frameNumber++;
        }
    }

    public static void addQueue(int lag)
    {
        instance.correctionFrames = lag;
    }

    static PhysicsManager instance;
    public static void AddObject(PhysicsObject newObject)
    {
        if (instance == null) instance = new GameObject("Physics_Dude").AddComponent<PhysicsManager>();

        instance.physicsObjects.Add(newObject);
    }

    public static void Remove(PhysicsObject newObject)
    {
        instance.physicsObjects.Remove(newObject);
    }

    public static int currentFrame => instance.frameNumber;
}
