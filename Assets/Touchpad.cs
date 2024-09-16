using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Touchpad : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
{
    Vector2 start;
    Vector2 end;

    bool touching = false;

    public Vector2 dir => (end - start);

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (touching) return;

        start = eventData.position;
        end = eventData.position;

        touching = true;
    }

    void IPointerMoveHandler.OnPointerMove(PointerEventData eventData)
    {
        if (!touching) return;
        if (Vector2.Distance(end, eventData.position) >= Screen.height * 0.33f) return;
        start = end;
        end = eventData.position;
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        start = eventData.position;
        end = eventData.position;
        touching = false;
    }

    void Start()
    {

    }

    void Update()
    {

    }

    static Touchpad touchpad;
    public static Touchpad Instance
    {
        get
        {
            if (!touchpad) touchpad = FindObjectOfType<Touchpad>();
            return touchpad;
        }
    }
}
