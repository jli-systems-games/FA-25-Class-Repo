using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Crave3GameTMP : MonoBehaviour
{
    [Header("Title Page")]
    public GameObject titlePanel;
    public Image titleImage;
    public TextMeshProUGUI promptText;
    public Toggle easyModeToggle;

    [Header("Game Panel Root")]
    public GameObject gamePanel;

    [Header("UI References")]
    public Slider mentalBar;
    public Slider physicalBar;
    public Slider cravingBar;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI goalText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI eventText;
    public TextMeshProUGUI smokeCountText;
    public Button smokeBtn;
    public Button gumBtn;
    public Button breatheBtn;
    public TextMeshProUGUI smokeBtnLabel;
    public TextMeshProUGUI gumBtnLabel;
    public TextMeshProUGUI breatheBtnLabel;

    [Header("Bar Labels")]
    public TextMeshProUGUI mentalLabel;
    public TextMeshProUGUI physicalLabel;
    public TextMeshProUGUI cravingLabel;

    [Header("Overlays")]
    public Image redOverlay;
    public Image blueOverlay;

    [Header("Restart")]
    public Button restartBtn;

    [Header("Goals Panel")]
    public GameObject goalsPanel;
    public TextMeshProUGUI goalsText;
    public Button closeGoalsBtn;
    public Button viewGoalsBtn;
    public TextMeshProUGUI goalsHintText;

    [Header("Easy Mode")]
    public bool simpleWin = false;
    private bool easyUnlocked = false;

    [Header("Dev")]
    public bool resetEasyUnlockOnStart = false;

    [Header("Game Timing")]
    public float totalTime = 240f;

    [Header("Initial Needs")]
    public float initMental = 70f;
    public float initPhysical = 70f;
    public float initCraving = 30f;

    [Header("Decay per 10s")]
    public int mentalDecayPer10s = -2;
    public int physicalDecayPer10s = -1;
    public int cravingDecayPer10s = +3;
    public int cravingDecayPer10sEvent = +10;

    [Header("Actions & Cooldowns")]
    public float smokeCD = 60f;
    public float gumCD = 25f;
    public float breatheCD = 18f;

    private float timeLeft;
    private readonly float[] eventSchedule = new float[] { 30f, 90f, 150f, 210f };
    private int nextEventIndex = 0;
    private bool inEvent = false;
    private float eventEndAt = -1f;
    private bool eventResolved = false;
    private bool usedSmokeThisEvent = false;
    private float tenSecAccumulator = 0f;

    [Range(0, 100)] public float mental;
    [Range(0, 100)] public float physical;
    [Range(0, 100)] public float craving;

    private float smokeCDLeft = 0f, gumCDLeft = 0f, breatheCDLeft = 0f;
    private Queue<float> reboundQueue = new Queue<float>();
    private int healthyResolves = 0;
    private int smokeTotal = 0;
    private readonly List<float> smokeTimestamps = new List<float>();
    private bool gameOver = false;
    private bool gameStarted = false;

    private bool breathingActive = false;
    private int breathStep = 0;
    private bool tappedThisWindow = false;
    private Coroutine breatheCo;
    private Coroutine pulseCo;

    public CharacterSpriteSwitcher spriteSwitcher;

    private const string KEY_EASY_UNLOCKED = "EasyUnlocked";
    private const string KEY_EASY_WANTED = "EasyWanted";

    void Awake()
    {
        if (resetEasyUnlockOnStart)
        {
            PlayerPrefs.DeleteKey(KEY_EASY_UNLOCKED);
            PlayerPrefs.DeleteKey(KEY_EASY_WANTED);
            PlayerPrefs.Save();
        }
        if (redOverlay) redOverlay.color = new Color(1, 0, 0, 0);
        if (blueOverlay) blueOverlay.color = new Color(0, 0.6f, 1f, 0);

        if (goalsPanel) goalsPanel.SetActive(false);
        if (goalsHintText) goalsHintText.gameObject.SetActive(false);
        if (viewGoalsBtn)
        {
            viewGoalsBtn.gameObject.SetActive(false);
            viewGoalsBtn.onClick.RemoveAllListeners();
            viewGoalsBtn.onClick.AddListener(() => ToggleGoals(true));
        }
        if (closeGoalsBtn)
        {
            closeGoalsBtn.onClick.RemoveAllListeners();
            closeGoalsBtn.onClick.AddListener(() => ToggleGoals(false));
        }

        ShowTitleScreen();
        MusicManager.I?.PlayTitleBgm();
    }

    void Start()
    {
        if (smokeBtnLabel) smokeBtnLabel.text = "Smoke";
        if (gumBtnLabel) gumBtnLabel.text = "Gum";
        if (breatheBtnLabel) breatheBtnLabel.text = "Breathe";
        if (promptText) promptText.text = "Press SPACE to start";
        UpdateGoalText();
        PlayerPrefs.DeleteKey(KEY_EASY_WANTED);
    }

    void Update()
    {
        if (!gameStarted && titlePanel && titlePanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
                OnTitlePressed();
            return;
        }

        if (gameOver && goalsPanel && !titlePanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.G)) ToggleGoals(!goalsPanel.activeSelf);
        }

        if (!gameStarted || gameOver) return;

        float dt = Time.deltaTime;
        timeLeft -= dt;
        float elapsed = totalTime - Mathf.Max(timeLeft, 0f);

        if (smokeCDLeft > 0) smokeCDLeft -= dt;
        if (gumCDLeft > 0) gumCDLeft -= dt;
        if (breatheCDLeft > 0) breatheCDLeft -= dt;

        if (reboundQueue.Count > 0 && elapsed >= reboundQueue.Peek())
        {
            reboundQueue.Dequeue();
            AddCraving(+15f);
            StartCoroutine(FlashImage(redOverlay, new Color(1, 0, 0, 0.25f), 0.25f));
            MusicManager.I?.PlayRebound();
        }

        if (!inEvent && nextEventIndex < eventSchedule.Length && elapsed >= eventSchedule[nextEventIndex])
        {
            inEvent = true;
            eventResolved = false;
            usedSmokeThisEvent = false;
            eventEndAt = elapsed + (simpleWin ? 14f : 12f);
            nextEventIndex++;
            craving = Mathf.Max(craving, simpleWin ? 70f : 75f);
            if (eventText) eventText.text = simpleWin ? "Spike! Use Gum/Breathe (goal ≤40 once)" : "Craving Spike! Use Gum / Breathe first.";
            if (pulseCo != null) { StopCoroutine(pulseCo); }
            pulseCo = StartCoroutine(PulseOverlay(true));
            MusicManager.I?.PlaySpike();
        }

        if (inEvent && elapsed >= eventEndAt)
        {
            inEvent = false;
            if (eventText) eventText.text = "";
            if (pulseCo != null) { StopCoroutine(pulseCo); pulseCo = null; }
            if (redOverlay) redOverlay.color = new Color(1, 0, 0, 0);
            if (!eventResolved && craving > (simpleWin ? 95f : 80f))
            {
                Fail("Crashed during spike: Craving too high");
            }
        }

        tenSecAccumulator += dt;
        if (tenSecAccumulator >= 10f)
        {
            tenSecAccumulator -= 10f;
            AddMental(mentalDecayPer10s);
            AddPhysical(physicalDecayPer10s);
            AddCraving(inEvent ? cravingDecayPer10sEvent : cravingDecayPer10s);
        }

        if (inEvent && !eventResolved && craving <= 40f)
        {
            eventResolved = true;
            if (!usedSmokeThisEvent) healthyResolves++;
            if (statusText) { statusText.text = "Resolved"; StartCoroutine(ClearStatusSoon()); }
        }

        if (physical < (simpleWin ? 25f : 30f)) { Fail("Body can't handle it"); }

        if (timeLeft <= 0f)
        {
            if (simpleWin)
            {
                bool goalA = (smokeTotal == 0);
                bool goalB = (healthyResolves >= 1);
                bool goalC = (smokeTotal <= 1 && physical >= 40f);
                if (goalA || goalB || goalC)
                    Win(goalA ? "No Smoking" : goalB ? "Beat 1 Spike" : "≤1 Smoke & Phys ≥40");
                else
                    Fail("Goal not reached");
            }
            else
            {
                bool goalA = (smokeTotal == 0) && mental >= 60f && physical >= 55f;
                bool goalB = (healthyResolves >= 3) && (smokeTotal <= 1) && (physical >= 45f);
                if (goalA || goalB) Win(goalA ? "Smoke-Free Clear" : "Healthy-Resolve Clear");
                else Fail("Goal not reached");
            }
        }

        UpdateUI();

        if (breathingActive && breatheCo == null)
        {
            breathingActive = false;
            SetButtonsInteractable(true, true, true);
        }

        if (spriteSwitcher != null)
            spriteSwitcher.SetLoopPoseByValues(craving, physical, inEvent);
    }

    public void OnTitlePressed()
    {
        if (gameStarted) return;
        if (easyModeToggle && easyModeToggle.gameObject.activeSelf)
            simpleWin = easyModeToggle.isOn;
        ApplyDifficultyTuning();
        if (titlePanel) titlePanel.SetActive(false);
        if (gamePanel) gamePanel.SetActive(true);
        ResetGame();
        gameStarted = true;
        MusicManager.I?.PlayGameBgm();
    }

    void ShowTitleScreen()
    {
        if (gamePanel) gamePanel.SetActive(false);
        if (titlePanel) titlePanel.SetActive(true);

        easyUnlocked = PlayerPrefs.GetInt(KEY_EASY_UNLOCKED, 0) == 1;
        simpleWin = PlayerPrefs.GetInt(KEY_EASY_WANTED, 0) == 1;

        if (easyModeToggle)
        {
            easyModeToggle.gameObject.SetActive(easyUnlocked);
            easyModeToggle.isOn = simpleWin;
            easyModeToggle.transform.SetAsLastSibling();
            easyModeToggle.onValueChanged.RemoveAllListeners();
            easyModeToggle.onValueChanged.AddListener(OnEasyModeToggled);
        }

        if (goalsPanel) goalsPanel.SetActive(false);
        if (goalsHintText) goalsHintText.gameObject.SetActive(false);
        if (viewGoalsBtn) viewGoalsBtn.gameObject.SetActive(false);

        ConfigureStartPrompt();
    }

    void ConfigureStartPrompt()
    {
        bool showToggle = easyModeToggle && easyModeToggle.gameObject.activeSelf;
        if (promptText)
            promptText.text = showToggle ? "Try! Toggle Easy then press SPACE to start" : "press SPACE to start";
    }

    void ResetGame()
    {
        timeLeft = totalTime;
        nextEventIndex = 0;
        inEvent = false;
        eventResolved = false;
        usedSmokeThisEvent = false;
        tenSecAccumulator = 0f;

        mental = initMental;
        physical = initPhysical;
        craving = initCraving;

        smokeCDLeft = gumCDLeft = breatheCDLeft = 0f;

        reboundQueue.Clear();
        healthyResolves = 0;
        smokeTotal = 0;
        smokeTimestamps.Clear();
        gameOver = false;

        breathingActive = false;
        tappedThisWindow = false;
        if (pulseCo != null) { StopCoroutine(pulseCo); pulseCo = null; }
        if (redOverlay) redOverlay.color = new Color(1, 0, 0, 0);
        if (blueOverlay) blueOverlay.color = new Color(0, 0.6f, 1f, 0);

        if (restartBtn) restartBtn.gameObject.SetActive(false);

        if (statusText) statusText.text = "";
        if (eventText) eventText.text = "";
        UpdateGoalText();

        if (smokeBtnLabel) smokeBtnLabel.text = "Smoke";
        if (gumBtnLabel) gumBtnLabel.text = "Gum";
        if (breatheBtnLabel) breatheBtnLabel.text = "Breathe";

        if (goalsPanel) goalsPanel.SetActive(false);
        if (goalsHintText) goalsHintText.gameObject.SetActive(false);
        if (viewGoalsBtn) viewGoalsBtn.gameObject.SetActive(false);

        SetButtonsInteractable(true, true, true);
        if (spriteSwitcher != null) spriteSwitcher.UnlockPose();

        UpdateUI();
    }

    void ApplyDifficultyTuning()
    {
        if (simpleWin)
        {
            mentalDecayPer10s = -1;
            physicalDecayPer10s = 0;
            cravingDecayPer10s = +2;
            cravingDecayPer10sEvent = +8;
            gumCD = 20f;
            breatheCD = 14f;
        }
        else
        {
            mentalDecayPer10s = -2;
            physicalDecayPer10s = -1;
            cravingDecayPer10s = +3;
            cravingDecayPer10sEvent = +10;
            gumCD = 25f;
            breatheCD = 18f;
        }
    }

    public void OnSmoke()
    {
        if (!gameStarted || gameOver) return;
        if (smokeCDLeft > 0f) return;
        if (physical <= 35f) { if (statusText) { statusText.text = "Too low Physical to smoke"; StartCoroutine(ClearStatusSoon()); } return; }

        AddMental(+30f);
        AddCraving(-40f);
        AddPhysical(-25f);

        smokeTotal++;
        float elapsed = totalTime - Mathf.Max(timeLeft, 0f);
        smokeTimestamps.Add(elapsed);
        if (inEvent) usedSmokeThisEvent = true;
        smokeCDLeft = smokeCD;

        reboundQueue.Enqueue(elapsed + 30f);
        StartCoroutine(FlashImage(redOverlay, new Color(1, 1, 1, 0.35f), 0.35f));
        MusicManager.I?.PlayClick();
        if (spriteSwitcher != null) spriteSwitcher.PlayOneShot("Smoking", 0.9f);
    }

    public void OnGum()
    {
        if (!gameStarted || gameOver) return;
        if (gumCDLeft > 0f) return;

        float craveDelta = simpleWin ? -30f : -25f;
        float mentalDelta = simpleWin ? +10f : +8f;
        float physDelta = simpleWin ? -1f : -2f;

        AddCraving(craveDelta);
        AddMental(mentalDelta);
        AddPhysical(physDelta);
        gumCDLeft = gumCD = simpleWin ? 20f : 25f;

        MusicManager.I?.PlayGum();
        if (spriteSwitcher != null) spriteSwitcher.PlayOneShot("ChewGum", 0.7f);
    }

    public void OnBreathePress()
    {
        if (!gameStarted || gameOver) return;
        if (breatheCDLeft > 0f && !breathingActive) return;

        if (!breathingActive)
        {
            breatheCo = StartCoroutine(BreatheRoutine());
            MusicManager.I?.PlayClick();
        }
        else
        {
            tappedThisWindow = true;
        }
    }

    IEnumerator BreatheRoutine()
    {
        breathingActive = true;
        breathStep = 0;
        tappedThisWindow = false;

        SetButtonsInteractable(false, false, true);
        string oldLabel = breatheBtnLabel ? breatheBtnLabel.text : "Breathe";

        if (statusText) statusText.text = "Breathing: TAP with the rhythm x3";
        if (breatheBtnLabel) breatheBtnLabel.text = "TAP";
        if (eventText) eventText.text = "Inhale... exhale... (tap with the pulse)";

        int success = 0;
        for (breathStep = 0; breathStep < 3; breathStep++)
        {
            tappedThisWindow = false;
            yield return StartCoroutine(BreathePulse(0.4f, 0.8f));
            if (tappedThisWindow) success++;
        }

        if (success == 3)
        {
            AddCraving(simpleWin ? -25f : -20f);
            AddMental(simpleWin ? +15f : +12f);
            if (statusText) statusText.text = "Breathing success";
            MusicManager.I?.PlayBreatheSuccess();
        }
        else
        {
            AddCraving(simpleWin ? -15f : -10f);
            AddMental(simpleWin ? +7f : +5f);
            if (statusText) statusText.text = $"Breathing partial ({success}/3)";
            MusicManager.I?.PlayBreathePartial();
        }
        StartCoroutine(ClearStatusSoon());

        if (breatheBtnLabel) breatheBtnLabel.text = oldLabel;
        if (eventText) eventText.text = "";
        SetButtonsInteractable(true, true, true);
        breathingActive = false;
        breatheCDLeft = breatheCD = simpleWin ? 14f : 18f;
        breatheCo = null;

        if (spriteSwitcher != null) spriteSwitcher.PlayOneShot("Breathe", 0.8f);
    }

    IEnumerator BreathePulse(float prepare, float tapWindow)
    {
        if (blueOverlay) blueOverlay.color = new Color(0, 0.6f, 1f, 0.15f);
        yield return new WaitForSeconds(prepare);

        if (blueOverlay) blueOverlay.color = new Color(0, 0.6f, 1f, 0.35f);
        float t = 0f;
        bool counted = false;
        while (t < tapWindow)
        {
            t += Time.deltaTime;
            if (tappedThisWindow && !counted) { counted = true; }
            yield return null;
        }
        if (blueOverlay) blueOverlay.color = new Color(0, 0.6f, 1f, 0.0f);
    }

    void Win(string msg)
    {
        if (gameOver) return;
        gameOver = true;
        UnlockEasyIfNeeded();
        if (statusText) statusText.text = "Victory! " + msg;
        if (eventText) eventText.text = $"Healthy resolves: {healthyResolves}   Smokes: {smokeTotal}";
        SetButtonsInteractable(false, false, false);
        StartCoroutine(FlashImage(redOverlay, new Color(0, 1f, 0.4f, 0.25f), 0.6f));
        if (spriteSwitcher != null) spriteSwitcher.LockPose("Win");
        if (restartBtn) restartBtn.gameObject.SetActive(true);
        ShowGoalsEntryOnResult();
    }

    void Fail(string reason)
    {
        if (gameOver) return;
        gameOver = true;
        UnlockEasyIfNeeded();
        if (statusText) statusText.text = "Failed... " + reason;
        if (eventText) eventText.text = $"Healthy resolves: {healthyResolves}   Smokes: {smokeTotal}";
        SetButtonsInteractable(false, false, false);
        StartCoroutine(FlashImage(redOverlay, new Color(1, 0, 0, 0.35f), 0.8f));
        if (spriteSwitcher != null) spriteSwitcher.LockPose("Fail");
        if (restartBtn) restartBtn.gameObject.SetActive(true);
        ShowGoalsEntryOnResult();
    }

    void ShowGoalsEntryOnResult()
    {
        if (viewGoalsBtn) viewGoalsBtn.gameObject.SetActive(true);
        if (goalsHintText)
        {
            goalsHintText.text = "View Goals";
            goalsHintText.gameObject.SetActive(true);
        }
        PrepareGoalsText();
    }

    void UnlockEasyIfNeeded()
    {
        if (!easyUnlocked)
        {
            easyUnlocked = true;
            PlayerPrefs.SetInt(KEY_EASY_UNLOCKED, 1);
            PlayerPrefs.Save();
        }
    }

    public void OnRestart()
    {
        PlayerPrefs.SetInt(KEY_EASY_WANTED, simpleWin ? 1 : 0);
        PlayerPrefs.Save();
        gameStarted = false;
        ShowTitleScreen();
        MusicManager.I?.PlayTitleBgm();
    }

    void OnEasyModeToggled(bool on)
    {
        simpleWin = on;
        ApplyDifficultyTuning();
        UpdateGoalText();
        ConfigureStartPrompt();
    }

    void UpdateGoalText()
    {
        if (!goalText) return;
        goalText.text = simpleWin
            ? "Goal:" +
            " No Smoking OR Beat 1 Spike OR ≤1 Smoke & Phys≥40"
            : "Goal: " +
            "Smoke-Free OR Healthy-Resolve";
    }

    void ToggleGoals(bool show)
    {
        if (!goalsPanel) return;
        goalsPanel.SetActive(show);
        if (show) PrepareGoalsText();
    }

    void PrepareGoalsText()
    {
        if (!goalsText) return;
        string normal =
            "NORMAL MODE\n" +
            "• Clear A: Smoke-Free + Mental ≥ 60 + Physical ≥ 55\n" +
            "• Clear B: Healthy Resolves ≥ 3 AND Smoke ≤ 1 AND Physical ≥ 45\n";
        string easy =
            "EASY MODE\n" +
            "• Clear A: No Smoking\n" +
            "• Clear B: Beat ≥ 1 Craving Spike (reduce Craving ≤ 40 during a spike)\n" +
            "• Clear C: Smoke ≤ 1 AND Physical ≥ 40\n";
        goalsText.text = normal + "\n" + easy;
    }

    void UpdateUI()
    {
        if (mentalBar) mentalBar.value = mental / 100f;
        if (physicalBar) physicalBar.value = physical / 100f;
        if (cravingBar) cravingBar.value = craving / 100f;

        if (timerText) timerText.text = FormatTime(Mathf.Max(timeLeft, 0f));
        if (smokeCountText) smokeCountText.text = $"Smoke: {smokeTotal}";

        int m = Mathf.RoundToInt(mental);
        int p = Mathf.RoundToInt(physical);
        int c = Mathf.RoundToInt(craving);
        if (mentalLabel) mentalLabel.text = $"Mental   {m}";
        if (physicalLabel) physicalLabel.text = $"Physical {p}";
        if (cravingLabel) cravingLabel.text = $"Craving  {c}";
        if (cravingLabel) cravingLabel.color = (craving >= 75f ? new Color(1f, 0.45f, 0.45f) : Color.white);
        if (physicalLabel) physicalLabel.color = (physical < 35f ? new Color(1f, 0.65f, 0.65f) : Color.white);

        bool canSmokeByHP = physical > 35f;
        if (smokeBtn) smokeBtn.interactable = !gameOver && (smokeCDLeft <= 0f) && canSmokeByHP && !breathingActive;
        if (gumBtn) gumBtn.interactable = !gameOver && (gumCDLeft <= 0f) && !breathingActive;
        if (breatheBtn) breatheBtn.interactable = !gameOver && (breatheCDLeft <= 0f || breathingActive);

        if (smokeBtnLabel) smokeBtnLabel.text = (smokeCDLeft > 0) ? $"Smoke ({smokeCDLeft:0}s)" : "Smoke";
        if (gumBtnLabel) gumBtnLabel.text = (gumCDLeft > 0) ? $"Gum ({gumCDLeft:0}s)" : "Gum";
        if (!breathingActive && breatheBtnLabel)
            breatheBtnLabel.text = (breatheCDLeft > 0) ? $"Breathe ({breatheCDLeft:0}s)" : "Breathe";
    }

    void SetButtonsInteractable(bool smoke, bool gum, bool breathe)
    {
        if (smokeBtn) smokeBtn.interactable = smoke && (smokeCDLeft <= 0f) && physical > 35f;
        if (gumBtn) gumBtn.interactable = gum && (gumCDLeft <= 0f);
        if (breatheBtn) breatheBtn.interactable = breathe && (breatheCDLeft <= 0f || breathingActive);
    }

    IEnumerator ClearStatusSoon(float sec = 1.2f)
    {
        yield return new WaitForSeconds(sec);
        if (!gameOver && statusText) statusText.text = "";
    }

    IEnumerator FlashImage(Image img, Color c, float dur)
    {
        if (img)
        {
            img.color = c;
            yield return new WaitForSeconds(dur);
            img.color = new Color(c.r, c.g, c.b, 0f);
        }
        else yield return null;
    }

    IEnumerator PulseOverlay(bool start)
    {
        if (!start || redOverlay == null) yield break;
        while (true)
        {
            for (float a = 0f; a <= 0.35f; a += Time.deltaTime * 1.8f)
            {
                redOverlay.color = new Color(1, 0, 0, a);
                yield return null;
            }
            for (float a = 0.35f; a >= 0f; a -= Time.deltaTime * 1.8f)
            {
                redOverlay.color = new Color(1, 0, 0, a);
                yield return null;
            }
        }
    }

    void AddMental(float v) { mental = Mathf.Clamp(mental + v, 0f, 100f); }
    void AddPhysical(float v) { physical = Mathf.Clamp(physical + v, 0f, 100f); }
    void AddCraving(float v) { craving = Mathf.Clamp(craving + v, 0f, 100f); }

    string FormatTime(float t)
    {
        int m = Mathf.FloorToInt(t / 60f);
        int s = Mathf.FloorToInt(t % 60f);
        return $"{m:00}:{s:00}";
    }
}
