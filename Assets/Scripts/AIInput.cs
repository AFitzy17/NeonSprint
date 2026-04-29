using UnityEngine;

[RequireComponent(typeof(CarController))]
public class AIInput : MonoBehaviour
{
    public Transform[] waypoints;
    public float waypointReachDistance = 3f;
    public float lookAheadDistance = 9f;
    public float passedWaypointDotThreshold = -0.2f;
    public bool canDrive = true;
    public float steerStrength = 1.35f;
    public float brakeLookAheadMultiplier = 1.9f;

    public float gentleTurnAngle = 5f;
    public float mediumTurnAngle = 12f;
    public float sharpTurnAngle = 20f;
    public float verySharpTurnAngle = 30f;

    public float stuckSpeedThreshold = 2f;
    public float stuckTimeThreshold = 3.5f;
    public float offTrackDistanceThreshold = 20f;
    public float offTrackTimeThreshold = 2.5f;
    public float recoveryHeightOffset = 1f;
    public float recoveryCooldown = 3f;

    CarController car;
    Rigidbody rb;
    int currentWaypointIndex;

    float stuckTimer;
    float offTrackTimer;
    float recoveryCooldownTimer;

    int lastSafeWaypointIndex;
    Vector3 lastSafePosition;
    Quaternion lastSafeRotation;

    void Awake()
    {
        car = GetComponent<CarController>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        SaveRecoveryPoint();
    }

    void Update()
    {
        if (!canDrive)
        {
            car.SetInputs(0f, 0f, 1f, false);
            return;
        }

        if (recoveryCooldownTimer > 0f)
            recoveryCooldownTimer -= Time.deltaTime;

        if (waypoints == null || waypoints.Length == 0)
        {
            car.SetInputs(0f, 0f, 1f, false);
            return;
        }

        Transform currentTarget = waypoints[currentWaypointIndex];
        if (currentTarget == null)
        {
            car.SetInputs(0f, 0f, 1f, false);
            return;
        }

        Vector3 toCurrent = currentTarget.position - transform.position;
        toCurrent.y = 0f;

        if (toCurrent.magnitude <= waypointReachDistance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            SaveRecoveryPoint();
            currentTarget = waypoints[currentWaypointIndex];
            toCurrent = currentTarget.position - transform.position;
            toCurrent.y = 0f;
        }

        Vector3 currentDir = toCurrent.normalized;
        float forwardDot = Vector3.Dot(transform.forward, currentDir);

        if (forwardDot < passedWaypointDotThreshold)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            SaveRecoveryPoint();
            currentTarget = waypoints[currentWaypointIndex];
            toCurrent = currentTarget.position - transform.position;
            toCurrent.y = 0f;
            currentDir = toCurrent.normalized;
        }

        CheckRecoveryNeeds();

        int lookAheadIndex = currentWaypointIndex;
        float travelled = 0f;
        Vector3 lookAheadPoint = waypoints[lookAheadIndex].position;

        while (travelled < lookAheadDistance)
        {
            int nextIndex = (lookAheadIndex + 1) % waypoints.Length;
            Vector3 a = waypoints[lookAheadIndex].position;
            Vector3 b = waypoints[nextIndex].position;
            travelled += Vector3.Distance(a, b);
            lookAheadIndex = nextIndex;
            lookAheadPoint = b;
        }

        int brakeLookAheadIndex = currentWaypointIndex;
        float brakeTravelled = 0f;
        Vector3 brakeLookAheadPoint = waypoints[brakeLookAheadIndex].position;
        float brakeLookAheadDistance = lookAheadDistance * brakeLookAheadMultiplier;

        while (brakeTravelled < brakeLookAheadDistance)
        {
            int nextIndex = (brakeLookAheadIndex + 1) % waypoints.Length;
            Vector3 a = waypoints[brakeLookAheadIndex].position;
            Vector3 b = waypoints[nextIndex].position;
            brakeTravelled += Vector3.Distance(a, b);
            brakeLookAheadIndex = nextIndex;
            brakeLookAheadPoint = b;
        }

