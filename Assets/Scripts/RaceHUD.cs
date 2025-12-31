using TMPro;
using UnityEngine;

public class RaceHUD : MonoBehaviour
{
    public CarController playerCar;
    public LapManager lapManager;

    public TMP_Text speedText;
    public TMP_Text lapText;
    public TMP_Text positionText;

    public int currentPosition = 1;
    public int totalRacers = 4;

    void Update()
    {
        if (playerCar != null && speedText != null)
        {
            speedText.text = "Speed: " + Mathf.RoundToInt(playerCar.SpeedKph);
        }

        if (lapManager != null && lapText != null)
        {
            int displayLap = Mathf.Clamp(lapManager.CurrentLap + 1, 1, lapManager.TotalLaps);
            if (lapManager.RaceFinished)
                displayLap = lapManager.TotalLaps;

            lapText.text = "Lap: " + displayLap + "/" + lapManager.TotalLaps;
        }

        if (positionText != null)
        {
            positionText.text = "Pos: " + currentPosition + "/" + totalRacers;
        }
    }

    public void SetPosition(int newPosition, int racerCount)
    {
        currentPosition = newPosition;
        totalRacers = racerCount;
    }
}