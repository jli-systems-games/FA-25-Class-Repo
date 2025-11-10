using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform cat;
    public TMP_Text winText;
    public GameObject replayButton;
    public float winDistance = 5f;

    private bool hasWon = false;

    void Start()
    {
        if (winText != null)
            winText.gameObject.SetActive(false);

        if (replayButton != null)
            replayButton.SetActive(false);
    }

    void Update()
    {
        if (hasWon || cat == null || player == null)
            return;

        float distance = Vector3.Distance(player.position, cat.position);

        if (distance <= winDistance)
        {
            hasWon = true;
            ShowWinScreen();
        }
    }

    void ShowWinScreen()
    {
        if (winText != null)
        {
            winText.gameObject.SetActive(true);
            winText.text = "You found your familiar!";
        }

        if (replayButton != null)
            replayButton.SetActive(true);

      
        StopwatchTimer timer = FindObjectOfType<StopwatchTimer>();
        if (timer != null)
            timer.StopTimer();

        Debug.Log(" Win triggered! Cat found.");
    }

   
    public void ReplayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetCat(Transform catTransform)
    {
        cat = catTransform;
    }
}
