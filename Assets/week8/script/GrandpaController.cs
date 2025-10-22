// GrandpaController.cs
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct NeedConfig
{
    public GrandpaState state;
    public float neglectLimitSeconds;
    public int spritePriority;
    public float spawnWeight;
}

public class GrandpaController : MonoBehaviour
{
    // --- Sprites / UI ---
    public Sprite hungrySprite;
    public Sprite dirtySprite;
    public Sprite boredSprite;
    public Sprite sickSprite;
    public Sprite sleepSprite;
    public Sprite idleSpriteIfNeeded;

    public Image grandpaImage;
    public Button medButton;     // Sick
    public Button bubbleButton;  // Dirty
    public Button tvButton;      // Bored
    public Button foodButton;    // Hungry

    // --- Need table ---
    public List<NeedConfig> needConfigs = new List<NeedConfig> {
        new NeedConfig{ state=GrandpaState.Sick,   neglectLimitSeconds=12f, spritePriority=0, spawnWeight=1f },
        new NeedConfig{ state=GrandpaState.Hungry, neglectLimitSeconds=18f, spritePriority=1, spawnWeight=1f },
        new NeedConfig{ state=GrandpaState.Dirty,  neglectLimitSeconds=20f, spritePriority=2, spawnWeight=1f },
        new NeedConfig{ state=GrandpaState.Bored,  neglectLimitSeconds=25f, spritePriority=3, spawnWeight=1f },
    };


    public float baseMinSpawnInterval = 6f;
    public float baseMaxSpawnInterval = 10f;


    public float fatigue = 0f;            
    public float fatigueIncreasePerSec = 5f;
    public float fatigueDecreasePerSec = 20f;
    public float fatigueSleepThreshold = 70f;
    public float fatigueWakeThreshold = 20f;


    public float life = 60f;
    public float baseLifeDecayPerSec = 2f;
    public float perNeedPenaltyPerSec = 1.2f;
    public float sleepDecayMultiplier = 0.5f;


    public float careHealDuration = 1.5f;
    public float careHealPerSec = 6f;
    public float healWhileNeedsMultiplier = 0.6f;
    public float midZoneMin = 40f, midZoneMax = 70f;
    public float healEffLow = 1.0f, healEffMid = 0.65f, healEffHigh = 0.35f;

    public float neglectLimitScale = 1f;   
    public float spawnIntervalScale = 1f; 
    public int maxConcurrentNeeds = 1;

    public bool neglectFailed { get; private set; }

    readonly HashSet<GrandpaState> activeNeeds = new HashSet<GrandpaState>();
    readonly Dictionary<GrandpaState, float> needTimers = new Dictionary<GrandpaState, float>();
    Dictionary<GrandpaState, Sprite> spriteMap;
    bool isSleeping = false;
    System.Random rng = new System.Random();

    void Awake()
    {
        spriteMap = new Dictionary<GrandpaState, Sprite> {
        {GrandpaState.Hungry, hungrySprite},
        {GrandpaState.Dirty,  dirtySprite},
        {GrandpaState.Bored,  boredSprite},
        {GrandpaState.Sick,   sickSprite},
        {GrandpaState.Sleep,  sleepSprite}
    };

        if (medButton) medButton.onClick.AddListener(() => { if (TryResolve(GrandpaState.Sick)) TriggerCareHeal(); });
        if (bubbleButton) bubbleButton.onClick.AddListener(() => { if (TryResolve(GrandpaState.Dirty)) TriggerCareHeal(); });
        if (tvButton) tvButton.onClick.AddListener(() => { if (TryResolve(GrandpaState.Bored)) TriggerCareHeal(); });
        if (foodButton) foodButton.onClick.AddListener(() => { if (TryResolve(GrandpaState.Hungry)) TriggerCareHeal(); });
    }
    void Start()
    {
        AddRandomNeed();
        StartCoroutine(SpawnLoop());
        
        UpdateSprite();
    }

    void Update()
    {
        if (isSleeping)
        {
            fatigue = Mathf.Max(0f, fatigue - fatigueDecreasePerSec * Time.deltaTime);
            if (fatigue <= fatigueWakeThreshold) WakeUp();

            life = Mathf.Clamp(life - (baseLifeDecayPerSec * sleepDecayMultiplier) * Time.deltaTime, 0f, 100f);
            UpdateSprite();
            return;
        }
        else
        {
            fatigue = Mathf.Min(100f, fatigue + fatigueIncreasePerSec * Time.deltaTime);
            if (fatigue >= fatigueSleepThreshold) { GoToSleep(); return; }
        }

        // 개별 타이
        if (activeNeeds.Count > 0)
        {
            var list = new List<GrandpaState>(activeNeeds);
            for (int i = 0; i < list.Count; i++)
            {
                var st = list[i];
                needTimers[st] = needTimers[st] + Time.deltaTime;
                float limit = Mathf.Max(0.1f, GetCfg(st).neglectLimitSeconds * neglectLimitScale);
                if (needTimers[st] >= limit)
                {
                    neglectFailed = true;
                    return;
                }
            }
        }

        // LIFE 감소
        float lifeDecay = baseLifeDecayPerSec + perNeedPenaltyPerSec * activeNeeds.Count;
        life = Mathf.Clamp(life - lifeDecay * Time.deltaTime, 0f, 100f);

        UpdateSprite();
    }


