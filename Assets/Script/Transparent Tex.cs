using UnityEngine;
using TMPro;

public class AutoDisableAfterTime : MonoBehaviour
{
    [Header("自动关闭的延迟时间（秒）")]
    public float disableDelay = 2f;

    [Header("是否在启用时重新计时")]
    public bool restartOnEnable = true;

    private float timer = 0f;
    private bool counting = false;

    void OnEnable()
    {
        if (restartOnEnable)
        {
            timer = 0f;
            counting = true;
        }
    }

    void Update()
    {
        if (!counting) return;

        timer += Time.deltaTime;

        if (timer >= disableDelay)
        {
            gameObject.SetActive(false);
        }
    }

    // ✅ 如果你想从别的脚本触发延时关闭，可以调用这个
    public void StartCountdown(float time)
    {
        disableDelay = time;
        timer = 0f;
        counting = true;
    }
}