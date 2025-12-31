using System.Collections;
using TMPro;
using UnityEngine;

public class RaceCountdown : MonoBehaviour
{
    public TMP_Text countdownText;
    public CarController playerCar;
    public AIInput[] aiCars;

    public float countDelay = 1f;
    public float goDisplayTime = 1f;

    void Start()
    {
        if (playerCar != null)
            playerCar.canDrive = false;

        for (int i = 0; i < aiCars.Length; i++)
        {
            if (aiCars[i] != null)
                aiCars[i].canDrive = false;
        }

        StartCoroutine(CountdownRoutine());
    }

    IEnumerator CountdownRoutine()
    {
        countdownText.gameObject.SetActive(true);

        countdownText.text = "3";
        yield return new WaitForSeconds(countDelay);

        countdownText.text = "2";
        yield return new WaitForSeconds(countDelay);

        countdownText.text = "1";
        yield return new WaitForSeconds(countDelay);

        countdownText.text = "GO!";

        if (playerCar != null)
            playerCar.canDrive = true;

        for (int i = 0; i < aiCars.Length; i++)
        {
            if (aiCars[i] != null)
                aiCars[i].canDrive = true;
        }

        yield return new WaitForSeconds(goDisplayTime);

        countdownText.gameObject.SetActive(false);
    }
}