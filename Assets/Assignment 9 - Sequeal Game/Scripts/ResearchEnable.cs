using UnityEngine;

public class ResearchEnable : MonoBehaviour
{
    public Canvas researchCanvas;
    private bool isResearchOpen = false;

    private void Start()
    {
        researchCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!isResearchOpen)
            {
                researchCanvas.gameObject.SetActive(true);
                Time.timeScale = 0;
                isResearchOpen = true;
            }
            else
            {
                researchCanvas.gameObject.SetActive(false);
                Time.timeScale = 1;
                isResearchOpen = false;
            }
        }
    }
}
