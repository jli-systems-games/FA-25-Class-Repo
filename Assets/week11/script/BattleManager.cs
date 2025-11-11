
using UnityEngine;
using UnityEngine.UI;          
using TMPro;                  
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;     

public class BattleManager : MonoBehaviour
{

    Coroutine _logCo;
    [SerializeField] float logShowTime = 0.9f;  // 글자 유지 시간
    [SerializeField] float logFadeTime = 0.6f;  // 서서히 사라지는 시간


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

        // 선택값이 준비돼 있으면 즉시 전투 시작
        if (p1Pick != null && p2Pick != null)
            StartBattle();
    }

    // --- Start Battle 버튼에서 호출 ---
    // BattleManager.cs (StartBattle 부분만)
    public void StartBattle()
    {
        if (p1Pick == null || p2Pick == null) { Log("양쪽 캐릭터를 먼저 선택하세요."); return; }

        if (p1) Destroy(p1.gameObject);
        if (p2) Destroy(p2.gameObject);

        if (resultText) resultText.text = "";
        if (logText) logText.text = "";

        battling = true;
        t = roundTime;

        // ✅ Instantiate → 곧바로 Setup으로 초기화 보장
        p1 = Instantiate(p1Pick.prefab, p1Spawn.position, Quaternion.identity);
        p1.Setup(p1Pick);
        p1.OnLog += AddLog;

        p2 = Instantiate(p2Pick.prefab, p2Spawn.position, Quaternion.identity);
        p2.Setup(p2Pick);
        p2.OnLog += AddLog;

        // 킥오프 힘 (가볍게 밀어줌)
        var rb1 = p1.GetComponent<Rigidbody2D>();
        var rb2 = p2.GetComponent<Rigidbody2D>();
        rb1.AddForce(Vector2.right * 4f, ForceMode2D.Impulse);
        rb2.AddForce(Vector2.left * 4f, ForceMode2D.Impulse);

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

    // 기존 AddLog 그대로 둬도 되고, 이렇게 둡니다.
    void AddLog(string m) => Log(m);

    // ✅ 누적 대신 "한 줄만" 보여주고 자동으로 사라짐
    void Log(string m)
    {
        if (!logText) return;

        // 이전 표시 코루틴이 돌고 있으면 정지
        if (_logCo != null) StopCoroutine(_logCo);
        _logCo = StartCoroutine(CoShowLogOnce(m));
    }

    System.Collections.IEnumerator CoShowLogOnce(string m)
    {
        // 내용/불투명도 초기화
        logText.text = m;
        var c = logText.color;
        c.a = 1f;
        logText.color = c;

        // 잠깐 유지
        yield return new WaitForSeconds(logShowTime);

        // 페이드 아웃
        float t = 0f;
        while (t < logFadeTime)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, t / logFadeTime);
            c.a = a;
            logText.color = c;
            yield return null;
        }

        // 깔끔히 비우기
        logText.text = "";
        _logCo = null;
    }

}
