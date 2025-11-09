// BattleManager.cs
using UnityEngine;
using UnityEngine.UI;          // Image, etc.
using TMPro;                   // TMP_Text
using Quaternion = UnityEngine.Quaternion; // 모호성 방지
using Random = UnityEngine.Random;       // 모호성 방지

public class BattleManager : MonoBehaviour
{
    [Header("Roster (drag SOs)")]
    public AnimalStats panda;   // Panda SO
    public AnimalStats chick;   // Chick SO
    public AnimalStats rabbit;  // Rabbit SO
    public AnimalStats turtle;  // Turtle SO
    public AnimalStats cat;     // Cat SO

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

    // 내부 상태
    AnimalStats p1Pick, p2Pick;
    AnimalController p1, p2;
    float t;
    bool battling;

    // --- (선택) 같은 씬에서 선택 UI를 쓸 때 버튼에서 호출 ---
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
        // 선택 씬에서 넘어온 값이 있으면 자동 세팅
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

        // 선택값이 준비돼 있으면 즉시 전투 시작
        if (p1Pick != null && p2Pick != null)
            StartBattle();
    }

    // --- Start Battle 버튼에서 호출 ---
    public void StartBattle()
    {
        if (p1Pick == null || p2Pick == null)
        {
            Log("양쪽 캐릭터를 먼저 선택하세요.");
            return;
        }

        // 이전 객체 정리
        if (p1) Destroy(p1.gameObject);
        if (p2) Destroy(p2.gameObject);

        // UI 초기화
        if (resultText) resultText.text = "";
        if (logText) logText.text = "";

        battling = true;
        t = roundTime;

        // 선택된 SO가 들고있는 프리팹으로 스폰
        p1 = Instantiate(p1Pick.prefab, p1Spawn.position, Quaternion.identity);
        p1.stats = p1Pick;
        p1.OnLog += AddLog;

        p2 = Instantiate(p2Pick.prefab, p2Spawn.position, Quaternion.identity);
        p2.stats = p2Pick;
        p2.OnLog += AddLog;

        // 초반 충돌 유도 킥오프
        p1.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 4f, ForceMode2D.Impulse);
        p2.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 4f, ForceMode2D.Impulse);

        // HP바 초기화(안전)
        UpdateHPBars();
        UpdateTimerUI();
    }

    void Update()
    {
        if (!battling || !p1 || !p2) return;

        // 타이머
        t -= Time.deltaTime;
        UpdateTimerUI();

        // HP바
        UpdateHPBars();

        // 즉시 승패
        if (p1.HP <= 0 || p2.HP <= 0)
        {
            battling = false;
            EndImmediate();
            return;
        }

        // 타임업 승패
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

    void EndImmediate()
    {
        string winner = p1.HP > 0 ? p1.stats.displayName : p2.stats.displayName;
        string loser = (winner == p1.stats.displayName) ? p2.stats.displayName : p1.stats.displayName;

        if (resultText)
            resultText.text = $"Winner: {winner}\nLoser: {loser}";
    }

    void EndByHP()
    {
        string winner;
        if (p1.HP == p2.HP)
            winner = (Random.value < .5f) ? p1.stats.displayName : p2.stats.displayName;
        else
            winner = (p1.HP > p2.HP) ? p1.stats.displayName : p2.stats.displayName;

        string loser = (winner == p1.stats.displayName) ? p2.stats.displayName : p1.stats.displayName;

        if (resultText)
            resultText.text = $"(Time up)\nWinner: {winner}\nLoser: {loser}";
    }

    void AddLog(string m) => Log(m);

    void Log(string m)
    {
        if (!logText) return;
        // 새 메시지를 위로 쌓기
        logText.text = m + "\n" + logText.text;
    }
}
