using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Ball : PhysicsNetObject
{
    Rigidbody rb;

    protected override void Init()
    {
        base.Init();
        rb = GetComponent<Rigidbody>();
    }

    public void UpdateBall(BallState state)
    {
        rb.position = state.position.get();
        rb.rotation = Quaternion.Euler(state.rotation.get());
        rb.velocity = state.velocity.get();
        rb.angularVelocity = state.angVelocity.get();
    }

    public BallState ballstate
    {
        get
        {
            return new BallState
            {
                position = V3.get(rb.position),
                rotation = V3.get(rb.rotation.eulerAngles),
                velocity = V3.get(rb.velocity),
                angVelocity = V3.get(rb.angularVelocity)
            };
        }
    }
}

[System.Serializable]
public class BallState
{
    public V3 position;
    public V3 rotation;
    public V3 velocity;
    public V3 angVelocity;

    public byte[] Bytes()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        using (MemoryStream ms = new MemoryStream())
        {
            formatter.Serialize(ms, this);
            return ms.ToArray();
        }
    }
    public static BallState Get(byte[] bytes)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream(bytes))
        {
            return (BallState)formatter.Deserialize(ms);
        }
    }
}
