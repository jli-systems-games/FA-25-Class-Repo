using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class JumpToAdvance : MonoBehaviour
{
    [Header("Win Condition")]
    public int jumpsToWin = 3;               // 需要的总跳跃次数
    public string nextSceneName = "Scene3";  // 跳转到的场景名（记得加到 Build Settings）

    [Header("Input")]
    public KeyCode jumpKey = KeyCode.Space;  // 统计哪个键的按下算“跳一次”
    public float debounce = 0.15f;           // 去抖，避免长按被多次统计

    [Header("UI (optional)")]
    public TMP_Text counterText;             // 可选：显示“Jumps: X / N”

    [Header("SFX (optional)")]
    public AudioSource uiAudio;              // 可选：普通 2D 音源
    public AudioClip countSfx;               // 计数音效
    public AudioClip winSfx;                 // 达成音效

    int count = 0;
    bool canCount = true;
    bool finished = false;

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        if (finished) return;

        if (Input.GetKeyDown(jumpKey) && canCount)
        {
            CountOne();
        }
    }

    void CountOne()
    {
        canCount = false;                    // 去抖
        Invoke(nameof(ResetDebounce), debounce);

        count++;
        if (uiAudio && countSfx) uiAudio.PlayOneShot(countSfx);
        UpdateUI();

        if (count >= jumpsToWin)
        {
            finished = true;
            if (uiAudio && winSfx) uiAudio.PlayOneShot(winSfx);
            // 小延时给音效时间
            Invoke(nameof(LoadNext), 0.25f);
        }
    }

    void ResetDebounce() => canCount = true;

    void UpdateUI()
    {
        if (counterText)
            counterText.text = $"Jumps: {count} / {jumpsToWin}";
    }

    void LoadNext()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }
}
