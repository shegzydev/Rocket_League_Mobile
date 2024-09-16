using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Networker))]
public class Car : PhysicsNetObject
{
    [SerializeField] List<WheelCollider> foreWheels;
    [SerializeField] List<Transform> foreWheel;
    [SerializeField] List<WheelCollider> rearWheels;
    [SerializeField] List<Transform> rearWheel;

    [SerializeField] float driveTorque = 525;
    float brakeTorque = 9000;
    float maxSteerAngle = 35;

    Rigidbody rb;
    [SerializeField] GameObject cam;
    GameObject camTarget;

    [SerializeField] Vector3 com;
    [SerializeField] string speed;

    AnimationCurve torqueCurve;

    float[] ratios = { 3.66f, 2.43f, 1.69f, 1.31f, 1.00f, 0.65f };
    float finaldrive = 3.31f;
    [SerializeField] int gear = 0;
    float upShiftTime = 0;
    float downShiftTime = 0;
    [SerializeField] float clutch = 1f;
    bool shifting;

    [Header("Sound")]
    float targetE_RPM;
    float torqueE_RPM;
    [SerializeField] float E_RPM, lastRPM;
    [SerializeField] RealisticEngineSound engineSound;

    ParticleSystem[] particle;

    bool grounded, wasGrounded;

    [SerializeField] LayerMask correctionMask;
    [SerializeField] Vector3 angVelocity => transform.InverseTransformDirection(attachedRigidBody.angularVelocity);

    NetInput carInput;

