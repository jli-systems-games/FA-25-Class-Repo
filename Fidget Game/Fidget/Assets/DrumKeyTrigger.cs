using UnityEngine;

public class DrumKeyTrigger : MonoBehaviour
{
    [Header("Input")]
    public KeyCode key = KeyCode.Q;     // 按键

    [Header("Audio")]
    public AudioSource source;          // 播放器
    public AudioClip clip;              // 鼓声音效
    public float volume = 1f;

    [Header("Timing")]
    public float firstDelay = 0.5f;     // 第一次延迟
    public float repeatInterval = 1f;   // 持续按住的循环间隔

    private float timer = -1f;          // 计时器
    private bool isHolding = false;     // 是否在按住

    void Update()
    {
        // 刚按下：启动计时器
        if (Input.GetKeyDown(key))
        {
            isHolding = true;
            timer = firstDelay; // 延迟0.5s
        }

        // 松开：停止
        if (Input.GetKeyUp(key))
        {
            isHolding = false;
            timer = -1f;
        }

        // 计时逻辑
        if (isHolding && timer >= 0f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                PlaySound();
                timer = repeatInterval; // 重置为循环间隔
            }
        }
    }

    void PlaySound()
    {
        if (source && clip)
            source.PlayOneShot(clip, volume);
    }
}
