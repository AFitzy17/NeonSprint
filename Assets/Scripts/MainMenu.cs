using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string raceSceneName = "RaceTrack01";

    public void PlayGame()
    {
        SceneManager.LoadScene(raceSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}