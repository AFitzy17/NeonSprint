using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public float distance = 6f;
    public float height = 3f;
    public float followSpeed = 5f;
    public float rotationSpeed = 3f;

    public float resetDelay = 2f;
    public float resetSpeed = 2f;

    private float currentYaw = 0f;
    private float defaultYaw = 0f;
    private float timeSinceMouseLook = 0f;

    void LateUpdate()
    {
        if (target == null) return;

        // Hold right mouse button to rotate camera
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            currentYaw += mouseX * rotationSpeed;
            timeSinceMouseLook = 0f;
        }
        else
        {
            timeSinceMouseLook += Time.deltaTime;

            // Reset camera behind the car after delay
            if (timeSinceMouseLook >= resetDelay)
            {
                float targetYaw = target.eulerAngles.y;
                currentYaw = Mathf.LerpAngle(currentYaw, targetYaw, resetSpeed * Time.deltaTime);
            }
        }

        Quaternion rotation = Quaternion.Euler(0f, currentYaw, 0f);
        Vector3 offset = rotation * new Vector3(0f, height, -distance);
        Vector3 targetPosition = target.position + offset;

        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }

    void Start()
    {
        if (target != null)
        {
            currentYaw = target.eulerAngles.y;
            defaultYaw = currentYaw;
        }
    }
}