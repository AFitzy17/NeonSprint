using UnityEngine;

public class RacerProgress : MonoBehaviour
{
    public int totalLaps = 3;

    public int currentLap;
    public int nextCheckpoint = 1;
    public bool raceFinished;

    public int CheckpointProgress
    {
        get
        {
            return nextCheckpoint - 1;
        }
    }

    public int ProgressScore
    {
        get
        {
            return currentLap * 1000 + CheckpointProgress * 100;
        }
    }

    public void PassCheckpoint(int checkpointNumber)
    {
        if (raceFinished) return;

        if (checkpointNumber == nextCheckpoint)
        {
            nextCheckpoint++;
        }
    }

    public void CrossStartFinish()
    {
        if (raceFinished) return;

        if (nextCheckpoint > 3)
        {
            currentLap++;
            nextCheckpoint = 1;

            if (currentLap >= totalLaps)
            {
                raceFinished = true;
            }
        }
    }
}