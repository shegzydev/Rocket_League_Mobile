using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

internal class Helper
{



}

[System.Serializable]
struct s_Input
{
    public float[] input;
    public int frameNumber;
}

[System.Serializable]
public class V3
{
    public float x, y, z;
    public Vector3 get()
    {
        return new Vector3(x, y, z);
    }
    public static V3 get(Vector3 v)
    {
        return new V3 { x = v.x, y = v.y, z = v.z };
    }
}

[System.Serializable]
public class V4 : V3
{
    public float w;

    public new Quaternion get()
    {
        return new Quaternion(x, y, z, w);
    }
    public static V4 get(Quaternion v)
    {
        return new V4 { x = v.x, y = v.y, z = v.z, w = v.w };
    }
}

[System.Serializable]
public struct State
{
    public int id;
    public int frame;
    public V3 velocity;
    public V3 angVelocity;
    public V3 position;
    public V3 rotation;
    public BallState ballState;

    public byte[] Bytes()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        using (MemoryStream ms = new MemoryStream())
        {
            formatter.Serialize(ms, this);
            return ms.ToArray();
        }
    }
    public static State Get(byte[] bytes)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream(bytes))
        {
            return (State)formatter.Deserialize(ms);
        }
    }
}


