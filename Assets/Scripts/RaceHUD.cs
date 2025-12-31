using TMPro;
using UnityEngine;

public class RaceHUD : MonoBehaviour
{
    public CarController playerCar;
    public RacerProgress playerProgress;
    public RacePositionManager positionManager;

    public TMP_Text speedText;
    public TMP_Text lapText;
    public TMP_Text positionText;

    void Update()
    {
        if (playerCar != null && speedText != null)
        {
            speedText.text = "Speed: " + Mathf.RoundToInt(playerCar.SpeedKph);
        }

        if (playerProgress != null && lapText != null)
        {
            int displayLap = Mathf.Clamp(playerProgress.currentLap + 1, 1, playerProgress.totalLaps);
            if (playerProgress.raceFinished)
            {
                displayLap = playerProgress.totalLaps;
            }

            lapText.text = "Lap: " + displayLap + "/" + playerProgress.totalLaps;
        }

        if (playerProgress != null && positionManager != null && positionText != null)
        {
            int position = positionManager.GetPosition(playerProgress);
            int racerCount = positionManager.GetRacerCount();
            positionText.text = "Pos: " + position + "/" + racerCount;
        }
    }
}