        Vector3 toLookAhead = lookAheadPoint - transform.position;
        toLookAhead.y = 0f;

        Vector3 toBrakeLookAhead = brakeLookAheadPoint - transform.position;
        toBrakeLookAhead.y = 0f;

        Vector3 targetDir = toLookAhead.sqrMagnitude > 0.01f ? toLookAhead.normalized : currentDir;
        Vector3 brakeDir = toBrakeLookAhead.sqrMagnitude > 0.01f ? toBrakeLookAhead.normalized : currentDir;

        float angleToTarget = Vector3.SignedAngle(transform.forward, targetDir, Vector3.up);
        float angleAbs = Mathf.Abs(angleToTarget);

        float brakeAngleToTarget = Vector3.SignedAngle(transform.forward, brakeDir, Vector3.up);
        float brakeAngleAbs = Mathf.Abs(brakeAngleToTarget);

        float steer = Mathf.Clamp(angleToTarget / 12f, -1f, 1f) * steerStrength;

        float throttle = 1f;
        float brake = 0f;
        bool handbrake = false;

        if (brakeAngleAbs > gentleTurnAngle)
            throttle = 0.85f;

        if (brakeAngleAbs > mediumTurnAngle)
        {
            throttle = 0.62f;
            brake = 0.18f;
        }

        if (brakeAngleAbs > sharpTurnAngle)
        {
            throttle = 0.38f;
            brake = 0.38f;
        }

        if (brakeAngleAbs > verySharpTurnAngle)
        {
            throttle = 0.16f;
            brake = 0.6f;
        }

        if (car.SpeedKph > 70f && brakeAngleAbs > mediumTurnAngle)
        {
            throttle = Mathf.Min(throttle, 0.48f);
            brake = Mathf.Max(brake, 0.3f);
        }

        if (car.SpeedKph > 95f && brakeAngleAbs > sharpTurnAngle)
        {
            throttle = Mathf.Min(throttle, 0.22f);
            brake = Mathf.Max(brake, 0.55f);
        }

        if (car.SpeedKph > 110f && brakeAngleAbs > verySharpTurnAngle)
        {
            throttle = Mathf.Min(throttle, 0.08f);
            brake = Mathf.Max(brake, 0.8f);
        }

        car.SetInputs(throttle, steer, brake, handbrake);
    }

    void CheckRecoveryNeeds()
    {
        if (recoveryCooldownTimer > 0f)
            return;

        float distanceToSafePoint = Vector3.Distance(transform.position, lastSafePosition);
        bool tooSlow = car.SpeedKph < stuckSpeedThreshold;
        bool offTrack = distanceToSafePoint > offTrackDistanceThreshold;
        bool flipped = Vector3.Dot(transform.up, Vector3.up) < 0.2f;

        if (tooSlow)
            stuckTimer += Time.deltaTime;
        else
            stuckTimer = 0f;

        if (offTrack)
            offTrackTimer += Time.deltaTime;
        else
            offTrackTimer = 0f;

        if (flipped || stuckTimer >= stuckTimeThreshold || offTrackTimer >= offTrackTimeThreshold)
        {
            RecoverCar();
        }
    }

    void SaveRecoveryPoint()
    {
        lastSafeWaypointIndex = currentWaypointIndex;

        Vector3 basePos = waypoints[lastSafeWaypointIndex].position;
        int nextIndex = (lastSafeWaypointIndex + 1) % waypoints.Length;
        Vector3 forward = (waypoints[nextIndex].position - basePos).normalized;
        forward.y = 0f;

        lastSafePosition = basePos + Vector3.up * recoveryHeightOffset;
        lastSafeRotation = forward.sqrMagnitude > 0.01f ? Quaternion.LookRotation(forward, Vector3.up) : transform.rotation;
    }

    void RecoverCar()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = lastSafePosition;
        transform.rotation = lastSafeRotation;
        currentWaypointIndex = lastSafeWaypointIndex;
        stuckTimer = 0f;
        offTrackTimer = 0f;
        recoveryCooldownTimer = recoveryCooldown;
        car.SetInputs(0f, 0f, 1f, false);
    }
}