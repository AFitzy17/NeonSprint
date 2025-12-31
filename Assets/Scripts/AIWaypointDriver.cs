using UnityEngine;

public class AIWaypointDriver : MonoBehaviour
{
    public Transform[] waypoints;

    public float acceleration = 18f;
    public float braking = 12f;
    public float maxSpeed = 16f;
    public float turnSpeed = 120f;
    public float waypointReachDistance = 6f;

    public float lateralGrip = 4f; // higher = less sideways sliding

    private int currentWaypointIndex = 0;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (rb == null || waypoints == null || waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        if (targetWaypoint == null) return;

        Vector3 toWaypoint = targetWaypoint.position - transform.position;
        toWaypoint.y = 0f;

        if (toWaypoint.magnitude <= waypointReachDistance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            return;
        }

        Vector3 targetDir = toWaypoint.normalized;

        // Steering
        float angleToTarget = Vector3.SignedAngle(transform.forward, targetDir, Vector3.up);
        float steerInput = Mathf.Clamp(angleToTarget / 45f, -1f, 1f);

        Quaternion targetRotation = Quaternion.LookRotation(targetDir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.fixedDeltaTime
        );

        // Current local velocity
        Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
        float forwardSpeed = localVelocity.z;
        float sidewaysSpeed = localVelocity.x;

        // Kill sideways sliding to simulate grip
        sidewaysSpeed = Mathf.Lerp(sidewaysSpeed, 0f, lateralGrip * Time.fixedDeltaTime);
        localVelocity.x = sidewaysSpeed;
        rb.linearVelocity = transform.TransformDirection(localVelocity);

        // Slow down for sharper turns
        float angleAbs = Mathf.Abs(angleToTarget);
        float targetSpeed = maxSpeed;

        if (angleAbs > 20f) targetSpeed = maxSpeed * 0.75f;
        if (angleAbs > 40f) targetSpeed = maxSpeed * 0.5f;
        if (angleAbs > 65f) targetSpeed = maxSpeed * 0.3f;

        // Throttle / brake logic
        if (forwardSpeed < targetSpeed)
        {
            rb.AddForce(transform.forward * acceleration, ForceMode.Acceleration);
        }
        else
        {
            rb.AddForce(-transform.forward * braking, ForceMode.Acceleration);
        }
    }
}