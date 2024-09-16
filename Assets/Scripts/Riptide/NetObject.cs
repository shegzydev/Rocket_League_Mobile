using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class NetObject : MonoBehaviour
{
    TideView view;
    public UnityAction<int> simInput;

    protected TideView tideView
    {
        get
        {
            if (view == null) view = GetComponent<TideView>();
            return view;
        }
    }

    void Start()
    {
        Init();
        print("hello");
    }

    protected virtual void Init()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Tick();
    }

    protected virtual void Tick()
    {

    }

    private void FixedUpdate()
    {
        FixedTick();
    }

    protected virtual void FixedTick()
    {

    }
}
