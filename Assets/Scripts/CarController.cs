using UnityEngine;

public class CarController : MonoBehaviour
{
    public float acceleration = 25f;
    public float brakeForce = 12f;
    public float reverseAcceleration = 18f;

    public float maxSpeed = 22f;
    public float maxReverseSpeed = 10f;

    public float grip = 0.9f;
    public float coastDrag = 0.985f;

    public float lowSpeedTurnRate = 65f;
    public float midSpeedTurnRate = 110f;
    public float highSpeedTurnRate = 80f;

    public float turnResponseSpeed = 8f;
    public float reverseTurnMultiplier = 0.75f;

    private Rigidbody rb;
    private float currentTurnInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);

        // Reduce sideways slide
        localVelocity.x *= grip;

        // Natural slowdown when coasting
        if (Mathf.Abs(moveInput) < 0.1f)
        {
            localVelocity.z *= coastDrag;
        }

        float forwardSpeed = localVelocity.z;
        float absForwardSpeed = Mathf.Abs(forwardSpeed);

        // Steering curve:
        // low speed -> moderate
        // mid speed -> strongest
        // high speed -> weaker, but not harsh
        float speedFactor = Mathf.Clamp01(absForwardSpeed / maxSpeed);

        float targetTurnRate;
        if (speedFactor < 0.4f)
        {
            float t = speedFactor / 0.4f;
            targetTurnRate = Mathf.Lerp(lowSpeedTurnRate, midSpeedTurnRate, t);
        }
        else
        {
            float t = (speedFactor - 0.4f) / 0.6f;
            targetTurnRate = Mathf.Lerp(midSpeedTurnRate, highSpeedTurnRate, t);
        }

        // Smooth steering input to stop sudden snap
        currentTurnInput = Mathf.Lerp(currentTurnInput, turnInput, turnResponseSpeed * Time.fixedDeltaTime);

        if (absForwardSpeed > 0.15f)
        {
            float direction = forwardSpeed >= 0 ? 1f : -1f;
            float reverseMultiplier = forwardSpeed < 0 ? reverseTurnMultiplier : 1f;

            float turnAmount = currentTurnInput * targetTurnRate * reverseMultiplier * direction * Time.fixedDeltaTime;
            transform.Rotate(0f, turnAmount, 0f);
        }

        // Forward
        if (moveInput > 0.1f)
        {
            if (forwardSpeed < maxSpeed)
            {
                localVelocity.z += acceleration * Time.fixedDeltaTime;
            }
        }
                // Brake / Reverse
        else if (moveInput < -0.1f)
        {
            // If moving forward at decent speed, brake
            if (forwardSpeed > 1.5f)
            {
                localVelocity.z -= brakeForce * Time.fixedDeltaTime;
            }
            // Otherwise go straight into reverse
            else
            {
                localVelocity.z -= reverseAcceleration * Time.fixedDeltaTime;
                localVelocity.z = Mathf.Max(localVelocity.z, -maxReverseSpeed);
            }
        }

        rb.linearVelocity = transform.TransformDirection(localVelocity);
    }
}