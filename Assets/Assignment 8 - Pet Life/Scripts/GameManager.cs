using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameStat statAsset;
    public Canvas guideCanvas;
    public bool isGuide = true;

    private void Awake()
    {
        Data.isFinished = false;
        Data.movementSpeed = 2.5f;
        Data.feedAmount = 8;
    }
    void Start()
    {
        statAsset.ResetStats();
        Time.timeScale = 0;
        isGuide = true;
        guideCanvas.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!isGuide)
            {
                Time.timeScale = 0;
                guideCanvas.gameObject.SetActive(true);
                isGuide = true;
            }
            else
            {
                Time.timeScale = 1;
                guideCanvas.gameObject.SetActive(false);
                isGuide = false;
            }
        }

        if (Data.isFinished)
        {
            statAsset.EndStats();

            if (Input.GetKeyDown(KeyCode.Return))
            {
                SceneManager.LoadScene("Start Scene");
            }
        }
    }
}
