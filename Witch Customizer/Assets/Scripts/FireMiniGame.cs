using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class FireMiniGame : MonoBehaviour
{
    public TMP_Text timerText, resultText;
    public float timeLimit = 15f;
    private float timer;
    private int litCount = 0;
    public int totalTorches = 5;
    private bool finished = false;

    void Start()
    {
        timer = timeLimit;
        resultText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (finished) return;

        timer -= Time.deltaTime;
        timerText.text = "Time: " + Mathf.Ceil(timer);

        if (timer <= 0 && litCount < totalTorches)
            Lose();
    }

    public void LightTorch()
    {
        litCount++;
        if (litCount >= totalTorches)
            Win();
    }

    void Win()
    {
        finished = true;
        resultText.gameObject.SetActive(true);
        resultText.text = $"You mastered Fire!";
        Invoke(nameof(ReturnToTitle), 2.5f);
    }

    void Lose()
    {
        finished = true;
        resultText.gameObject.SetActive(true);
        resultText.text = "The flame fades...";
        Invoke(nameof(ReturnToTitle), 2.5f);
    }

    void ReturnToTitle()
    {
        Data.Instance.ResetData();
        SceneManager.LoadScene("Menu");
    }
}
