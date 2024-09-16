using System.Net.Sockets;
using UnityEngine;

struct wheelState
{

}

public class Wheel : NetObject
{
    [SerializeField] float Spring, Damper;

    [SerializeField] float restlength;
    [SerializeField] float extension;

    [SerializeField] float radius;

    float springLength, lastLength;

    public float motorTorque, brakeTorque, steerAngle;

    float angularSpeed;
    float angularAcceleration;

    public GameObject attached => gameObject;
    public Rigidbody rb;

    [SerializeField] LayerMask mask;

    public Vector3 rayPoint;
    Vector3 offset;
    float comHeight;
    public void sim()
    {
        float max = restlength + extension;
        float min = restlength - extension;

        transform.localRotation = Quaternion.Euler(0, steerAngle, 0);
        Vector3 rayPoint = rb.position + rb.transform.right * offset.x + rb.transform.forward * offset.z + rb.transform.up * offset.y;

        if (Physics.Raycast(rayPoint, -transform.up, out RaycastHit info, max, mask))
        {
            lastLength = springLength;
            springLength = info.distance - radius;
            springLength = Mathf.Clamp(springLength, min, max);

            var disp = restlength - springLength;
            var sForce = Spring * disp;

            var speed = (lastLength - springLength) / Time.fixedDeltaTime;
            var dForce = Damper * speed;

            var force = dForce + sForce;

            var pointSpeed = transform.InverseTransformDirection(rb.GetPointVelocity(info.point));
            var carSpeed = transform.InverseTransformDirection(rb.velocity);

            Vector3 fwForceDirection = Vector3.Cross(transform.right, info.normal);
            Vector3 upForceDirection = info.normal;
            Vector3 sdForceDirection = Vector3.Cross(transform.forward, info.normal);

            angularSpeed = pointSpeed.z / radius;

            angularSpeed -= Mathf.Clamp(brakeTorque * 0.01f, -sForce, sForce) * angularSpeed;
            var vertForce = sForce;

            var fwForce = -pointSpeed.z * (Mathf.Abs(pointSpeed.z) > 0.1f ? 0 : vertForce);
            fwForce += (Mathf.Abs(pointSpeed.z) > 0.1f) ? 0 : (Mathf.Sign(fwForce) * 0.15f);

            fwForce -= Mathf.Clamp(pointSpeed.z * brakeTorque, -sForce, sForce) * 0.9f;

            rb.AddForceAtPosition(fwForceDirection * fwForce, info.point);//longitudinal

            var sideForce = pointSpeed.x * sForce;
            rb.AddForceAtPosition(sdForceDirection * sideForce, info.point);//lateral

            rb.AddForceAtPosition(upForceDirection * force, info.point);//suspension

            var driveFore = motorTorque / radius;
            rb.AddForceAtPosition(fwForceDirection * driveFore, info.point);//drive
        }
    }

    float LongitudinalFriction(float wheelVelocityForward, float vehicleForwardSpeed, float normalForce)
    {
        float kFriction = 1.0f;

        float speedDifference = wheelVelocityForward - vehicleForwardSpeed;

        float frictionForce = kFriction * normalForce * speedDifference;

        frictionForce += frictionForce + (Mathf.Sign(frictionForce) * 0.15f);

        return Mathf.Clamp(frictionForce, -normalForce, normalForce);
    }

    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        offset = rb.transform.InverseTransformPoint(transform.position);
        comHeight = rb.transform.TransformPoint(rb.centerOfMass).y - transform.position.y;
    }


    void Update()
    {

    }

    private void OnDestroy()
    {

    }

    public void GetWorldPose(out Vector3 position, out Quaternion rotation)
    {
        position = transform.position - transform.up * springLength;
        rotation = transform.rotation;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + transform.forward * 0.05f, transform.position + transform.forward * 0.05f - transform.up * restlength);

        Gizmos.color = Color.yellow;
        Vector3 start = transform.position - transform.up * restlength;
        Gizmos.DrawLine(start + transform.up * extension, start - transform.up * extension);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(start, radius);
    }
}
