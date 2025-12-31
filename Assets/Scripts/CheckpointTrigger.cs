using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    public int checkpointNumber = 0; // 0 = start/finish
    public LapManager lapManager;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

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