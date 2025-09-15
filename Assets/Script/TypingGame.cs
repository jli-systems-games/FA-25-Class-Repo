using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TypingGame : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text TargetText;   // 目标词：显示 revealDuration 秒后清空
    public TMP_Text ProgressText; // 仅显示“已输入”的字符；未输入时留空
    public TMP_Text HudText;      // x/x 与可选倒计时

    [Header("Rules")]
    public bool caseSensitive = false;
    public float totalTimeLimit = 0f;  // 0 = 不限时
    public int goalCount = 0;          // 0 或超出词表长度则=词表长度
    public float revealDuration = 2f;  // 目标词可见时长（秒）

    [Header("Audio")]
    public AudioSource audioSource;    // 可留空，运行时自动挂
    public AudioClip successClip;      // 完成一个词时播放

    [Header("Word List (按顺序)")]
    [TextArea(3, 10)]
    public string customWords =
        "apple\nriver\nunity\ngame\nsystem\nkeyboard\ncamera\npixel\nshader\nvector\n"
        + "banana\nmountain\nriverbank\nnotebook\nlamp\npencil\nocean\nforest\ncastle\nbridge\n"
        + "random\nspeed\ncloud\nstorm\ndesert\nisland\njourney\ndragon\nsword\nshield\n"
        + "wizard\nmagic\nquest\nadventure\nbattle\nvictory\nkingdom\nvillage\ntravel\nhero";
    private readonly List<string> words = new();
    private int wordIndex = 0;
    private string currentWord = "";
    private int cursor = 0;              // 已正确输入的字符数
    private int solved = 0;
    private bool playing = true;
    private float timeLeft;
    private float revealTimer = 0f;      // 剩余可见时间
    private bool targetHidden = false;

    public string failSceneName; // 在Inspector中填写失败时跳转的场景名
    public string winSceneName;  // 新增：通关后跳转的场景名

    void Awake()
    {
        if (!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
            if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void Start()
    {
        foreach (var line in customWords.Split('\n'))
        {
            var w = line.Trim();
            if (!string.IsNullOrEmpty(w)) words.Add(w);
        }
        if (words.Count == 0) words.Add("hello");

        if (goalCount <= 0 || goalCount > words.Count) goalCount = words.Count;

        timeLeft = totalTimeLimit > 0 ? totalTimeLimit : 0f;

        wordIndex = 0;
        LoadWord();           // 会显示 target，并清空 progress
        UpdateHUD();
    }

    void Update()
    {
        if (!playing)
        {
            return;
        }

        // 总时长
        if (totalTimeLimit > 0f)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0f)
            {
                timeLeft = 0f;
                GameOver(false);
                return;
            }
        }

        // 目标词可见计时
        if (!targetHidden)
        {
            revealTimer -= Time.deltaTime;
            if (revealTimer <= 0f)
            {
                targetHidden = true;
                if (TargetText) TargetText.text = ""; // 到时隐藏
            }
        }

        // —— IME 无关输入 —— //
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (cursor > 0) cursor--;
            RenderProgress();
        }

        bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // A..Z
        for (KeyCode k = KeyCode.A; k <= KeyCode.Z; k++)
        {
            if (Input.GetKeyDown(k))
            {
                char ch = (char)('a' + (k - KeyCode.A));
                if (caseSensitive && shift) ch = char.ToUpperInvariant(ch);
                HandleChar(ch);
                if (!playing) return;
            }
        }
        // 0..9
        for (KeyCode k = KeyCode.Alpha0; k <= KeyCode.Alpha9; k++)
        {
            if (Input.GetKeyDown(k))
            {
                char ch = (char)('0' + (k - KeyCode.Alpha0));
                HandleChar(ch);
                if (!playing) return;
            }
        }
        // 常用符号（按需增减）
        if (Input.GetKeyDown(KeyCode.Space)) HandleChar(' ');
        if (Input.GetKeyDown(KeyCode.Minus)) HandleChar('-');
        if (Input.GetKeyDown(KeyCode.Period)) HandleChar('.');
        if (Input.GetKeyDown(KeyCode.Comma)) HandleChar(',');
        if (Input.GetKeyDown(KeyCode.Quote)) HandleChar('\'');
        if (Input.GetKeyDown(KeyCode.Semicolon)) HandleChar(';');
        if (Input.GetKeyDown(KeyCode.Slash)) HandleChar('/');

        UpdateHUD();
    }

    void HandleChar(char raw)
    {
        char c = caseSensitive ? raw : char.ToLowerInvariant(raw);
        string target = caseSensitive ? currentWord : currentWord.ToLowerInvariant();

        if (cursor < target.Length && c == target[cursor])
        {
            cursor++;
            RenderProgress(); // 只回显“已输入”部分

            if (cursor >= target.Length)
            {
                solved++;
                PlaySuccess();
                wordIndex++;

                if (solved >= goalCount || wordIndex >= words.Count)
                {
                    GameOver(true);
                }
                else
                {
                    LoadWord();
                }
            }
        }
        else
        {
            // 错误不提示；Progress 仍只显示已输入部分
            RenderProgress();
        }
    }

    void LoadWord()
    {
        currentWord = words[wordIndex];
        cursor = 0;

        // 目标词显示 X 秒
        targetHidden = false;
        revealTimer = Mathf.Max(0.01f, revealDuration);
        if (TargetText) TargetText.text = currentWord;

        // 直到你开始打字前，Progress 为空
        if (ProgressText) ProgressText.text = "";

        UpdateHUD();
    }

    void RenderProgress()
    {
        if (!ProgressText) return;
        ProgressText.text = cursor > 0 ? currentWord.Substring(0, cursor) : "";
    }

    void UpdateHUD()
    {
        if (!HudText) return;
        if (totalTimeLimit > 0f) HudText.text = $"{solved}/{goalCount}   Time: {timeLeft:0.0}s";
        else HudText.text = $"{solved}/{goalCount}";
    }

    void PlaySuccess()
    {
        if (audioSource && successClip) audioSource.PlayOneShot(successClip);
    }

    void GameOver(bool win)
    {
        playing = false;
        if (TargetText) TargetText.text = win ? "通关！" : "时间到！";
        if (ProgressText && win) ProgressText.text = "";
        UpdateHUD();

        if (win && !string.IsNullOrEmpty(winSceneName))
        {
            SceneManager.LoadScene(winSceneName);
        }
        else if (!win && !string.IsNullOrEmpty(failSceneName))
        {
            SceneManager.LoadScene(failSceneName);
        }
    }

}