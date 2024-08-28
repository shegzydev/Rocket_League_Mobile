using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[System.Serializable]
struct s_Input
{
    public float[] input;
    public int frameNumber;
}

[System.Serializable]
public struct State
{
    public int id;
    public int frame;
    public float[] velocity;
    public float[] angVelocity;
    public float[] position;
    public float[] rotation;
}

[RequireComponent(typeof(RipView))]
public class Car : MonoBehaviour, PhysicsObject
{
    [SerializeField] List<WheelCollider> foreWheels;
    [SerializeField] List<Transform> foreWheel;
    [SerializeField] List<WheelCollider> rearWheels;
    [SerializeField] List<Transform> rearWheel;

    float throttle, brake, steer;

    float driveTorque = 850;
    float brakeTorque = 9000;
    float maxSteerAngle = 35;

    Rigidbody rb;
    RipView ripView;

    [SerializeField] Dictionary<int, Vector3> inputBuffer;

    void Start()
    {
        clientInputBuffer = new List<KeyValuePair<int, Vector3>>();
        clientInputBuffer.Add(new KeyValuePair<int, Vector3>(-1, new Vector3()));

        rb = GetComponent<Rigidbody>();
        ripView = GetComponent<RipView>();

        if (!ripView.IsMine && ClientManager.instance.Active) { Destroy(rb); }
        if (ripView.IsMine || !ClientManager.instance.Active) { PhysicsManager.AddObject(this); }

        inputBuffer = new Dictionary<int, Vector3>();
    }

    void Update()
    {

    }

    public void preSim(int frameNumber, int correction = -1)
    {
        if (correction < 0)
        {
            if (ripView.IsMine)
            {
                throttle = Input.GetAxis("Vertical");
                brake = Input.GetAxis("Jump");
                steer = Input.GetAxis("Horizontal");

                var data = new Vector3(throttle, brake, steer);
                inputBuffer.Add(frameNumber, data);

                if (ClientManager.instance.Active)
                    ripView.RPC(MessageId.SendInput, new s_Input { input = new float[] { data.x, data.y, data.z }, frameNumber = frameNumber });

                if (inputBuffer.Count > 100)
                {
                    inputBuffer.Remove(frameNumber - 100);
                }
            }
            else
            {
                if (!ClientManager.instance.Active)//server
                {
                    if (clientInputBuffer.Count > 0)
                    {
                        var data = clientInputBuffer[0];
                        
                        throttle = data.Value.x;
                        brake = data.Value.y;
                        steer = data.Value.z;
                        clientFrame = data.Key;

                        clientInputBuffer.RemoveAt(0);
                    }
                }
            }
        }
        else
        {
            int index = correctionStartFrame + correction;
            throttle = inputBuffer[index].x;
            brake = inputBuffer[index].y;
            steer = inputBuffer[index].z;

            //Debug.Log($"frame:{index};throttle:{throttle}:brake:{brake}:steer{steer}");
        }

        rearWheels.ForEach(x =>
        {
            x.motorTorque = throttle * driveTorque * 0.5f;
            x.brakeTorque = brake * brakeTorque;

            x.GetWorldPose(out Vector3 pos, out Quaternion rot);

            rearWheel[rearWheels.IndexOf(x)].position = pos;
            rearWheel[rearWheels.IndexOf(x)].rotation = rot;
        });
        foreWheels.ForEach(x =>
        {
            x.steerAngle = steer * maxSteerAngle;
            x.brakeTorque = brake * brakeTorque;

            x.GetWorldPose(out Vector3 pos, out Quaternion rot);

            foreWheel[foreWheels.IndexOf(x)].position = pos;
            foreWheel[foreWheels.IndexOf(x)].rotation = rot;
        });
    }

    bool canServerUpdate = false;
    int tickframe;
    int lasttickFrame;

    public void postSim(int frameNumber)
    {
        if (!canServerUpdate) return;
        if (!ClientManager.instance.Active)
        {
            if (clientFrame == -1) return;

            lasttickFrame = tickframe;
            tickframe = ripView.IsMine ? frameNumber : clientFrame;

            if (!ripView.IsMine && tickframe == lasttickFrame)
            {
                Debug.Log("no input yet!");
                return;
            }

            ripView.ServerRPC(MessageId.UpdateCar, new State
            {
                angVelocity = new float[] { rb.angularVelocity.x, rb.angularVelocity.y, rb.angularVelocity.z },
                velocity = new float[] { rb.velocity.x, rb.velocity.y, rb.velocity.z },
                position = new float[] { rb.position.x, rb.position.y, rb.position.z },
                rotation = new float[] { transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z },
                frame = tickframe,
                id = ripView.ID
            });
        }
    }

    public void OpenServerUpdate()
    {
        canServerUpdate = true;
    }

    private void FixedUpdate()
    {

    }

    List<KeyValuePair<int, Vector3>> clientInputBuffer;
    int clientFrame = -1;
    public void SendInput(Vector3 inputData, int frame)
    {
        //Buffer Input

        clientInputBuffer.Add(new KeyValuePair<int, Vector3>(frame, inputData));

        //throttle = inputData.x;
        //brake = inputData.y;
        //steer = inputData.z;
        //clientFrame = frame;
    }

    int correctionStartFrame;
    public GameObject attached => gameObject;

    public void UpdateCar(State state)
    {
        if (state.id != ripView.ID) return;

        transform.position = new Vector3(state.position[0], state.position[1], state.position[2]);
        transform.rotation = Quaternion.Euler(new Vector3(state.rotation[0], state.rotation[1], state.rotation[2]));

        if (ripView.IsMine)
        {
            int correctionFrames = PhysicsManager.currentFrame - state.frame;
            correctionStartFrame = state.frame;

            print("correct frame : " + state.frame + "Real frame :" + PhysicsManager.currentFrame);

            rb.velocity = new Vector3(state.velocity[0], state.velocity[1], state.velocity[2]);
            rb.angularVelocity = new Vector3(state.angVelocity[0], state.angVelocity[1], state.angVelocity[2]);

            PhysicsManager.addQueue(correctionFrames);
        }
    }
}