    void GoToSleep()
    {
        isSleeping = true;
        activeNeeds.Clear();
        needTimers.Clear();
        
        SetSprite(GrandpaState.Sleep);
    }

    void WakeUp()
    {
        isSleeping = false;
        
        AddRandomNeed();
        UpdateSprite();
    }

    // --- Resolve ---
    bool TryResolve(GrandpaState target)
    {
        if (isSleeping) return false;
        if (!activeNeeds.Contains(target)) return false;

        activeNeeds.Remove(target);
        needTimers.Remove(target);

        AddRandomNeed();
        
        UpdateSprite();
        return true;
    }

    // --- Heal Burst ---
    void TriggerCareHeal()
    {
        StartCoroutine(CareBoost(careHealPerSec, careHealDuration));
    }

    IEnumerator CareBoost(float perSec, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            float eff = (life < midZoneMin) ? healEffLow : (life < midZoneMax) ? healEffMid : healEffHigh;
            if (activeNeeds.Count > 0) eff *= healWhileNeedsMultiplier;

            life = Mathf.Clamp(life + perSec * eff * Time.deltaTime, 0f, 100f);
            t += Time.deltaTime;
            yield return null;
        }
    }

    // --- Spawning ---
    bool CanSpawnMoreNeeds() => activeNeeds.Count < Mathf.Max(1, maxConcurrentNeeds);

    void AddRandomNeed()
    {
        if (!CanSpawnMoreNeeds()) return;

        var cands = new List<NeedConfig>();
        for (int i = 0; i < needConfigs.Count; i++)
            if (!activeNeeds.Contains(needConfigs[i].state)) cands.Add(needConfigs[i]);
        if (cands.Count == 0) return;

        float total = 0f;
        for (int i = 0; i < cands.Count; i++) total += Mathf.Max(0.0001f, cands[i].spawnWeight);

        float pick = (float)rng.NextDouble() * total;
        float run = 0f;
        for (int i = 0; i < cands.Count; i++)
        {
            var c = cands[i];
            run += Mathf.Max(0.0001f, c.spawnWeight);
            if (pick <= run)
            {
                activeNeeds.Add(c.state);
                needTimers[c.state] = 0f;
                break;
            }
        }

        
        UpdateSprite();
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (!isSleeping && CanSpawnMoreNeeds())
            {
                float t = UnityEngine.Random.Range(baseMinSpawnInterval, baseMaxSpawnInterval);
                t *= Mathf.Max(0.25f, spawnIntervalScale);
                yield return new WaitForSeconds(t);

                if (!isSleeping && CanSpawnMoreNeeds())
                    AddRandomNeed();
            }
            else yield return null;
        }
    }

    // --- Visuals ---
    void UpdateSprite()
    {
        if (grandpaImage == null) return;

        if (isSleeping) { SetSprite(GrandpaState.Sleep); return; }
        if (activeNeeds.Count == 0)
        {
            if (idleSpriteIfNeeded) grandpaImage.sprite = idleSpriteIfNeeded;
            return;
        }

        GrandpaState chosen = default;
        bool set = false;
        float best = float.NegativeInfinity;
        int bestPrio = int.MaxValue;

        foreach (var st in activeNeeds)
        {
            var cfg = GetCfg(st);
            float limit = Mathf.Max(0.1f, cfg.neglectLimitSeconds * neglectLimitScale);
            float score = needTimers[st] / limit;
            if (!set || score > best || (Mathf.Approximately(score, best) && cfg.spritePriority < bestPrio))
            {
                chosen = st; set = true; best = score; bestPrio = cfg.spritePriority;
            }
        }
        SetSprite(chosen);
    }

    void SetSprite(GrandpaState st)
    {
        if (spriteMap != null && spriteMap.TryGetValue(st, out var s) && s != null)
            grandpaImage.sprite = s;
        else if (idleSpriteIfNeeded) grandpaImage.sprite = idleSpriteIfNeeded;
    }



    NeedConfig GetCfg(GrandpaState st)
    {
        for (int i = 0; i < needConfigs.Count; i++)
            if (needConfigs[i].state == st) return needConfigs[i];
        return new NeedConfig { state = st, neglectLimitSeconds = 20f, spritePriority = 99, spawnWeight = 1f };
    }

    // 외부 사용
    public float GetLife() => life;
    public bool IsSleeping => isSleeping;
    public bool HasNeed(GrandpaState target) => activeNeeds.Contains(target);
    public void ResolveNeedExternally(GrandpaState target)
    {
        if (!activeNeeds.Contains(target)) return;

        activeNeeds.Remove(target);
        needTimers.Remove(target);

        AddRandomNeed();
        
        UpdateSprite();
        TriggerCareHeal(); // 외부 해결 성공 시에만 회복
    }
}
