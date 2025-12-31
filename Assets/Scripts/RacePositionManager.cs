using UnityEngine;

public class RacePositionManager : MonoBehaviour
{
    public RacerProgress[] racers;
    public Transform checkpoint1;
    public Transform checkpoint2;
    public Transform checkpoint3;
    public Transform startFinish;

    public int GetPosition(RacerProgress targetRacer)
    {
        int position = 1;
        float targetValue = GetRaceValue(targetRacer);

        for (int i = 0; i < racers.Length; i++)
        {
            if (racers[i] == null || racers[i] == targetRacer) continue;

            float otherValue = GetRaceValue(racers[i]);
            if (otherValue > targetValue)
            {
                position++;
            }
        }

        return position;
    }

    public int GetRacerCount()
    {
        return racers.Length;
    }

    float GetRaceValue(RacerProgress racer)
    {
        float baseScore = racer.currentLap * 100000f;
        baseScore += racer.CheckpointProgress * 10000f;

        Transform targetCheckpoint = GetTargetCheckpoint(racer.nextCheckpoint);
        if (targetCheckpoint != null)
        {
            float distance = Vector3.Distance(racer.transform.position, targetCheckpoint.position);
            baseScore -= distance;
        }

        if (racer.raceFinished)
        {
            baseScore += 1000000f;
        }

        return baseScore;
    }

    Transform GetTargetCheckpoint(int nextCheckpoint)
    {
        if (nextCheckpoint == 1) return checkpoint1;
        if (nextCheckpoint == 2) return checkpoint2;
        if (nextCheckpoint == 3) return checkpoint3;
        return startFinish;
    }
}