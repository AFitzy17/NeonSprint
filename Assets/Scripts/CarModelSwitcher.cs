using UnityEngine;

public class CarModelSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class CarVisualSet
    {
        public GameObject root;
        public Transform frontLeftWheel;
        public Transform frontRightWheel;
        public Transform rearLeftWheel;
        public Transform rearRightWheel;
    }

    public CarVisualSet[] carModels;
    public int currentIndex;

    CarController carController;

    void Start()
    {
        carController = GetComponent<CarController>();
        ApplyModel(currentIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            currentIndex++;
            if (currentIndex >= carModels.Length)
                currentIndex = 0;

            ApplyModel(currentIndex);
        }
    }

    void ApplyModel(int index)
    {
        if (carModels == null || carModels.Length == 0) return;

        for (int i = 0; i < carModels.Length; i++)
        {
            if (carModels[i].root != null)
                carModels[i].root.SetActive(i == index);
        }

        CarVisualSet activeModel = carModels[index];

        if (carController != null)
        {
            carController.SetWheelVisuals(
                activeModel.frontLeftWheel,
                activeModel.frontRightWheel,
                activeModel.rearLeftWheel,
                activeModel.rearRightWheel
            );
        }
    }
}