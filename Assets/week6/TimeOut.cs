using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeOut : MonoBehaviour
{
    public static TimeOut Instance;

    public float duration = 65f;       // 제한 시간(초)
    public string failSceneName = "fail";

    bool ended;        // 성공/실패로 종료 여부
    float startRealtime; // Time.timeScale 무시하고 실시간 카운트

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void OnEnable()
    {
        ended = false;
        startRealtime = Time.realtimeSinceStartup; // 언스케일드 시작 시간
    }

    void Update()
    {
        if (ended) return;

        float elapsed = Time.realtimeSinceStartup - startRealtime;
        if (elapsed >= duration)
        {
            ended = true;
            // 혹시 멈춰있다면 풀고 실패 씬 로드
            if (Time.timeScale != 1f) Time.timeScale = 1f;
            SceneManager.LoadScene(failSceneName);
        }
    }

    // 트리거 밟아 성공했을 때 호출
    public void RegisterSuccess()
    {
        if (ended) return;
        ended = true; // 타이머 종료만 표시 (씬 전환은 성공쪽에서)
    }
}