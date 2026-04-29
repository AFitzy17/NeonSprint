using UnityEngine;

public class LapManager : MonoBehaviour
{
    public int totalLaps = 3;
    public int currentLap = 0;

    int nextCheckpoint = 1;
    bool raceFinished = false;

    public int CurrentLap => currentLap;
    public int TotalLaps => totalLaps;
    public int NextCheckpoint => nextCheckpoint;
    public bool RaceFinished => raceFinished;

    public void PassCheckpoint(int checkpointNumber)
    {
        if (raceFinished) return;

        if (checkpointNumber == nextCheckpoint)
        {
            nextCheckpoint++;
            Debug.Log("Passed Checkpoint " + checkpointNumber);
        }
    }

    public void CrossStartFinish()
    {
        if (raceFinished) return;

        if (nextCheckpoint > 3)
        {
            currentLap++;
            nextCheckpoint = 1;

            Debug.Log("Lap " + currentLap + " completed");

            if (currentLap >= totalLaps)
            {
                raceFinished = true;
                Debug.Log("Race Finished!");
            }
        }
        else
        {
            Debug.Log("Missed checkpoint(s), lap not counted");
        }
    }
}