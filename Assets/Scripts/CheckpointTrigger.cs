using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    public int checkpointNumber = 0;
    public LapManager lapManager;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody hitBody = other.attachedRigidbody;

        if (hitBody == null) return;
        if (!hitBody.CompareTag("Player")) return;

        Debug.Log("Player entered trigger: " + gameObject.name);

        if (checkpointNumber == 0)
        {
            lapManager.CrossStartFinish();
        }
        else
        {
            lapManager.PassCheckpoint(checkpointNumber);
        }
    }
}