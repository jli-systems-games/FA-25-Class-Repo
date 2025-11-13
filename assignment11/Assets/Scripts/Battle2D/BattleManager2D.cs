using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleManager2D : MonoBehaviour
{
    [Header("Spawns & Prefab")]
    [SerializeField] private Transform leftSpawnA;
    [SerializeField] private Transform leftSpawnB;
    [SerializeField] private Transform rightSpawnA;
    [SerializeField] private Transform rightSpawnB;
    [SerializeField] private ElementUnit2D unitPrefab;

    [Header("UI - Player Picks")]
    [SerializeField] private TMP_Dropdown leftDropdownA;
    [SerializeField] private TMP_Dropdown leftDropdownB;
    [SerializeField] private Button startButton;

    [Header("UI - Bars & Icons (4 units)")]
    [SerializeField] private Image leftAIcon; [SerializeField] private Slider leftA_HP;
    [SerializeField] private Image leftBIcon; [SerializeField] private Slider leftB_HP;
    [SerializeField] private Image rightAIcon; [SerializeField] private Slider rightA_HP;
    [SerializeField] private Image rightBIcon; [SerializeField] private Slider rightB_HP;
    [SerializeField] private TMP_Text resultText;

    [Header("UI - Overlay Texts")]
    [SerializeField] private TMP_Text announcerText;
    [SerializeField] private TMP_Text bannerText;

    [Header("SFX (Optional)")]
    [SerializeField] private AudioSource sfx;
    [SerializeField] private AudioClip beep;
    [SerializeField] private AudioClip fight;
    [SerializeField] private AudioClip comboSfx;

    [Header("Database")]
    [SerializeField] private ElementData[] allElements;

    [Header("Balance - basic")]
    [SerializeField] private float baseTurnInterval = 1.0f;
    [SerializeField] private float strongMultiplier = 1.5f;
    [SerializeField] private float weakMultiplier = 0.7f;
    [SerializeField] private Vector2 randomRange = new Vector2(0.9f, 1.1f);

    [Header("Balance - team/combos")]
    [SerializeField] private float comboCooldown = 4.0f;
    [SerializeField] private float shieldReduction = 0.7f;
    [SerializeField] private float burnTickInterval = 0.5f;

    [Header("Smoothing")]
    [SerializeField] private bool smoothHP = true;
    [SerializeField] private float hpLerpSpeed = 10f;

    private ElementUnit2D leftA, leftB, rightA, rightB;

    private float intv_leftA, intv_leftB, intv_rightA, intv_rightB;
    private float t_leftA, t_leftB, t_rightA, t_rightB;

    private float nextComboTimeLeft;
    private float nextComboTimeRight;

    private readonly Dictionary<ElementUnit2D, float> shieldUntil = new Dictionary<ElementUnit2D, float>();

    private Coroutine bannerCo;
    private Coroutine introCo;

    private void Awake()
    {
        SetupDropdowns();
        if (startButton) startButton.onClick.AddListener(OnStartClicked);
        if (resultText) resultText.text = "";
        SetTMPAlpha(announcerText, 0f);
        SetTMPAlpha(bannerText, 0f);
    }

    private void Update()
    {
        if (leftA && leftA_HP) leftA_HP.value = smoothHP ? Mathf.Lerp(leftA_HP.value, leftA.CurrentHealth, Time.deltaTime * hpLerpSpeed) : leftA.CurrentHealth;
        if (leftB && leftB_HP) leftB_HP.value = smoothHP ? Mathf.Lerp(leftB_HP.value, leftB.CurrentHealth, Time.deltaTime * hpLerpSpeed) : leftB.CurrentHealth;
        if (rightA && rightA_HP) rightA_HP.value = smoothHP ? Mathf.Lerp(rightA_HP.value, rightA.CurrentHealth, Time.deltaTime * hpLerpSpeed) : rightA.CurrentHealth;
        if (rightB && rightB_HP) rightB_HP.value = smoothHP ? Mathf.Lerp(rightB_HP.value, rightB.CurrentHealth, Time.deltaTime * hpLerpSpeed) : rightB.CurrentHealth;
    }

    private void SetupDropdowns()
    {
        var options = new List<TMP_Dropdown.OptionData>();
        foreach (var e in allElements)
        {
            var op = new TMP_Dropdown.OptionData();
            op.text = e.displayName;
            op.image = e.icon;
            options.Add(op);
        }
        leftDropdownA.ClearOptions(); leftDropdownB.ClearOptions();
        leftDropdownA.AddOptions(options); leftDropdownB.AddOptions(options);
    }

    private void OnStartClicked()
    {
        if (introCo != null) StopCoroutine(introCo);
        StopAllCoroutines();
        if (resultText) resultText.text = "";

        startButton.interactable = false;
        ClearOldUnits();

        int idxA = Mathf.Clamp(leftDropdownA.value, 0, allElements.Length - 1);
        int idxB = Mathf.Clamp(leftDropdownB.value, 0, allElements.Length - 1);
        if (idxB == idxA) idxB = (idxA + 1) % allElements.Length;

        var leftDataA = allElements[idxA];
        var leftDataB = allElements[idxB];

        int ridxA = Random.Range(0, allElements.Length);
        int ridxB = Random.Range(0, allElements.Length);
        int safety = 0;
        while (ridxB == ridxA && safety++ < 20) ridxB = Random.Range(0, allElements.Length);

        var rightDataA = allElements[ridxA];
        var rightDataB = allElements[ridxB];

        leftA = Instantiate(unitPrefab, leftSpawnA.position, Quaternion.identity);
        leftB = Instantiate(unitPrefab, leftSpawnB.position, Quaternion.identity);
        rightA = Instantiate(unitPrefab, rightSpawnA.position, Quaternion.identity);
        rightB = Instantiate(unitPrefab, rightSpawnB.position, Quaternion.identity);

        leftA.Init(leftDataA); leftB.Init(leftDataB);
        rightA.Init(rightDataA); rightB.Init(rightDataB);

        leftA.GetComponent<SpriteRenderer>().sortingOrder = 2;
        leftB.GetComponent<SpriteRenderer>().sortingOrder = 1;
        rightA.GetComponent<SpriteRenderer>().sortingOrder = 2;
        rightB.GetComponent<SpriteRenderer>().sortingOrder = 1;

        InitBar(leftAIcon, leftA_HP, leftA);
        InitBar(leftBIcon, leftB_HP, leftB);
        InitBar(rightAIcon, rightA_HP, rightA);
        InitBar(rightBIcon, rightB_HP, rightB);

        intv_leftA = GetInterval(leftA); t_leftA = 0f;
        intv_leftB = GetInterval(leftB); t_leftB = 0f;
        intv_rightA = GetInterval(rightA); t_rightA = 0f;
        intv_rightB = GetInterval(rightB); t_rightB = 0f;

        nextComboTimeLeft = Time.time + 2f;
        nextComboTimeRight = Time.time + 2f;
        shieldUntil.Clear();

        introCo = StartCoroutine(MatchSequence());
    }

    private IEnumerator MatchSequence()
    {
        string line = $"Player: {leftA.Data.displayName} + {leftB.Data.displayName}\n" +
                      $"PC: {rightA.Data.displayName} + {rightB.Data.displayName}";
        yield return StartCoroutine(AnnounceLine(line, 1.6f));

        yield return StartCoroutine(Countdown321());

        yield return StartCoroutine(ShowBanner("FIGHT!", Color.white, 1.1f));

        yield return StartCoroutine(BattleLoop());

        startButton.interactable = true;
    }

    private IEnumerator AnnounceLine(string text, float hold)
    {
        if (!announcerText) yield break;
        announcerText.text = text;
        yield return StartCoroutine(FadeTMP(announcerText, 0f, 1f, 0.35f));
        yield return new WaitForSeconds(hold);
        yield return StartCoroutine(FadeTMP(announcerText, 1f, 0f, 0.35f));
    }

    private IEnumerator Countdown321()
    {
        string[] arr = { "3", "2", "1" };
        foreach (var s in arr)
        {
            if (sfx && beep) sfx.PlayOneShot(beep);
            yield return StartCoroutine(ShowBanner(s, Color.white, 0.7f));
            yield return new WaitForSeconds(0.1f);
        }
        if (sfx && fight) sfx.PlayOneShot(fight);
    }

    private IEnumerator BattleLoop()
    {
        var cam = FindObjectOfType<CameraDirector2D>();

        while (TeamAlive(leftA, leftB) && TeamAlive(rightA, rightB))
        {
            TickUnitAttack(leftA, ref t_leftA, intv_leftA, rightA, rightB, cam);
            if (!TeamAlive(rightA, rightB)) break;

            TickUnitAttack(leftB, ref t_leftB, intv_leftB, rightA, rightB, cam);
            if (!TeamAlive(rightA, rightB)) break;

            TickUnitAttack(rightA, ref t_rightA, intv_rightA, leftA, leftB, cam);
            if (!TeamAlive(leftA, leftB)) break;

            TickUnitAttack(rightB, ref t_rightB, intv_rightB, leftA, leftB, cam);
            if (!TeamAlive(leftA, leftB)) break;

            if (Time.time >= nextComboTimeLeft && BothAlive(leftA, leftB)) { TryTriggerCombo(true); nextComboTimeLeft = Time.time + comboCooldown; }
            if (Time.time >= nextComboTimeRight && BothAlive(rightA, rightB)) { TryTriggerCombo(false); nextComboTimeRight = Time.time + comboCooldown; }

            yield return null;
        }

        string winner;
        bool leftWin = TeamAlive(leftA, leftB);
        bool rightWin = TeamAlive(rightA, rightB);
        if (leftWin && !rightWin) winner = "Player";
        else if (!leftWin && rightWin) winner = "PC";
        else winner = "Draw";

        if (resultText) resultText.text = winner == "Draw" ? "Result: Draw" : $"Winner: {winner}";
    }

    private void TickUnitAttack(ElementUnit2D attacker, ref float timer, float interval, ElementUnit2D enemyA, ElementUnit2D enemyB, CameraDirector2D cam)
    {
        if (attacker == null || !attacker.IsAlive) return;
        timer += Time.deltaTime;
        if (timer < interval) return;
        timer = 0f;

        var targets = GetAliveList(enemyA, enemyB);
        if (targets.Count == 0) return;
        var target = targets[Random.Range(0, targets.Count)];

        float dmg = CalculateDamage(attacker.Data, target.Data, IsShielded(target));
        target.TakeDamage(dmg);
        attacker.PlayAttackVFX();
        if (cam) cam.Punch(0.1f, 0.15f);
    }

    private void TryTriggerCombo(bool isLeftTeam)
    {
        ElementUnit2D a = isLeftTeam ? leftA : rightA;
        ElementUnit2D b = isLeftTeam ? leftB : rightB;
        ElementUnit2D e1 = isLeftTeam ? rightA : leftA;
        ElementUnit2D e2 = isLeftTeam ? rightB : leftB;

        var t1 = a.Data.elementType;
        var t2 = b.Data.elementType;

        string teamTag = isLeftTeam ? "PLAYER" : "PC";

        if (IsPair(t1, t2, ElementType.Fire, ElementType.Air))
        {
            var enemies = GetAliveList(e1, e2);
            if (enemies.Count == 0) return;
            var target = enemies[Random.Range(0, enemies.Count)];

            float baseBurst = (a.Data.attack + b.Data.attack) * 1.0f;
            target.TakeDamage(baseBurst);
            StartCoroutine(BurnDOT(target, a.Data.attack * 0.3f, 3, burnTickInterval));

            ShowComboBanner($"{teamTag} COMBO\nFIRE TORNADO", new Color(1f, 0.45f, 0.12f));
        }
        else if (IsPair(t1, t2, ElementType.Water, ElementType.Earth))
        {
            HealUnit(a, a.Data.maxHealth * 0.15f);
            HealUnit(b, b.Data.maxHealth * 0.15f);
            ApplyShield(a, 3f);
            ApplyShield(b, 3f);

            ShowComboBanner($"{teamTag} COMBO\nMUD SHIELD", new Color(0.45f, 0.75f, 0.5f));
        }
        else if (IsPair(t1, t2, ElementType.Lightning, ElementType.Air))
        {
            var enemies = GetAliveList(e1, e2);
            float dmg = (a.Data.attack + b.Data.attack) * 0.6f;
            for (int i = 0; i < enemies.Count; i++) enemies[i].TakeDamage(dmg);

            ShowComboBanner($"{teamTag} COMBO\nCHAIN LIGHTNING", new Color(1f, 0.95f, 0.45f));
        }
    }

    private void ShowComboBanner(string text, Color color)
    {
        if (sfx && comboSfx) sfx.PlayOneShot(comboSfx);
        StartCoroutine(ShowBanner(text, color, 1.2f));
    }

    private IEnumerator BurnDOT(ElementUnit2D target, float tickDamage, int ticks, float interval)
    {
        for (int i = 0; i < ticks; i++)
        {
            if (target && target.IsAlive) target.TakeDamage(tickDamage);
            yield return new WaitForSeconds(interval);
        }
    }

    private void HealUnit(ElementUnit2D unit, float amount)
    {
        if (!unit || !unit.IsAlive) return;
        unit.TakeDamage(-amount);
    }

    private void ApplyShield(ElementUnit2D unit, float duration)
    {
        if (!unit) return;
        shieldUntil[unit] = Time.time + duration;
    }

    private bool IsShielded(ElementUnit2D unit)
    {
        if (unit == null) return false;
        if (shieldUntil.TryGetValue(unit, out float until))
            return Time.time < until;
        return false;
    }

    private float CalculateDamage(ElementData atk, ElementData def, bool defenderShielded)
    {
        float baseDamage = Mathf.Max(1f, atk.attack - def.defense * 0.5f);
        float typeBonus = 1f;
        if (atk.strengthAgainst == def.elementType) typeBonus = strongMultiplier;
        if (atk.weakAgainst == def.elementType) typeBonus = weakMultiplier;
        float rnd = Random.Range(randomRange.x, randomRange.y);
        float dmg = baseDamage * typeBonus * rnd;
        if (defenderShielded) dmg *= shieldReduction;
        return dmg;
    }

    private bool BothAlive(ElementUnit2D u1, ElementUnit2D u2) => (u1 && u1.IsAlive) && (u2 && u2.IsAlive);
    private bool TeamAlive(ElementUnit2D u1, ElementUnit2D u2) => (u1 && u1.IsAlive) || (u2 && u2.IsAlive);

    private List<ElementUnit2D> GetAliveList(ElementUnit2D a, ElementUnit2D b)
    {
        var list = new List<ElementUnit2D>(2);
        if (a && a.IsAlive) list.Add(a);
        if (b && b.IsAlive) list.Add(b);
        return list;
    }

    private float GetInterval(ElementUnit2D u)
    {
        return Mathf.Max(0.1f, baseTurnInterval / Mathf.Max(0.1f, u.Data.speed));
    }

    private void InitBar(Image icon, Slider hp, ElementUnit2D u)
    {
        if (icon) icon.sprite = u.Data.icon;
        if (hp) { hp.maxValue = u.Data.maxHealth; hp.value = u.Data.maxHealth; }
    }

    private void ClearOldUnits()
    {
        if (leftA) Destroy(leftA.gameObject);
        if (leftB) Destroy(leftB.gameObject);
        if (rightA) Destroy(rightA.gameObject);
        if (rightB) Destroy(rightB.gameObject);
        leftA = leftB = rightA = rightB = null;
    }

    private void SetTMPAlpha(TMP_Text t, float a)
    {
        if (!t) return;
        var c = t.color; c.a = a; t.color = c;
    }

    private IEnumerator FadeTMP(TMP_Text t, float from, float to, float dur)
    {
        if (!t) yield break;
        float t0 = 0f;
        while (t0 < dur)
        {
            t0 += Time.deltaTime;
            float a = Mathf.Lerp(from, to, t0 / dur);
            var c = t.color; c.a = a; t.color = c;
            yield return null;
        }
        var c2 = t.color; c2.a = to; t.color = c2;
    }

    private IEnumerator ShowBanner(string text, Color color, float hold)
    {
        if (!bannerText) yield break;
        if (bannerCo != null) StopCoroutine(bannerCo);
        bannerCo = StartCoroutine(BannerRoutine(text, color, hold));
        yield return bannerCo;
    }

    private IEnumerator BannerRoutine(string text, Color color, float hold)
    {
        bannerText.text = text;
        bannerText.color = new Color(color.r, color.g, color.b, 0f);
        var rt = bannerText.rectTransform;
        rt.localScale = Vector3.one * 0.2f;

        float inDur = 0.25f;
        float t0 = 0f;
        while (t0 < inDur)
        {
            t0 += Time.deltaTime;
            float k = t0 / inDur;
            SetTMPAlpha(bannerText, Mathf.Lerp(0f, 1f, k));
            rt.localScale = Vector3.one * Mathf.Lerp(0.2f, 1.05f, k);
            yield return null;
        }
        SetTMPAlpha(bannerText, 1f);
        rt.localScale = Vector3.one * 1.05f;

        float bDur = 0.12f; t0 = 0f;
        while (t0 < bDur)
        {
            t0 += Time.deltaTime;
            rt.localScale = Vector3.one * Mathf.Lerp(1.05f, 1f, t0 / bDur);
            yield return null;
        }

        yield return new WaitForSeconds(hold);

        float outDur = 0.25f; t0 = 0f;
        while (t0 < outDur)
        {
            t0 += Time.deltaTime;
            SetTMPAlpha(bannerText, Mathf.Lerp(1f, 0f, t0 / outDur));
            yield return null;
        }
        SetTMPAlpha(bannerText, 0f);
    }

    private bool IsPair(ElementType a, ElementType b, ElementType x, ElementType y)
    {
        return (a == x && b == y) || (a == y && b == x);
    }
}
