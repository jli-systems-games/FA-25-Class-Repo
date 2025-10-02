using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoLoader : MonoBehaviour
{
    [Header("자동 전환 설정")]
    [Tooltip("지연 시간(초). 기본 15초")]
    public float delaySeconds = 15f;

    [Tooltip("전환할 씬 이름")]
    public string targetSceneName = "game";

    [Tooltip("TimeScale의 영향 없이 실제 시간 기준으로 카운트할지")]
    public bool useUnscaledTime = true;

    [Tooltip("아무 키를 누르면 즉시 전환(옵션)")]
    public bool allowSkipWithAnyKey = false;

    float elapsed;
    bool loading;

    void OnEnable()
    {
        elapsed = 0f;
        loading = false;
    }

    void Update()
    {
        if (loading) return;

        elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        if (allowSkipWithAnyKey && Input.anyKeyDown)
        {
            Load();
            return;
        }

        if (elapsed >= delaySeconds)
        {
            Load();
        }
    }

    void Load()
    {
        if (loading) return;
        loading = true;
        SceneManager.LoadScene(targetSceneName);
    }
}
