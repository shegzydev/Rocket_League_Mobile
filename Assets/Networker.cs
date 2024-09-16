using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Diagnostics;
using ExitGames.Client.Photon;
using System.Security.Cryptography;

public class Networker : MonoBehaviourPunCallbacks//, IPunObservable
{
    public Car car;
    RLInput m_input;

    private void Awake()
    {

    }

    protected void Start()
    {
        PhotonPeer.RegisterType(typeof(state), (byte)'M', Utility.Serialize, Utility.Deserialize);

        m_input = new RLInput();
        m_input.Enable();

        PhysicsManager.Add(this);
        car = GetComponent<Car>();

        if (!photonView.IsMine)
        {
            car.Disable(PhotonConnector.isMaster);
        }
        StartCoroutine(postFixedUpdate());
    }

    private void OnDestroy()
    {
        PhysicsManager.Remove(this);
    }

    protected void Update()
    {

    }

    NetInput input = new NetInput();
    public void FixedUpdate()
    {
        if (!car) car = GetComponent<Car>();

        if (photonView.IsMine)
        {
            input = new NetInput
            {
                throttle = m_input.Input.Drive.ReadValue<float>(),
                brake = m_input.Input.HandBrake.ReadValue<float>(),
                steer = (Application.platform == RuntimePlatform.Android) ? Mathf.Clamp(GameManager.angle / 50, -1.5f, 1.5f) : m_input.Input.Steer.ReadValue<float>(),
                inputFrame = PhysicsManager.currentFrame
            };
        }
        else//others
        {
            //car.attachedRigidBody.velocity = targetVelocity;
            //car.attachedRigidBody.angularVelocity = targetAngVelocity;
        }
        car.ApplyInput(input);
    }

    IEnumerator postFixedUpdate()
    {
        if (!photonView.IsMine) yield break;

        while (true)
        {
            yield return new WaitForFixedUpdate();
            photonView.RPC(nameof(UpdateState), RpcTarget.Others, PhysicsManager.currentFrame, car.attachedRigidBody.transform.position, car.attachedRigidBody.transform.rotation,
                car.attachedRigidBody.velocity, car.attachedRigidBody.angularVelocity,
                input.throttle, input.brake, input.steer);
        }
    }

    Vector3 targetPosition;
    Vector3 targetVelocity;
    Quaternion targetRotation;
    Vector3 targetAngVelocity;
    int lastReceived = -1;

    [PunRPC]
    public void UpdateState(int frame, Vector3 pos, Quaternion rot, Vector3 vel, Vector3 angVel, float throttle, float brake, float steer)
    {
        targetPosition = pos;
        targetRotation = rot;
        targetVelocity = vel;
        targetAngVelocity = angVel;

        car.attachedRigidBody.position = targetPosition;
        car.attachedRigidBody.rotation = targetRotation;

        if (lastReceived > -1)
        {
            int steps = frame - lastReceived;
            while (steps > 0)
            {
                car.attachedRigidBody.position += vel * Time.fixedDeltaTime;
                Quaternion deltaRotation = Quaternion.Euler(angVel * Mathf.Rad2Deg * Time.fixedDeltaTime);
                car.attachedRigidBody.MoveRotation(car.attachedRigidBody.rotation * deltaRotation);
                steps--;
            }
        }

        input.throttle = throttle;
        input.steer = steer;
        input.brake = brake;

        lastReceived = frame;
    }

    public void OnPhotonSerializeVie(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (photonView.IsMine)
            {
                stream.SendNext(car.attachedRigidBody.position);
                stream.SendNext(car.attachedRigidBody.rotation);
                stream.SendNext(car.attachedRigidBody.velocity);
                stream.SendNext(car.attachedRigidBody.angularVelocity);

                stream.SendNext(input.throttle);
                stream.SendNext(input.brake);
                stream.SendNext(input.steer);

                print("sent!");
            }
        }
        else if (stream.IsReading)
        {
            if (photonView.IsMine)
            {

            }
            else
            {
                car.attachedRigidBody.position = (Vector3)stream.ReceiveNext();
                car.attachedRigidBody.rotation = (Quaternion)stream.ReceiveNext();
                car.attachedRigidBody.velocity = (Vector3)stream.ReceiveNext();
                car.attachedRigidBody.angularVelocity = (Vector3)stream.ReceiveNext();

                input.throttle = (float)stream.ReceiveNext();
                input.brake = (float)stream.ReceiveNext();
                input.steer = (float)stream.ReceiveNext();

                print("received!");
            }
        }
    }
}

[System.Serializable]
struct state
{
    public Vector3 targetPosition;
    public Vector3 targetVelocity;
    public Quaternion targetRotation;
    public Vector3 targetAngVelocity;
}

[System.Serializable]
public class NetInput
{
    public int inputFrame = 0;
    public float throttle = 0;
    public float brake = 0;
    public float steer = 0;
}
