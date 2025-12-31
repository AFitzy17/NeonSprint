using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceResultsUI : MonoBehaviour
{
    public RacerProgress playerProgress;
    public RacePositionManager positionManager;
    public CarController playerCar;
    public AIInput[] aiCars;

    public GameObject raceHUDRoot;
    public GameObject resultsPanel;

    public TMP_Text finishText;
    public TMP_Text finalPositionText;
    public TMP_Text restartText;
    public TMP_Text quitText;

    bool resultsShown;

    void Start()
    {
        resultsShown = false;

        if (resultsPanel != null)
            resultsPanel.SetActive(false);

        if (raceHUDRoot != null)
            raceHUDRoot.SetActive(true);
    }

    void Update()
    {
        if (!resultsShown && playerProgress != null && playerProgress.raceFinished)
        {
            ShowResults();
        }

        if (resultsShown)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }

    void ShowResults()
    {
        resultsShown = true;

        if (playerCar != null)
            playerCar.canDrive = false;

        for (int i = 0; i < aiCars.Length; i++)
        {
            if (aiCars[i] != null)
                aiCars[i].canDrive = false;
        }

        if (raceHUDRoot != null)
            raceHUDRoot.SetActive(false);

        if (resultsPanel != null)
            resultsPanel.SetActive(true);

        if (finishText != null)
            finishText.text = "FINISHED!";

        if (positionManager != null && finalPositionText != null && playerProgress != null)
        {
            int finalPosition = positionManager.GetPosition(playerProgress);
            int racerCount = positionManager.GetRacerCount();
            finalPositionText.text = "Position: " + finalPosition + "/" + racerCount;
        }

        if (restartText != null)
            restartText.text = "Press R to Restart";

        if (quitText != null)
            quitText.text = "Press Esc to Quit";
    }
}