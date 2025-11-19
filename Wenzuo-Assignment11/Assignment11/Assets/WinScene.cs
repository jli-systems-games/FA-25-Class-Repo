using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BallWinDisplay : MonoBehaviour
{
    [Header("UI")]
    public Image winnerColorImage;      // 上面的颜色块
    public TMP_Text winnerText;         // 下面的文字

    [Header("Settings")]
    public string winWord = "Wins";     // 显示的字
    public float checkInterval = 0.5f;  // 每隔多久检查一次剩余球数

    bool hasWinner = false;
    float timer = 0f;

    void Start()
    {
        if (winnerColorImage != null)
            winnerColorImage.enabled = false;

        if (winnerText != null)
            winnerText.text = "";
    }

    void Update()
    {
        if (hasWinner) return;

        timer += Time.deltaTime;
        if (timer < checkInterval) return;
        timer = 0f;

        BallMove[] balls = FindObjectsOfType<BallMove>();

        if (balls.Length == 1)
        {
            hasWinner = true;
            BallMove winner = balls[0];

            Renderer r = winner.GetComponent<Renderer>();
            if (r != null && winnerColorImage != null)
            {
                // 用球的颜色当作获胜颜色
                winnerColorImage.color = r.material.color;
                winnerColorImage.enabled = true;
            }

            if (winnerText != null)
            {
                winnerText.text = winWord;
            }
        }
        // 如果你想没球的时候写点什么，也可以在这里加 else if (balls.Length == 0) ...
    }
}
