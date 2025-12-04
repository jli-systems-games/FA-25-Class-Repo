using Hertzole.GoldPlayer;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public Canvas tutorialCanvas;
    public Canvas hintVisionCanvas;

    public TextMeshProUGUI tutorialTextArea;

    public GoldPlayerInputSystem playerInputSys;

    void Start()
    {
        hintVisionCanvas.gameObject.SetActive(false);
        tutorialCanvas.gameObject.SetActive(false);
    }
}