    public Rigidbody attachedRigidBody
    {
        get
        {
            if (rb == null) rb = GetComponent<Rigidbody>();
            return rb;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        camTarget = new GameObject("camTarget");
        camTarget.transform.position = cam.transform.position;
        camTarget.transform.parent = transform;

        cam.transform.parent = null;
    }

    protected override void Init()
    {
        carInput = new NetInput();

        particle = new ParticleSystem[2];
        particle[0] = Instantiate(Resources.Load<GameObject>("Smoke")).GetComponent<ParticleSystem>();
        particle[1] = Instantiate(Resources.Load<GameObject>("Smoke")).GetComponent<ParticleSystem>();

        clutch = 1;
        torqueCurve = new AnimationCurve();
        torqueCurve.AddKey(1000, 264);
        torqueCurve.AddKey(2000, 431);
        torqueCurve.AddKey(3000, 431);
        torqueCurve.AddKey(4000, 523);
        torqueCurve.AddKey(4500, 529);
        torqueCurve.AddKey(5000, 521);
        torqueCurve.AddKey(6000, 487);
        torqueCurve.AddKey(7000, 384);
        torqueCurve.AddKey(7500, 000);
    }

    protected override void Tick()
    {
        base.Tick();
        torqueE_RPM = Mathf.Min(foreWheels[0].rpm, foreWheels[1].rpm, rearWheels[0].rpm, rearWheels[1].rpm) * ratios[gear] * finaldrive;

        targetE_RPM = Mathf.Max(foreWheels[0].rpm, foreWheels[1].rpm, rearWheels[0].rpm, rearWheels[1].rpm) * ratios[gear] * finaldrive;
        targetE_RPM = Mathf.Clamp(Mathf.Abs(targetE_RPM), 1500, 7000);

        E_RPM = Mathf.Lerp(E_RPM, targetE_RPM, 10 * Time.deltaTime);

        var v = transform.InverseTransformDirection(rb.velocity).z;

        if (v > (5500 * 0.10472) / (finaldrive * ratios[gear]) * rearWheels[0].radius)
        {
            if (gear < ratios.Length - 1) { StartCoroutine(shift(1)); upShiftTime = 0; }
        }
        else if (v < (2500 * 0.10472) / (finaldrive * ratios[gear]) * rearWheels[0].radius)
        {
            if (gear > 0) { StartCoroutine(shift(-1)); downShiftTime = 0; }
        }

        engineSound.engineCurrentRPM = E_RPM;
        engineSound.isReversing = Mathf.Sign(targetE_RPM) < 0;
    }

    IEnumerator shift(int var)
    {
        if (shifting && var > 0) yield break;
        shifting = true;
        clutch = 0;
        yield return new WaitForSeconds(var > 0 ? 0.7f : 0.1f);
        shifting = false;
        gear += var;
        clutch = 1;
        gear = Mathf.Clamp(gear, 0, ratios.Length - 1);
    }

    protected override void FixedTick()
    {
        base.FixedTick();
        attachedRigidBody.centerOfMass = com;

        cam.transform.position = Vector3.Lerp(cam.transform.position, camTarget.transform.position, 30 * Time.fixedDeltaTime);
        speed = (int)(transform.InverseTransformDirection(rb.velocity).z * 3.59) + " km/h";

        rb.AddTorque(-transform.up * angVelocity.y * 5000);

        WheelUpdate();

        AirControl();
    }

    void WheelUpdate()
    {
        int onGround = 0;

        var _gear = clutch * finaldrive * ratios[gear];
        driveTorque = torqueCurve.Evaluate(torqueE_RPM);

        rearWheels.ForEach(x =>
        {
            x.motorTorque = carInput.throttle * _gear * driveTorque * 0.5f;
            x.brakeTorque = carInput.brake * brakeTorque;

            if (x.GetGroundHit(out WheelHit hit))
            {
                onGround++;

                var dir = attachedRigidBody.GetPointVelocity(hit.point);
                int i = rearWheels.IndexOf(x);

                particle[i].transform.position = hit.point;
                particle[i].transform.rotation = Quaternion.LookRotation(-hit.forwardDir, hit.normal);

                if (hit.forwardSlip > 0.05f)
                {
                    particle[i].Emit((int)(1000 * dir.magnitude));
                    particle[i].transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                }
            }

            x.GetWorldPose(out Vector3 pos, out Quaternion rot);

            rearWheel[rearWheels.IndexOf(x)].position = pos;
            rearWheel[rearWheels.IndexOf(x)].rotation = rot;
        });
        foreWheels.ForEach(x =>
        {
            x.motorTorque = carInput.throttle * _gear * driveTorque * 0.0f;
            x.steerAngle = carInput.steer * maxSteerAngle;
            x.brakeTorque = carInput.brake * brakeTorque;

            if (x.GetGroundHit(out WheelHit hit))
            {
                onGround++;
            }

            x.GetWorldPose(out Vector3 pos, out Quaternion rot);

            foreWheel[foreWheels.IndexOf(x)].position = pos;
            foreWheel[foreWheels.IndexOf(x)].rotation = rot;
        });

        wasGrounded = grounded;
        grounded = onGround >= 3;
    }

    void AirControl()
    {
        if (!grounded)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, 2000))
            {
                normal = hitInfo.normal;

                var offsetZ = Vector3.SignedAngle(normal, transform.up, cam.transform.forward);
                var offsetX = Vector3.SignedAngle(normal, transform.up, cam.transform.right);
                var offsetY = Vector3.SignedAngle(rb.velocity.normalized, transform.forward, Vector3.up);

                offsetZ = offsetZ * 200;
                offsetY = offsetY * 800;
                offsetX = offsetX * 200;

                var torque = cam.transform.right * -offsetX + cam.transform.forward * -offsetZ + Vector3.up * -offsetY;
                var damp = -rb.angularVelocity * 20000;

                rb.AddTorque(torque + damp);
            }
            rb.velocity = Quaternion.AngleAxis(carInput.steer * 0.3f, Vector2.up) * rb.velocity;
        }
    }

    public void Disable(bool master)
    {
        cam.SetActive(false);
        if (!rb) rb = GetComponent<Rigidbody>();
        if (master) return;
    }

    public void ApplyInput(NetInput input)
    {
        carInput = input;

    }
    Vector3 normal = Vector3.up;

    private void OnDrawGizmos()
    {
        attachedRigidBody.centerOfMass = com;
        Gizmos.DrawWireSphere(transform.TransformPoint(attachedRigidBody.centerOfMass), 0.2f);
    }

    public void Interpolate()
    {

    }

    private void LateUpdate()
    {

    }
}