using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ChooseButton : MonoBehaviour
{
    public bool giveSeat;
    public GameManager gameManager;
    public GameObject passenger;
    public GameObject passengerInLine;

    public void OnClick()
    {
        gameManager.Choose(passenger, passengerInLine, giveSeat);
    }
    public void OnNextSceneClick()
    {
        SceneManager.LoadScene("Main");
    }
}
