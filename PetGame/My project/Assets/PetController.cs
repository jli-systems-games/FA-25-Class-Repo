using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class PetController : MonoBehaviour
{
    public Animator anim;
    public SimpleWalkerDesktop walker;
    public PetVitals vitals = new PetVitals { hp = 100, mood = 100, energy = 100 };
    public float speedToRun = 3.5f;
    public float sleepSeconds = 5f;
    public float healEveryMinutes = 5f;
    public float nearMissRadius = 0.8f;
    public float jumpCooldown = 5f;
    public int clickDamage = 50;
    public PetMode petMode = PetMode.FollowMouse;
    public int danceClipsCount = 3;

    bool isDrunk, isInjured, isSleeping, isSad;
    float lastJumpTime = -999f;
    float lastBeerTime = -999f;
    Coroutine coHeal, coDrunk, coDance;

    void Awake()
    {
        if (!anim) anim = GetComponentInChildren<Animator>();
        if (!walker) walker = GetComponent<SimpleWalkerDesktop>();
    }

    void Start()
    {
        ApplyAnimatorFlags();
        coHeal = StartCoroutine(AutoHeal());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) ResetPet();
        if (RayHitMe())
        {
            if (Input.GetMouseButtonDown(0)) OnLeftClick();
            if (Input.GetMouseButtonDown(1)) ToggleStageMode();
        }
        if (Time.time - lastJumpTime > jumpCooldown && MouseNearMe(nearMissRadius))
        {
            lastJumpTime = Time.time;
            anim.SetTrigger("Jump");
            AddStat(StatType.Mood, -25);
        }
    }

    public void SetWorldSpeed(float v)
    {
        float s = Mathf.InverseLerp(0f, speedToRun, v);
        anim.SetFloat("Speed", s);
    }

    public void SetStageMode(bool v)
    {
        petMode = v ? PetMode.Stage : PetMode.FollowMouse;
        anim.SetBool("StageMode", v);
        if (v) { SetSad(false); if (coDance != null) StopCoroutine(coDance); coDance = StartCoroutine(DanceLoop()); }
        else { if (coDance != null) StopCoroutine(coDance); anim.SetInteger("DanceIndex", -1); }
    }

    public void ToggleStageMode()
    {
        SetStageMode(petMode == PetMode.FollowMouse);
    }

    public void SetSad(bool v) { isSad = v; anim.SetBool("Sad", v); }

    void OnLeftClick()
    {
        if (isSleeping) return;
        Damage(clickDamage);
    }

    public void Consume(ConsumableSO item)
    {
        AddStat(StatType.HP, item.hpDelta);
        AddStat(StatType.Mood, item.moodDelta);
        AddStat(StatType.Energy, item.energyDelta);
        anim.SetTrigger("Drink");
        if (item.isAlcohol)
        {
            if (Time.time - lastBeerTime <= 5f) StartDrunk(item.drunkSeconds);
            lastBeerTime = Time.time;
        }
    }

    void StartDrunk(float seconds)
    {
        isDrunk = true;
        anim.SetBool("IsDrunk", true);
        if (coDrunk != null) StopCoroutine(coDrunk);
        coDrunk = StartCoroutine(CoDrunk(seconds));
    }

    IEnumerator CoDrunk(float t)
    {
        yield return new WaitForSeconds(t);
        isDrunk = false;
        anim.SetBool("IsDrunk", false);
    }

    void Damage(int d)
    {
        AddStat(StatType.HP, -d);
        isInjured = vitals.hp < 100 && vitals.hp > 0;
        anim.SetBool("IsInjured", isInjured);
        isSad = true; anim.SetBool("Sad", true);
        walker.GoToClosestCorner = true;
        if (vitals.hp <= 0) Die();
    }

    void Die()
    {
        anim.SetTrigger("Die");
        StartCoroutine(CoDieRespawn());
    }

    IEnumerator CoDieRespawn()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        ResetPet();
    }

    public void ResetPet()
    {
        if (walker) walker.TeleportToCorner(0);
        vitals.hp = vitals.mood = vitals.energy = 100;
        vitals.Clamp();
        isSad = isInjured = isDrunk = isSleeping = false;
        SetStageMode(false);
        ApplyAnimatorFlags();
        anim.SetTrigger("Revive");
        gameObject.SetActive(true);
    }

    void ApplyAnimatorFlags()
    {
        anim.SetBool("Sad", isSad);
        anim.SetBool("IsInjured", isInjured);
        anim.SetBool("IsDrunk", isDrunk);
        anim.SetBool("Sleep", isSleeping);
        anim.SetBool("StageMode", petMode == PetMode.Stage);
    }

    void AddStat(StatType type, int delta)
    {
        switch (type)
        {
            case StatType.HP: vitals.hp += delta; break;
            case StatType.Mood: vitals.mood += delta; break;
            case StatType.Energy: vitals.energy += delta; break;
        }
        vitals.Clamp();
        if (!isSleeping && (vitals.mood == 0 || vitals.energy == 0)) StartCoroutine(CoSleep());
    }

    IEnumerator CoSleep()
    {
        isSleeping = true; anim.SetBool("Sleep", true);
        yield return new WaitForSeconds(sleepSeconds);
        vitals.hp = vitals.mood = vitals.energy = 100;
        isSleeping = false; anim.SetBool("Sleep", false);
        anim.SetTrigger("Revive");
    }

    IEnumerator AutoHeal()
    {
        var wait = new WaitForSeconds(healEveryMinutes * 60f);
        while (true)
        {
            yield return wait;
            AddStat(StatType.HP, +25);
            isInjured = vitals.hp < 100;
            anim.SetBool("IsInjured", isInjured);
        }
    }

    IEnumerator DanceLoop()
    {
        while (petMode == PetMode.Stage)
        {
            int idx = Mathf.Clamp(Random.Range(0, danceClipsCount), 0, Mathf.Max(0, danceClipsCount - 1));
            anim.SetInteger("DanceIndex", idx);
            AddStat(StatType.Energy, -10);
            yield return new WaitForSeconds(Random.Range(3f, 6f));
        }
    }

    bool RayHitMe()
    {
        var cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out RaycastHit hit, 1000f) && hit.collider && hit.collider.transform.IsChildOf(transform);
    }

    bool MouseNearMe(float radius)
    {
        var cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        new Plane(Vector3.up, Vector3.zero).Raycast(ray, out float d);
        var world = ray.GetPoint(d);
        world.y = 0;
        Vector3 me = transform.position; me.y = 0;
        return Vector3.Distance(world, me) < radius;
    }
}
