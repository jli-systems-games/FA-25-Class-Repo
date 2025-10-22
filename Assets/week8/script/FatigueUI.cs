using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FatigueUI : MonoBehaviour
{
    public GrandpaController grandpa;
    public Slider slider;

    public bool smooth = true;
    public float smoothSpeed = 50f; // 1초당 이동량(게이지 단위: 0~100)

    public bool manageScenesHere = true;
    public string successSceneName = "Success";
    public string gameOverSceneName = "GameOver";
    [Range(0f, 5f)] public float winLoseThreshold = 0.01f;

    float currentValue;
    bool ended = false;

    void Start()
    {
        if (slider != null)
        {
            slider.minValue = 0f;
            slider.maxValue = 100f;
            slider.wholeNumbers = false;
        }
        if (grandpa != null)
        {
            currentValue = Mathf.Clamp(grandpa.GetLife(), 0f, 100f);
            if (slider) slider.value = currentValue;
        }
    }

    void Update()
    {
        if (grandpa == null || slider == null || ended) return;

        // 1) 씬 전환은 '원시 Life 값'으로 즉시 판정 (보간과 무관)
        float raw = Mathf.Clamp(grandpa.GetLife(), 0f, 100f);
        if (manageScenesHere)
        {
            if (raw <= winLoseThreshold)
            {
                ended = true;
                SceneManager.LoadScene(gameOverSceneName);
                return;
            }
            if (raw >= 100f - winLoseThreshold)
            {
                ended = true;
                SceneManager.LoadScene(successSceneName);
                return;
            }
        }

        // 2) UI 값은 MoveTowards로 부드럽게, 그리고 '정확히' 도달
        if (smooth)
            currentValue = Mathf.MoveTowards(currentValue, raw, smoothSpeed * Time.deltaTime);
        else
            currentValue = raw;

        slider.value = currentValue;
    }
}
