using UnityEngine;

public class StartLoadScene : MonoBehaviour
{
    public GameManager GameManager;

    public Canvas GameOverCanvas;
    public Canvas CompleteCanvas;

    private void Start()
    {
        if (GameOverCanvas != null && CompleteCanvas != null)
        {
            GameOverCanvas.gameObject.SetActive(false);
            CompleteCanvas.gameObject.SetActive(false);
        }

        Data.globalHealthAmount = 3;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            GameManager.LoadRandomGame();
        }

        if (GameOverCanvas != null && CompleteCanvas != null)
        {
            Data.globalLevel = 1;

            if (Data.globalHasGameOver)
            {
                GameOverCanvas.gameObject.SetActive(true);
            }
            else
            {
                CompleteCanvas.gameObject.SetActive(true);
            }
        }
    }   
}
