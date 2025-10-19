using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class NeedConfig
{
    public GrandpaState state;
    [Tooltip("이 상태를 방치할 수 있는 최대 시간(초)")]
    public float neglectLimitSeconds = 20f;
    [Tooltip("스프라이트 우선순위(낮을수록 우선)")]
    public int spritePriority = 0;
    [Tooltip("스폰 가중치(확률)")]
    public float spawnWeight = 1f;
}

public class GrandpaController : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite hungrySprite;
    [SerializeField] private Sprite dirtySprite;
    [SerializeField] private Sprite boredSprite;
    [SerializeField] private Sprite sickSprite;
    [SerializeField] private Sprite sleepSprite;
    [SerializeField] private Sprite idleSpriteIfNeeded;

    [Header("UI Refs")]
    [SerializeField] private Image grandpaImage;
    [SerializeField] private Button medButton;     // Sick
    [SerializeField] private Button bubbleButton;  // Dirty
    [SerializeField] private Button tvButton;      // Bored
    [SerializeField] private Button foodButton;    // Hungry

    [Header("Need Config Table")]
    [SerializeField]
    private List<NeedConfig> needConfigs = new List<NeedConfig> {
        new NeedConfig{ state=GrandpaState.Sick,   neglectLimitSeconds=12f, spritePriority=0, spawnWeight=1.0f },
        new NeedConfig{ state=GrandpaState.Hungry, neglectLimitSeconds=18f, spritePriority=1, spawnWeight=1.0f },
        new NeedConfig{ state=GrandpaState.Dirty,  neglectLimitSeconds=20f, spritePriority=2, spawnWeight=1.0f },
        new NeedConfig{ state=GrandpaState.Bored,  neglectLimitSeconds=25f, spritePriority=3, spawnWeight=1.0f },
    };

    [Header("Spawn Timing (base)")]
    [SerializeField] private float baseMinSpawnInterval = 6f;
    [SerializeField] private float baseMaxSpawnInterval = 10f;

    [Header("Fatigue")]
    [SerializeField, Tooltip("0~100")] private float fatigue = 0f;
    [SerializeField] private float fatigueIncreasePerSec = 5f;   // 깨어있을 때 증가
    [SerializeField] private float fatigueDecreasePerSec = 20f;  // 수면 중 감소
    [SerializeField] private float fatigueSleepThreshold = 70f;  // 이 이상이면 잠듦
    [SerializeField] private float fatigueWakeThreshold = 20f;  // 이 이하면 깸

    // -------- 이벤트(C# event) : 인스펙터에 안 보이는 게 정상입니다 --------
    public event Action OnNeglectExceeded;     // GameOver 트리거
    public event Action<bool> OnSleeping;      // true=잠듦, false=기상

    // -------- 난이도 파라미터(게임매니저가 런타임에서 갱신) --------
    [Header("Difficulty (runtime from GameManager)")]
    [Range(0.25f, 1f)] public float neglectLimitScale = 1f; // 방치 한계 스케일(낮을수록 빡셈)
    [Min(0.25f)] public float spawnIntervalScale = 1f;      // 스폰 간격 스케일(낮을수록 더 자주)
    [Min(1)] public int maxConcurrentNeeds = 1;             // 동시 요구 최대 개수

    // -------- 내부 상태 --------
    private readonly HashSet<GrandpaState> activeNeeds = new HashSet<GrandpaState>();
    private readonly Dictionary<GrandpaState, float> needTimers = new Dictionary<GrandpaState, float>();
    private Dictionary<GrandpaState, Sprite> spriteMap;
    private bool isSleeping = false;
    private System.Random rng = new System.Random();
    private Coroutine spawnLoopCo;

    private void Awake()
    {
        spriteMap = new Dictionary<GrandpaState, Sprite> {
            {GrandpaState.Hungry, hungrySprite},
            {GrandpaState.Dirty,  dirtySprite},
            {GrandpaState.Bored,  boredSprite},
            {GrandpaState.Sick,   sickSprite},
            {GrandpaState.Sleep,  sleepSprite}
        };

        // 버튼 연결
        if (medButton) medButton.onClick.AddListener(() => TryResolve(GrandpaState.Sick));
        if (bubbleButton) bubbleButton.onClick.AddListener(() => TryResolve(GrandpaState.Dirty));
        if (tvButton) tvButton.onClick.AddListener(() => TryResolve(GrandpaState.Bored));
        if (foodButton) foodButton.onClick.AddListener(() => TryResolve(GrandpaState.Hungry));
    }

    private void Start()
    {
        // 시작 시 하나 스폰
        AddRandomNeed();
        spawnLoopCo = StartCoroutine(SpawnLoop());

        UpdateButtonsInteractable();
        UpdateSprite();
    }

    private void Update()
    {
        // 수면/피로
        if (isSleeping)
        {
            fatigue = Mathf.Max(0f, fatigue - fatigueDecreasePerSec * Time.deltaTime);
            if (fatigue <= fatigueWakeThreshold)
            {
                WakeUp();
            }
            return; // 수면 중엔 방치 타이머 정지
        }
        else
        {
            fatigue = Mathf.Min(100f, fatigue + fatigueIncreasePerSec * Time.deltaTime);
            if (fatigue >= fatigueSleepThreshold)
            {
                GoToSleep();
                return;
            }
        }

        // 개별 방치 타이머
        foreach (var st in activeNeeds.ToList())
        {
            needTimers[st] += Time.deltaTime;

            float limit = Mathf.Max(0.1f, GetConfig(st).neglectLimitSeconds * neglectLimitScale);
            if (needTimers[st] >= limit)
            {
                OnNeglectExceeded?.Invoke(); // GameOver
                return;
            }
        }

        UpdateSprite();
    }

    // ---------------- 수면 ----------------
    private void GoToSleep()
    {
        isSleeping = true;

        // 잠들면 요구 초기화(원하시면 유지로 바꿔도 됨)
        activeNeeds.Clear();
        needTimers.Clear();

        OnSleeping?.Invoke(true);
        UpdateButtonsInteractable();
        SetSprite(GrandpaState.Sleep);
    }

    private void WakeUp()
    {
        isSleeping = false;
        OnSleeping?.Invoke(false);
        UpdateButtonsInteractable();

        // 깨어나면 즉시 하나 스폰
        AddRandomNeed();
        UpdateSprite();
    }

    // ---------------- 요구 해결 ----------------
    private void TryResolve(GrandpaState target)
    {
        if (isSleeping) return;
        if (!activeNeeds.Contains(target)) return;

        // 해결: 해당 요구 제거
        activeNeeds.Remove(target);
        needTimers.Remove(target);

        // 해결 직후 새 요구 하나 추가(패턴 끊기지 않게)
        AddRandomNeed();

        UpdateButtonsInteractable();
        UpdateSprite();
    }

    // ---------------- 스폰 ----------------
    private bool CanSpawnMoreNeeds() => activeNeeds.Count < Mathf.Max(1, maxConcurrentNeeds);

    private void AddRandomNeed()
    {
        if (!CanSpawnMoreNeeds()) return;

        // Sleep 제외 + 이미 활성화된 것 제외
        var candidates = needConfigs.Where(c => !activeNeeds.Contains(c.state)).ToList();
        if (candidates.Count == 0) return;

        float totalW = candidates.Sum(c => Mathf.Max(0.0001f, c.spawnWeight));
        float pick = (float)rng.NextDouble() * totalW;
        float run = 0f;

        foreach (var c in candidates)
        {
            run += Mathf.Max(0.0001f, c.spawnWeight);
            if (pick <= run)
            {
                activeNeeds.Add(c.state);
                needTimers[c.state] = 0f;
                break;
            }
        }

        UpdateButtonsInteractable();
        UpdateSprite();
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (!isSleeping && CanSpawnMoreNeeds())
            {
                float t = UnityEngine.Random.Range(baseMinSpawnInterval, baseMaxSpawnInterval);
                t *= Mathf.Max(0.25f, spawnIntervalScale); // 난이도에 따라 단축
                yield return new WaitForSeconds(t);

                if (!isSleeping && CanSpawnMoreNeeds())
                    AddRandomNeed();
            }
            else
            {
                yield return null;
            }
        }
    }

    // ---------------- 스프라이트/버튼 ----------------
    private void UpdateSprite()
    {
        if (isSleeping)
        {
            SetSprite(GrandpaState.Sleep);
            return;
        }

        if (activeNeeds.Count == 0)
        {
            if (idleSpriteIfNeeded && grandpaImage) grandpaImage.sprite = idleSpriteIfNeeded;
            return;
        }

        // (타이머/한계) 비율 높은 순 → 동률이면 spritePriority 낮은 순
        GrandpaState chosen = activeNeeds
            .OrderByDescending(st => needTimers[st] / Mathf.Max(0.1f, GetConfig(st).neglectLimitSeconds * neglectLimitScale))
            .ThenBy(st => GetConfig(st).spritePriority)
            .First();

        SetSprite(chosen);
    }

    private void SetSprite(GrandpaState st)
    {
        if (grandpaImage == null) return;

        if (spriteMap.TryGetValue(st, out var s) && s != null)
            grandpaImage.sprite = s;
        else if (idleSpriteIfNeeded != null)
            grandpaImage.sprite = idleSpriteIfNeeded;
    }

    private void UpdateButtonsInteractable()
    {
        bool can = !isSleeping;

        if (medButton) medButton.interactable = can && activeNeeds.Contains(GrandpaState.Sick);
        if (bubbleButton) bubbleButton.interactable = can && activeNeeds.Contains(GrandpaState.Dirty);
        if (tvButton) tvButton.interactable = can && activeNeeds.Contains(GrandpaState.Bored);
        if (foodButton) foodButton.interactable = can && activeNeeds.Contains(GrandpaState.Hungry);
    }

    // ---------------- 유틸 ----------------
    private NeedConfig GetConfig(GrandpaState st)
    {
        // 테이블에 반드시 존재한다고 가정
        return needConfigs.First(c => c.state == st);
    }

    // 외부에서 확인/디버그용(옵션)
    public bool IsSleeping => isSleeping;
    public IReadOnlyCollection<GrandpaState> ActiveNeeds => activeNeeds;
    public float GetFatigue() => fatigue;
}
