using UnityEngine;
using UnityEngine.Events;

public class GameManagerPong : MonoBehaviour
{
    public PaintManager3D paint;
    public float matchDuration = 90f;
    public UnityEvent onMatchEnd;

    float timeLeft;
    bool ended;

    void Start() { timeLeft = matchDuration; }

    void Update()
    {
        if (ended) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            ended = true;
            var (day, night) = paint.GetScores();
            string result = day > night ? "Day Wins" : day < night ? "Night Wins" : "Draw";
            Debug.Log($"[Match End] Day={day}, Night={night} -> {result}");
            onMatchEnd?.Invoke();
            // TODO: 停止输入/停球、弹出结算 UI
        }
    }

    public float GetTimeLeft() => Mathf.Max(0f, timeLeft);
}