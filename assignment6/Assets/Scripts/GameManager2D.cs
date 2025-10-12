using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager2D : MonoBehaviour
{
    public static GameManager2D I;

    [Header("HUD")]
    public TMP_Text scoreText;    
    public TMP_Text speedText;   
    public TMP_Text tipText;  

    [Header("Stability UI (optional)")]
    public Slider stabilityBar;    
    public Image stabilityFill;   
    public Color stableColor = new Color(0.4f, 1f, 0.5f, 1f);
    public Color dangerColor = new Color(1f, 0.5f, 0.4f, 1f);

    [Header("Start Panel")]
    public GameObject startPanel;    
    public Image coverImage;     
    public TMP_Text startTitleText; 
    public TMP_Text pressText;   
    [Tooltip("Accept mouse/touch to start as well as SPACE")]
    public bool allowMouseStart = true;
    [Tooltip("Title shown on start panel")]
    public string gameTitle = "Rhythm Rails";
    [Tooltip("Blink speed of 'Press SPACE to Start'")]
    public float pressBlinkSpeed = 2.2f;

    [Header("End Panel")]
    public GameObject endPanel;       
    public TMP_Text titleText;  
    public TMP_Text finalScoreText;   

    [Header("Refs")]
    public Train2DController_Pos train;
    public TrackGenerator2D_Pos generator;   

    [Header("Game Flow")]
    public bool pauseOnGameOver = true;      
    public bool disableGeneratorOnGameOver = true;

    [Header("Tips")]
    public float tipInterval = 10f;    
    float _tipTimer;

    static readonly string[] FUN_TIPS_EN = {
      "Tap SPACE to nudge the throttle. Mashing keeps the train alive!",
      "Too slow drains stability, too fast drains it too. Find the sweet spot.",
      "Near zero stability? Mash SPACE now or never!",
      "Don’t hold—tap! SPACE, SPACE, SPACE!",
     // "Junctions switch with a click/tap. Let SPACE handle speed only.",
     // "Obstacles are instant breakup. Keep it steady and dodge.",
     // "Green zone speed = comfy ride. Red zone = risky ride.",
    //  "If the bar turns red, ease up the SPACE taps.",
      "Stall window active: you have a second—hammer SPACE!",
      "Every SPACE tap adds a burst. Don’t waste the rhythm.",
      "You’re driving a train, not a rocket. Tap SPACE, don’t launch.",
      "Speed isn’t glory—survival is. Tap SPACE to stay smooth.",
      "SPACE taps work better in rhythm. Find your beat.",
      //"Cargo gives points. No points if you crash into obstacles.",
      "Almost zero? Don’t panic. Rapid SPACE can still save you.",
      "The track won’t wait—be decisive with SPACE.",
      //"Stability under 0.35 is danger zone. Tap SPACE harder.",
      "Short bursts > long holds. SPACE is a drum, not a lever.",
     // "Click to switch at junctions; SPACE controls speed only.",
      "If you overshoot speed, tap slower—let drag do the work.",
      "Don’t chase max speed. Chase the green zone.",
      "If it wiggles, it lives. Keep tapping SPACE to keep it alive!",
      "SPACE taps pause some drain for a moment—use that window.",
      "Calm mind, fast fingers: SPACE is your heartbeat."
    };

    [Header("Score pulse")]
    [SerializeField] float pulseScale = 1.08f;     
    [SerializeField] float pulseDuration = 0.12f;  
    [SerializeField] float pulseMax = 1.20f;  
    Coroutine _pulseCo;
    Vector3 _scoreBaseScale = Vector3.one;

    int _score = 0;
    bool _isGameOver = false;
    bool _hasStarted = false; 

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;

        if (!generator) generator = FindObjectOfType<TrackGenerator2D_Pos>();
        if (!train) train = FindObjectOfType<Train2DController_Pos>();

        if (endPanel) endPanel.SetActive(false);

        if (scoreText) _scoreBaseScale = scoreText.rectTransform.localScale;

        UpdateScoreUI();

        if (startPanel) startPanel.SetActive(true);
        if (startTitleText) startTitleText.text = string.IsNullOrEmpty(gameTitle) ? "Rhythm Rails" : gameTitle;
        if (pressText) pressText.text = "Press SPACE to Start";

        Time.timeScale = 0f;
        if (generator) generator.enabled = false;
        _hasStarted = false;

        if (FUN_TIPS_EN.Length > 0) ShowTip(FUN_TIPS_EN[0]);
    }

    void Update()
    {
        if (!_hasStarted)
        {
            if (pressText)
            {
                float a = 0.6f + 0.4f * Mathf.Abs(Mathf.Sin(Time.unscaledTime * pressBlinkSpeed));
                var c = pressText.color; c.a = a; pressText.color = c;
            }

            if (Input.GetKeyDown(KeyCode.Space) ||
               (allowMouseStart && (Input.GetMouseButtonDown(0) ||
                                   (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))))
            {
                StartGame();
            }
            return; 
        }

        _tipTimer += Time.unscaledDeltaTime;
        if (_tipTimer >= tipInterval)
        {
            _tipTimer = 0f;
            if (FUN_TIPS_EN.Length > 0)
                ShowTip(FUN_TIPS_EN[Random.Range(0, FUN_TIPS_EN.Length)]);
        }
    }

    void StartGame()
    {
        _hasStarted = true;

        if (startPanel) startPanel.SetActive(false);
        if (generator) generator.enabled = true;

        Time.timeScale = 1f;

        ShowTip("Tap SPACE to keep speed in the green zone!");
    }

    public void AddScore(int s)
    {
        if (_isGameOver) return;
        _score += s;
        UpdateScoreUI();
        StartScorePulse();
    }

    public void SetSpeedAndStability(float speed, float stability01, bool inZone)
    {
        if (speedText) speedText.text = $"Speed: {speed:0.0}";
        if (stabilityBar)
        {
            stabilityBar.value = Mathf.Clamp01(stability01);
            if (stabilityFill)
                stabilityFill.color = inZone ? stableColor : dangerColor;
        }
    }

    public void GameOver()
    {
        if (_isGameOver) return;
        _isGameOver = true;

        if (disableGeneratorOnGameOver && generator) generator.enabled = false;
        if (pauseOnGameOver) Time.timeScale = 0f;

        if (endPanel) endPanel.SetActive(true);
        if (titleText) titleText.text = "Game Over";
        if (finalScoreText) finalScoreText.text = $"Score: {_score}";
        ShowTip("You crashed! Tap SPACE to keep speed in the green next time.");
    }

    public void Win()
    {
        if (_isGameOver) return;
        _isGameOver = true;

        if (disableGeneratorOnGameOver && generator) generator.enabled = false;
        if (pauseOnGameOver) Time.timeScale = 0f;

        if (endPanel) endPanel.SetActive(true);
        if (titleText) titleText.text = "You Win!";
        if (finalScoreText) finalScoreText.text = $"Score: {_score}";
        ShowTip("Smooth ride! Keep tapping SPACE to stay in the sweet spot.");
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    public void ShowTip(string msg)
    {
        if (!tipText) return;
        tipText.text = msg;
    }

    public void Tip_NearZero() => ShowTip("Near zero stability! Mash SPACE!");
    public void Tip_StallEnter() => ShowTip("Stall! You have a moment—hammer SPACE!");
    public void Tip_StallRecover() => ShowTip("Recovered! Keep tapping SPACE inside the green zone.");
    public void Tip_Obstacle() => ShowTip("Obstacle ahead—steady! Tap SPACE, don’t panic.");
    public void Tip_Cargo() => ShowTip("+1 score! Smooth driving = more cargo.");
    public void Tip_Junction() => ShowTip("Junction ahead: click to switch. SPACE is only for speed.");

    void UpdateScoreUI()
    {
        if (scoreText) scoreText.text = $"Score: {_score}";
    }

    void StartScorePulse()
    {
        if (!scoreText) return;

        if (_pulseCo != null)
        {
            StopCoroutine(_pulseCo);
            scoreText.rectTransform.localScale = _scoreBaseScale;
            _pulseCo = null;
        }
        _pulseCo = StartCoroutine(PulseTMP_Clamped(scoreText, pulseScale, pulseDuration, pulseMax));
    }

    System.Collections.IEnumerator PulseTMP_Clamped(TMP_Text t, float scale, float dur, float maxScale)
    {
        var rt = t.rectTransform;

        Vector3 from = _scoreBaseScale;
        Vector3 to = _scoreBaseScale * Mathf.Min(scale, maxScale);

        float t0 = 0f;
        while (t0 < dur)
        {
            t0 += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t0 / dur);
            float e = 1f - (1f - k) * (1f - k); 
            rt.localScale = Vector3.LerpUnclamped(from, to, e);
            yield return null;
        }

        rt.localScale = _scoreBaseScale;
        _pulseCo = null;
    }
}
