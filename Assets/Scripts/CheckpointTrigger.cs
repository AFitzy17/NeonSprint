using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    public int checkpointNumber = 0;

    void OnTriggerEnter(Collider other)
    {
        Rigidbody hitBody = other.attachedRigidbody;
        if (hitBody == null) return;

        RacerProgress racer = hitBody.GetComponent<RacerProgress>();
        if (racer == null) return;

        if (checkpointNumber == 0)
        {
            racer.CrossStartFinish();
        }
        else
        {
            racer.PassCheckpoint(checkpointNumber);
        }
    }
}