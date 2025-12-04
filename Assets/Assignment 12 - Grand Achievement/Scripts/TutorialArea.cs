using TMPro;
using UnityEngine;

public class TutorialArea : TutorialManager
{
    [TextArea(1, 5)]
    public string tutorialText;

    public bool hintVisionEnabling = false;
    private bool inTutorialMode = false;
    private bool tutorialDone = false;

    private void Update()
    {
        if (inTutorialMode)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                tutorialCanvas.gameObject.SetActive(false);
                inTutorialMode = false;

                tutorialDone = true;

                if (hintVisionEnabling)
                {
                    hintVisionCanvas.gameObject.SetActive(true);
                    Data.hintVisionEnabled = true;
                }

                playerInputSys.enabled = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!tutorialDone && other.gameObject.CompareTag("Player"))
        {
            tutorialTextArea.text = tutorialText;
            tutorialCanvas.gameObject.SetActive(true);
            inTutorialMode = true;

            playerInputSys.enabled = false;
        }
    }
}
