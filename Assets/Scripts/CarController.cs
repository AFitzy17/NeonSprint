using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public Transform rearLeftWheel;
    public Transform rearRightWheel;

    public float acceleration = 36f;
    public float reverseAcceleration = 16f;
    public float brakeForce = 24f;
    public float handbrakeForce = 18f;
    public float maxForwardSpeed = 48f;
    public float maxReverseSpeed = 14f;

    public float lowSpeedTurnRate = 120f;
    public float highSpeedTurnRate = 42f;
    public float turnSpeedFalloff = 50f;

    public float throttleSharpness = 6f;
    public float steerSharpness = 5f;
    public float brakeSharpness = 6f;

    public float lateralGrip = 5.5f;
    public float driftGrip = 3.5f;
    public float handbrakeGrip = 1.6f;
    public float gripSharpness = 3f;
    public float driftThreshold = 0.65f;
    public float coastDrag = 0.8f;
    public float handbrakeYawBoost = 1.35f;
    public float yawSharpness = 3f;
    public float handbrakeBrakeStrength = 0.12f;
    public float handbrakeBrakeSharpness = 3f;

    public float wheelRadius = 0.4f;
    public Vector3 centerOfMassOffset = new Vector3(0f, -0.55f, 0f);

    public bool usePlayerInput = true;

    float throttleInput;
    float steerInput;
    float brakeInput;
    bool handbrakeInput;

    float currentThrottle;
    float currentSteer;
    float currentBrake;
    float currentGrip;
    float currentYawBoost = 1f;
    float currentHandbrakeBrake;

    Rigidbody rb;

    public float ForwardSpeed { get; private set; }
    public float SpeedKph => rb.linearVelocity.magnitude * 3.6f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass += centerOfMassOffset;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        currentGrip = lateralGrip;
    }

    void Update()
    {
        if (usePlayerInput)
        {
            float vertical = Input.GetAxisRaw("Vertical");
            steerInput = Input.GetAxisRaw("Horizontal");
            handbrakeInput = Input.GetKey(KeyCode.Space);

            if (vertical > 0f)
            {
                throttleInput = 1f;
                brakeInput = 0f;
            }
            else if (vertical < 0f)
            {
                if (ForwardSpeed > 1f)
                {
                    throttleInput = 0f;
                    brakeInput = 1f;
                }
                else
                {
                    throttleInput = -1f;
                    brakeInput = 0f;
                }
            }
            else
            {
                throttleInput = 0f;
                brakeInput = 0f;
            }
        }

        AnimateWheels();
    }

    void FixedUpdate()
    {
        currentThrottle = Mathf.Lerp(currentThrottle, throttleInput, throttleSharpness * Time.fixedDeltaTime);
        currentSteer = Mathf.Lerp(currentSteer, steerInput, steerSharpness * Time.fixedDeltaTime);
        currentBrake = Mathf.Lerp(currentBrake, brakeInput, brakeSharpness * Time.fixedDeltaTime);

        Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
        ForwardSpeed = localVelocity.z;

        float steerAmountAbs = Mathf.Abs(currentSteer);
        float speedAbs = Mathf.Abs(localVelocity.z);

        float targetGrip = lateralGrip;
        float targetYawBoost = 1f;
        float targetHandbrakeBrake = 0f;

        if (handbrakeInput && speedAbs > 4f)
        {
            targetGrip = handbrakeGrip;
            targetYawBoost = handbrakeYawBoost;
            targetHandbrakeBrake = handbrakeBrakeStrength;
        }
        else if (steerAmountAbs > driftThreshold)
        {
            targetGrip = driftGrip;
        }

        currentGrip = Mathf.Lerp(currentGrip, targetGrip, gripSharpness * Time.fixedDeltaTime);
        currentYawBoost = Mathf.Lerp(currentYawBoost, targetYawBoost, yawSharpness * Time.fixedDeltaTime);
        currentHandbrakeBrake = Mathf.Lerp(currentHandbrakeBrake, targetHandbrakeBrake, handbrakeBrakeSharpness * Time.fixedDeltaTime);

        localVelocity.x = Mathf.Lerp(localVelocity.x, 0f, currentGrip * Time.fixedDeltaTime);

        if (currentThrottle > 0f)
        {
            if (localVelocity.z < maxForwardSpeed)
            {
                float speedRatio = Mathf.Clamp01(localVelocity.z / maxForwardSpeed);
                float accelFactor = Mathf.Lerp(1f, 0.18f, speedRatio * speedRatio);
                localVelocity.z += acceleration * currentThrottle * accelFactor * Time.fixedDeltaTime;
            }
        }
        else if (currentThrottle < 0f)
        {
            if (localVelocity.z > -maxReverseSpeed)
            {
                float reverseRatio = Mathf.Clamp01(Mathf.Abs(localVelocity.z) / maxReverseSpeed);
                float reverseFactor = Mathf.Lerp(1f, 0.25f, reverseRatio * reverseRatio);
                localVelocity.z -= reverseAcceleration * Mathf.Abs(currentThrottle) * reverseFactor * Time.fixedDeltaTime;
            }
        }

        if (currentBrake > 0f && localVelocity.z > 0f)
        {
            localVelocity.z -= brakeForce * currentBrake * Time.fixedDeltaTime;
        }

        if (Mathf.Abs(currentThrottle) < 0.01f && currentBrake < 0.01f)
        {
            localVelocity.z = Mathf.Lerp(localVelocity.z, 0f, coastDrag * Time.fixedDeltaTime);
        }

        if (speedAbs > 4f)
        {
            localVelocity.z = Mathf.Lerp(localVelocity.z, 0f, currentHandbrakeBrake * Time.fixedDeltaTime);
        }

        localVelocity.z = Mathf.Clamp(localVelocity.z, -maxReverseSpeed, maxForwardSpeed);
        rb.linearVelocity = transform.TransformDirection(localVelocity);

        float lowSpeedFactor = Mathf.InverseLerp(1.5f, 8f, speedAbs);
        float highSpeedFactor = Mathf.Clamp01(speedAbs / turnSpeedFalloff);

        float baseTurnRate = Mathf.Lerp(lowSpeedTurnRate * 0.08f, lowSpeedTurnRate, lowSpeedFactor);
        float turnRate = Mathf.Lerp(baseTurnRate, highSpeedTurnRate, highSpeedFactor);

        if (speedAbs > 0.15f)
        {
            float direction = localVelocity.z >= 0f ? 1f : -1f;
            float steerStep = currentSteer * turnRate * currentYawBoost * direction * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, steerStep, 0f));
        }
    }

    void AnimateWheels()
    {
        float rotationAmount = (rb.linearVelocity.magnitude / (2f * Mathf.PI * wheelRadius)) * 360f * Time.deltaTime;

        rearLeftWheel.Rotate(Vector3.right, rotationAmount, Space.Self);
        rearRightWheel.Rotate(Vector3.right, rotationAmount, Space.Self);

        float steerVisual = currentSteer * 30f;

        frontLeftWheel.localRotation = Quaternion.Euler(frontLeftWheel.localEulerAngles.x + rotationAmount, steerVisual, 0f);
        frontRightWheel.localRotation = Quaternion.Euler(frontRightWheel.localEulerAngles.x + rotationAmount, steerVisual, 0f);
    }

    public void SetInputs(float throttle, float steer, float brake, bool handbrake)
    {
        throttleInput = Mathf.Clamp(throttle, -1f, 1f);
        steerInput = Mathf.Clamp(steer, -1f, 1f);
        brakeInput = Mathf.Clamp01(brake);
        handbrakeInput = handbrake;
    }

    public void SetWheelVisuals(Transform fl, Transform fr, Transform rl, Transform rr)
    {
        frontLeftWheel = fl;
        frontRightWheel = fr;
        rearLeftWheel = rl;
        rearRightWheel = rr;
    }
}