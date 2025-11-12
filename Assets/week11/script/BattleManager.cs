using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;

public class BattleManager : MonoBehaviour
{
    Coroutine _logCo;
    [SerializeField] float logShowTime = 0.9f;
    [SerializeField] float logFadeTime = 0.6f;

    [Header("Roster (drag SOs)")]
    public AnimalStats panda;
    public AnimalStats chick;
    public AnimalStats rabbit;
    public AnimalStats turtle;
    public AnimalStats cat;

    [Header("Spawns")]
    public Transform p1Spawn;
    public Transform p2Spawn;

    [Header("UI - HP Bars (Image Type=Filled)")]
    public Image p1HPBar;
    public Image p2HPBar;

    [Header("UI - Portraits (optional)")]
    public Image p1Portrait;
    public Image p2Portrait;

    [Header("UI - TextMeshPro")]
    public TMP_Text timerText;
    public TMP_Text resultText;
    public TMP_Text logText;

    [Header("Round")]
    public float roundTime = 20f;

    AnimalStats p1Pick, p2Pick;
    AnimalController p1, p2;
    float t;
    bool battling;

    public void PickP1(string name)
    {
        p1Pick = NameToStats(name);
        if (p1Portrait && p1Pick) p1Portrait.sprite = p1Pick.sprite;
    }

    public void PickP2(string name)
    {
        p2Pick = NameToStats(name);
        if (p2Portrait && p2Pick) p2Portrait.sprite = p2Pick.sprite;
    }

    AnimalStats NameToStats(string n)
    {
        switch (n)
        {
            case "Panda": return panda;
            case "Chick": return chick;
            case "Rabbit": return rabbit;
            case "Turtle": return turtle;
            case "Cat": return cat;
        }
        return null;
    }

    void Start()
    {
        if (SelectionData.P1 != null)
        {
            p1Pick = SelectionData.P1;
            if (p1Portrait && p1Pick) p1Portrait.sprite = p1Pick.sprite;
        }
        if (SelectionData.P2 != null)
        {
            p2Pick = SelectionData.P2;
            if (p2Portrait && p2Pick) p2Portrait.sprite = p2Pick.sprite;
        }
        if (p1Pick != null && p2Pick != null)
            StartBattle();
    }

    public void StartBattle()
    {
        if (p1Pick == null || p2Pick == null) { Log("양쪽 캐릭터를 먼저 선택하세요."); return; }

        if (p1) Destroy(p1.gameObject);
        if (p2) Destroy(p2.gameObject);

        if (resultText) resultText.text = "";
        if (logText) logText.text = "";

        battling = true;
        t = roundTime;

        p1 = Instantiate(p1Pick.prefab, p1Spawn.position, Quaternion.identity);
        p1.Setup(p1Pick);
        p1.OnLog += AddLog;

        p2 = Instantiate(p2Pick.prefab, p2Spawn.position, Quaternion.identity);
        p2.Setup(p2Pick);
        p2.OnLog += AddLog;

        var rb1 = p1.GetComponent<Rigidbody2D>();
        var rb2 = p2.GetComponent<Rigidbody2D>();
        if (rb1) rb1.AddForce(Vector2.right * 4f, ForceMode2D.Impulse);
        if (rb2) rb2.AddForce(Vector2.left * 4f, ForceMode2D.Impulse);

        UpdateHPBars();
        UpdateTimerUI();
    }

    void Update()
    {
        if (!battling || !p1 || !p2) return;

        t -= Time.deltaTime;
        UpdateTimerUI();
        UpdateHPBars();

        if (p1.HP <= 0 || p2.HP <= 0)
        {
            battling = false;
            EndImmediate();
            return;
        }

        if (t <= 0f)
        {
            battling = false;
            EndByHP();
        }
    }

    void UpdateHPBars()
    {
        if (p1HPBar && p1 && p1.stats)
            p1HPBar.fillAmount = Mathf.Clamp01(p1.HP / (float)p1.stats.maxHP);

        if (p2HPBar && p2 && p2.stats)
            p2HPBar.fillAmount = Mathf.Clamp01(p2.HP / (float)p2.stats.maxHP);
    }

    void UpdateTimerUI()
    {
        if (timerText)
            timerText.text = Mathf.CeilToInt(Mathf.Max(0, t)).ToString();
    }

    void FreezeWinner(AnimalController a)
    {
        if (!a) return;
        var rb = a.GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.isKinematic = true;
        }
        a.enabled = false;
    }

    void DespawnLoser(AnimalController winner, AnimalController loser)
    {
        FreezeWinner(winner);
        if (loser)
        {
            if (loser == p1 && p1HPBar) p1HPBar.fillAmount = 0f;
            if (loser == p2 && p2HPBar) p2HPBar.fillAmount = 0f;
            Destroy(loser.gameObject);
        }
    }

    void EndImmediate()
    {
        string winnerName = p1.HP > 0 ? p1.stats.displayName : p2.stats.displayName;
        string loserName = (winnerName == p1.stats.displayName) ? p2.stats.displayName : p1.stats.displayName;

        if (resultText)
            resultText.text = $"Winner: {winnerName}\nLoser: {loserName}";

        var winnerCtrl = (winnerName == p1.stats.displayName) ? p1 : p2;
        var loserCtrl = (winnerCtrl == p1) ? p2 : p1;
        DespawnLoser(winnerCtrl, loserCtrl);
    }

    void EndByHP()
    {
        string winnerName;
        if (p1.HP == p2.HP)
            winnerName = (Random.value < .5f) ? p1.stats.displayName : p2.stats.displayName;
        else
            winnerName = (p1.HP > p2.HP) ? p1.stats.displayName : p2.stats.displayName;

        string loserName = (winnerName == p1.stats.displayName) ? p2.stats.displayName : p1.stats.displayName;

        if (resultText)
            resultText.text = $"(Time up)\nWinner: {winnerName}\nLoser: {loserName}";

        var winnerCtrl = (winnerName == p1.stats.displayName) ? p1 : p2;
        var loserCtrl = (winnerCtrl == p1) ? p2 : p1;
        DespawnLoser(winnerCtrl, loserCtrl);
    }

    void AddLog(string m) => Log(m);

    void Log(string m)
    {
        if (!logText) return;
        if (_logCo != null) StopCoroutine(_logCo);
        _logCo = StartCoroutine(CoShowLogOnce(m));
    }

    System.Collections.IEnumerator CoShowLogOnce(string m)
    {
        logText.text = m;
        var c = logText.color;
        c.a = 1f;
        logText.color = c;
        yield return new WaitForSeconds(logShowTime);
        float t = 0f;
        while (t < logFadeTime)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, t / logFadeTime);
            c.a = a;
            logText.color = c;
            yield return null;
        }
        logText.text = "";
        _logCo = null;
    }
}
