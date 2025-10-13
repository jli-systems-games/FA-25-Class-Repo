using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Image passengerPortrait;
    public TMP_Text timerText;
    public TMP_Text scoreText;
    public TMP_Text destinationText;
    public TMP_Text dateText;
    public TMP_Text stampText;
    public Button approveButton;
    public Button denyButton;
    public Button platformAButton;
    public Button platformBButton;
    public Button platformCButton;
    public GameObject platformRow;
    public TMP_Text hintText;
    public TMP_Text platformMapText;
    public GameObject referencePanel;
    public TMP_Text todayRefText;
    public TMP_Text validDestText;
    public TMP_Text stampRuleText;
    public TMP_Text wrongText;
    public float decisionTime = 5f;
    public Sprite[] passengerPortraits;
    public AudioSource sfx;
    public AudioClip sfxApprove;
    public AudioClip sfxDeny;
    public AudioClip sfxTick;
    public AudioClip sfxPaper;
    public AudioClip enterClip;
    public AudioClip assignOkClip;
    public AudioClip assignWrongClip;
    public string failSceneName = "GameOver";
    public float enterDuration = 0.6f;
    public float exitDuration = 0.55f;
    public GlitchEffect glitchEffect;

    private Passenger current;
    private string todayDate = "2005-10-10";
    private int score = 0;
    private int wrongCount = 0;
    private bool waitingPlatform = false;
    private bool locked = false;
    private RectTransform passengerRT;
    private CanvasGroup passengerCG;
    private Vector2 passengerHomePos;
    private Coroutine timerCo;

    void Start()
    {
        if (timerText) timerText.gameObject.SetActive(true);
        if (glitchEffect == null && Camera.main != null) glitchEffect = Camera.main.GetComponent<GlitchEffect>();
        PreparePassengerVisual();
        ResetRound();
        RefreshReferencePanel();
        UpdateWrongText();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) ToggleReferencePanel();
    }

    private void PreparePassengerVisual()
    {
        if (passengerPortrait == null) return;
        passengerRT = passengerPortrait.GetComponent<RectTransform>();
        if (passengerRT == null) passengerRT = passengerPortrait.gameObject.AddComponent<RectTransform>();
        passengerCG = passengerPortrait.GetComponent<CanvasGroup>();
        if (passengerCG == null) passengerCG = passengerPortrait.gameObject.AddComponent<CanvasGroup>();
        passengerHomePos = passengerRT.anchoredPosition;
    }

    private void ResetRound()
    {
        locked = false;
        waitingPlatform = false;
        if (platformRow != null) platformRow.SetActive(false);
        current = PassengerGenerator.Create(todayDate, passengerPortraits);
        if (passengerPortrait != null)
        {
            passengerPortrait.sprite = current.portrait;
            passengerPortrait.color = Color.white;
        }
        if (scoreText != null) scoreText.text = score.ToString();
        destinationText.text = "Destination: " + current.ticket.destination;
        dateText.text = "Date: " + current.ticket.date;
        stampText.text = "Stamp: " + (current.ticket.stampValid ? "Valid" : "Invalid");
        destinationText.color = Color.white;
        dateText.color = Color.white;
        stampText.color = Color.white;
        if (hintText != null) hintText.text = "Decide within " + Mathf.RoundToInt(decisionTime) + " seconds";
        RefreshReferencePanel();
        RefreshPlatformMapText();
        approveButton.interactable = true;
        denyButton.interactable = true;
        platformAButton.interactable = true;
        platformBButton.interactable = true;
        platformCButton.interactable = true;
        if (sfxPaper && sfx) sfx.PlayOneShot(sfxPaper);
        if (timerCo != null) StopCoroutine(timerCo);
        timerCo = StartCoroutine(RoundTimer(Mathf.RoundToInt(decisionTime)));
        StartCoroutine(EnterPassenger());
    }

    private IEnumerator RoundTimer(int seconds)
    {
        int left = Mathf.Max(0, seconds);
        UpdateTopLeftTimer(left);
        while (left > 0 && !locked)
        {
            yield return new WaitForSecondsRealtime(1f);
            left--;
            UpdateTopLeftTimer(left);
            if (sfxTick && sfx && left > 0) sfx.PlayOneShot(sfxTick);
        }
        if (!locked && left <= 0)
        {
            locked = true;
            OnTimeOut();
        }
    }

    private void UpdateTopLeftTimer(int sec)
    {
        if (timerText) timerText.text = sec.ToString("00");
    }

    private IEnumerator EnterPassenger()
    {
        if (passengerRT == null || passengerCG == null) yield break;
        Vector2 start = new Vector2(-Screen.width * 0.4f, passengerHomePos.y + 8f);
        Vector2 end = passengerHomePos;
        passengerRT.anchoredPosition = start;
        passengerCG.alpha = 0f;
        float t = 0f;
        if (enterClip && sfx) sfx.PlayOneShot(enterClip);
        while (t < enterDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / enterDuration);
            float bob = Mathf.Sin(k * Mathf.PI * 2f) * 4f * (1f - k);
            passengerRT.anchoredPosition = Vector2.Lerp(start, end, k) + new Vector2(0f, bob);
            passengerCG.alpha = Mathf.Lerp(0f, 1f, k);
            yield return null;
        }
        passengerRT.anchoredPosition = end;
        passengerCG.alpha = 1f;
    }

    private IEnumerator ExitPassenger(bool exitRight, bool tintRed)
    {
        if (passengerRT == null || passengerCG == null)
        {
            ResetOrGameOver();
            yield break;
        }
        if (tintRed && passengerPortrait) passengerPortrait.color = new Color(1f, 0.25f, 0.25f);
        Vector2 start = passengerRT.anchoredPosition;
        Vector2 dir = exitRight ? Vector2.right : Vector2.left;
        Vector2 end = start + dir * (Screen.width * 0.45f) + new Vector2(0f, -40f);
        float t = 0f;
        AudioClip clip = exitRight ? assignOkClip : assignWrongClip;
        if (sfx && clip) sfx.PlayOneShot(clip);
        while (t < exitDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Pow(Mathf.Clamp01(t / exitDuration), 0.8f);
            float fall = Mathf.Sin(k * Mathf.PI) * 10f;
            passengerRT.anchoredPosition = Vector2.Lerp(start, end, k) + new Vector2(0f, -fall);
            passengerCG.alpha = 1f - k * 0.95f;
            yield return null;
        }
        passengerCG.alpha = 0f;
        ResetOrGameOver();
    }

    private void ResetOrGameOver()
    {
        if (score <= -2 && !string.IsNullOrEmpty(failSceneName)) SceneManager.LoadScene(failSceneName);
        else ResetRound();
    }

    public void OnClickDeny()
    {
        if (locked) return;
        locked = true;
        bool shouldDeny = !current.isTicketValid;
        if (shouldDeny)
        {
            Succeed("Denied correctly", false);
            StartCoroutine(ExitPassenger(true, false));
        }
        else
        {
            Fail("Should not deny", false, true);
            StartCoroutine(ExitPassenger(false, true));
        }
    }

    public void OnClickApprove()
    {
        if (locked) return;
        if (!current.isTicketValid)
        {
            locked = true;
            Fail("Ticket invalid", false, true);
            StartCoroutine(ExitPassenger(false, true));
            return;
        }
        waitingPlatform = true;
        if (platformRow != null) platformRow.SetActive(true);
        if (hintText != null) hintText.text = "Approved. Choose platform A / B / C";
        if (sfxApprove && sfx) sfx.PlayOneShot(sfxApprove);
    }

    public void OnClickPlatform(string platformId)
    {
        if (!waitingPlatform || locked) return;
        string expected = platformId == "A" ? "Tokyo" : platformId == "B" ? "Berlin" : platformId == "C" ? "Paris" : null;
        if (string.IsNullOrEmpty(expected))
        {
            locked = true;
            Fail("Invalid platform", false, true);
            StartCoroutine(ExitPassenger(false, true));
            return;
        }
        bool match = string.Equals(current.ticket.destination, expected, System.StringComparison.OrdinalIgnoreCase);
        locked = true;
        if (match)
        {
            Succeed("Sent to Platform " + platformId, false);
            StartCoroutine(ExitPassenger(true, false));
        }
        else
        {
            Fail("Wrong platform", false, true);
            StartCoroutine(ExitPassenger(false, true));
        }
    }

    private void RefreshReferencePanel()
    {
        if (todayRefText) todayRefText.text = "Today: " + todayDate;
        if (validDestText) validDestText.text = "Valid Destinations:\n- Tokyo\n- Berlin\n- Paris";
        if (stampRuleText) stampRuleText.text = "Stamp Rule: must be \"Valid\"";
    }

    private void RefreshPlatformMapText()
    {
        if (platformMapText != null) platformMapText.text = "Platform A ¡ª Tokyo\nPlatform B ¡ª Berlin\nPlatform C ¡ª Paris";
    }

    public void ToggleReferencePanel()
    {
        if (!referencePanel) return;
        referencePanel.SetActive(!referencePanel.activeSelf);
    }

    private void Succeed(string msg, bool autoNext)
    {
        score++;
        if (scoreText != null) scoreText.text = score.ToString();
        if (hintText != null) hintText.text = msg;
        if (autoNext) ResetOrGameOver();
    }

    private void Fail(string msg, bool autoNext, bool countWrong)
    {
        if (countWrong)
        {
            wrongCount++;
            UpdateWrongText();
            if (glitchEffect != null) glitchEffect.Trigger(1f);
        }
        if (sfxDeny && sfx) sfx.PlayOneShot(sfxDeny);
        score--;
        if (scoreText != null) scoreText.text = score.ToString();
        if (hintText != null && msg != null) hintText.text = msg;
        if (autoNext) ResetOrGameOver();
    }

    private void UpdateWrongText()
    {
        if (wrongText != null) wrongText.text = "Wrong Operation: " + wrongCount;
    }

    private void OnTimeOut()
    {
        Fail("Time out", false, true);
        StartCoroutine(ExitPassenger(false, true));
    }
}
