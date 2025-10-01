using UnityEngine;

public class PlayerEnter : MonoBehaviour
{
    public GameObject FPCCam;
    public GameObject carCam;

    public bool hasEnteredCar = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Car Enter"))
        {
            Debug.Log("triggered.");
            carCam.SetActive(true);
            FPCCam.SetActive(false);

            hasEnteredCar = true;

            gameObject.SetActive(false);
        }
    }
}
