using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FatigueUI : MonoBehaviour
{
    [Header("Refs")]
    public GrandpaController grandpa;
    public Slider slider;

    [Header("UI 옵션")]
    public bool smooth = true;
    public float smoothSpeed = 10f;

    [Header("Scene Handling (이 스크립트에서 승패 처리할지 여부)")]
    public bool manageScenesHere = true;
    public string successSceneName = "Success";
    public string gameOverSceneName = "GameOver";

    // 0/100 판정 민감도(슬라이더 보간시 99.9/0.1 등으로 머무는 경우 대비)
    [Range(0f, 5f)] public float winLoseThreshold = 0.01f;

    float currentValue;
    bool ended = false; // 씬 중복 로드 방지

    private void Start()
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

    private void Update()
    {
        if (grandpa == null || slider == null) return;
        if (ended) return;

        float target = Mathf.Clamp(grandpa.GetLife(), 0f, 100f);
        currentValue = smooth
            ? Mathf.Lerp(currentValue, target, Time.deltaTime * smoothSpeed)
            : target;

        slider.value = currentValue;

        if (!manageScenesHere) return;

        // 승패 판정 (보간/부동소수 오차 대비 임계값 사용)
        if (currentValue <= winLoseThreshold)
        {
            ended = true;
            SceneManager.LoadScene(gameOverSceneName);
            return;
        }
        if (currentValue >= 100f - winLoseThreshold)
        {
            ended = true;
            SceneManager.LoadScene(successSceneName);
            return;
        }
    }
}
