using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EarthquakePlayerController : MonoBehaviour
{
    public EarthquakeBaseMover earthquake;
    public CoreGoal core;

    public int maxQuakes;
    public TMP_Text quakesTMP;

    public GameObject p1WinPanel;
    public GameObject p2WinPanel;
    public GameObject restartButton;
    public AudioClip WinClip;

    private int remaining;
    private bool coreFailed;
    private bool gameOver;

    void Start()
    {
        if (!earthquake) earthquake = Object.FindFirstObjectByType<EarthquakeBaseMover>();
        StartCoroutine(WaitForCore());

        SetPanel(p1WinPanel, false);
        SetPanel(p2WinPanel, false);
        SetPanel(restartButton, false);
    }

    System.Collections.IEnumerator WaitForCore()
    {
        while (core == null)
        {
            core = Object.FindFirstObjectByType<CoreGoal>();
            if (core) break;
            yield return null;
        }
        core.onCoreFailed.AddListener(OnCoreFailed);
        remaining = Mathf.Max(0, maxQuakes);
        UpdateHUD();
    }

    void Update()
    {
        if (gameOver) return;

        if (Input.GetKeyDown(KeyCode.Space))
            TryShake();

        if (remaining == 0 && earthquake && !earthquake.IsShaking && !coreFailed)
            WinP1();
    }

    void TryShake()
    {
        if (gameOver) return;
        if (!earthquake) return;
        if (earthquake.IsShaking) return;
        if (remaining <= 0) return;

        remaining--;
        earthquake.TriggerOnce();
        UpdateHUD();
    }

    void OnCoreFailed()
    {
        if (gameOver) return;
        coreFailed = true;
        WinP2();
    }

    void WinP1()
    {
        if (gameOver) return;
        gameOver = true;
        if (WinClip) AudioSource.PlayClipAtPoint(WinClip, Camera.main.transform.position);
        SetPanel(p1WinPanel, true);
        SetPanel(p2WinPanel, false);
        SetPanel(restartButton, true);
    }

    void WinP2()
    {
        if (gameOver) return;
        gameOver = true;
        if (WinClip) AudioSource.PlayClipAtPoint(WinClip, Camera.main.transform.position);
        SetPanel(p1WinPanel, false);
        SetPanel(p2WinPanel, true);
        SetPanel(restartButton, true);
    }

    void UpdateHUD()
    {
        if (quakesTMP)
            quakesTMP.text = $"{remaining}";
    }

    void SetPanel(GameObject go, bool on)
    {
        if (go && go.activeSelf != on)
            go.SetActive(on);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    public void BackToBuild()
    {
        SceneManager.LoadScene(0);
    }
}