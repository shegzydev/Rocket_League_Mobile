using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    [SerializeField] float touchSensitivity;
    float Yaw = 0;
    float Tilt = 0;

    [SerializeField] Car targetCar;
    [SerializeField] Transform targetCamera;


    Vector3 diff;

    private void Start()
    {
        targetCamera = transform.GetChild(0);
    }

    void Update()
    {

        foreach (Touch touch in Input.touches)
        {
            startTouchPosition = endTouchPosition;
            endTouchPosition = touch.position;
            if (touch.phase == TouchPhase.Began) startTouchPosition = endTouchPosition;
            if (touch.phase == TouchPhase.Ended) startTouchPosition = endTouchPosition;
        }

        //diff = endTouchPosition - startTouchPosition;
        diff = Touchpad.Instance.dir;

        Yaw += diff.x * touchSensitivity;
        Tilt += diff.y * touchSensitivity;

        Tilt = Mathf.Clamp(Tilt, -20, 30);
    }

    private void FixedUpdate()
    {
        if (diff.magnitude < 0.01f) transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetCar.attachedRigidBody.velocity, Vector3.up), targetCar.attachedRigidBody.velocity.magnitude * 0.5f * Time.fixedDeltaTime);

        if (Application.platform == RuntimePlatform.Android)
            targetCamera.localEulerAngles = new Vector3(0, 0, -GameManager.angle);
    }

    private void LateUpdate()
    {
        if (diff.magnitude > 0.01f)
        {
            transform.rotation = Quaternion.identity;
            transform.Rotate(0, Yaw, 0, Space.World);
            transform.Rotate(Tilt, 0, 0, Space.Self);
        }
        else
        {
            var x = transform.localEulerAngles.x % 360;
            if (x > 180) x = x - 360;

            Yaw = transform.eulerAngles.y; Tilt = x;
            Tilt = Mathf.Clamp(Tilt, -20, 30);
        }
    }
}
