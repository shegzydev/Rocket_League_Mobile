using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

[System.Serializable]
public class SnapShot
{
    public V3 Position;
    public V4 Rotation;
    public V3 Velocity;
    public V3 AngularVelocity;
}

[RequireComponent(typeof(Rigidbody))]
public class PhysicsNetObject : MonoBehaviour
{
    List<SnapShot> Snaps = new List<SnapShot>();
    Rigidbody body;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        if (ClientManager.instance.Active)
        {
            StartCoroutine(SaveSnaps());
        }
        Init();
    }
    protected virtual void Init() { }

    void Update()
    {
        Tick();
    }
    protected virtual void Tick() { }

    void FixedUpdate()
    {
        FixedTick();
    }
    protected virtual void FixedTick() { }

    IEnumerator SaveSnaps()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();

            if (!body) body = GetComponent<Rigidbody>();

            var shot = new SnapShot
            {
                AngularVelocity = V3.get(body.angularVelocity),
                Position = V3.get(body.position),
                Rotation = V4.get(body.rotation),
                Velocity = V3.get(body.velocity),
            };
            Snaps.Add(shot);
        }
    }

    public void Set(SnapShot snapShot)
    {
        body.position = snapShot.Position.get();
        body.rotation = snapShot.Rotation.get();
        body.velocity = snapShot.Velocity.get();
        //body.angularVelocity = snapShot.AngularVelocity.get();
    }

    public SnapShot GetSnap()
    {
        var shot = new SnapShot
        {
            AngularVelocity = V3.get(body.angularVelocity),
            Position = V3.get(body.position),
            Rotation = V4.get(body.rotation),
            Velocity = V3.get(body.velocity),
        };
        return shot;
    }

    public void RollBack(int frames)
    {
        if (Snaps.Count <= frames) return;

        int index = Snaps.Count - frames - 1;
        print("index:" + index);

        var shot = Snaps[Mathf.Max(0, index)];

        body.velocity = shot.Velocity.get();
        transform.position = shot.Position.get();
        body.angularVelocity = shot.AngularVelocity.get();
        transform.rotation = shot.Rotation.get();
    }

    public SnapShot BackRoll(int frames)
    {
        int index = Snaps.Count - frames - 1;
        var shot = Snaps[Mathf.Max(0, index)];
        return shot;
    }
}
