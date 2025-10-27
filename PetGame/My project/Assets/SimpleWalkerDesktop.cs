using System.Collections;
using UnityEngine;

public enum PetMode { Free, Dance, Attack }

[System.Serializable]
public struct Vitals
{
    public int hp, mood, energy;
    public void Clamp() { hp = Mathf.Clamp(hp, 0, 100); mood = Mathf.Clamp(mood, 0, 100); energy = Mathf.Clamp(energy, 0, 100); }
}

public class PetController : MonoBehaviour
{
    public Animator anim;
    public Anchors anchors;
    public PetMode mode = PetMode.Free;
    public Vitals vitals = new Vitals { hp = 100, mood = 100, energy = 100 };
    public int clickDamage = 50;
    public float sleepSeconds = 5f;

    bool sad, injured, drunk, sleeping;

    void Awake() { if (!anim) anim = GetComponentInChildren<Animator>(); }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && RayHitMe()) Hurt(clickDamage);
        if (Input.GetKeyDown(KeyCode.Backslash)) ResetPet();
    }

    bool RayHitMe()
    {
        var cam = Camera.main;
        Ray r = cam.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(r, out RaycastHit h, 200f) && h.collider && h.collider.transform.IsChildOf(transform);
    }

    public void SetSpeed(float worldSpeed, float toRun)
    {
        float s = worldSpeed < 0.05f ? 0f : Mathf.InverseLerp(0f, toRun, worldSpeed);
        anim.SetFloat("Speed", s);
    }

    public void SetMode(PetMode m)
    {
        mode = m;
        anim.SetBool("StageMode", m == PetMode.Dance);
        if (m != PetMode.Dance) anim.SetInteger("DanceInd", -1);
    }

    public void DanceIndex(int i) { anim.SetInteger("DanceInd", i); }
    public void Trigger(string t) { anim.SetTrigger(t); }

    public void Hurt(int d)
    {
        vitals.hp -= d; vitals.Clamp();
        injured = vitals.hp < 100 && vitals.hp > 0;
        anim.SetBool("IsInjured", injured);
        sad = true; anim.SetBool("Sad", true);
        if (vitals.hp <= 0) Die();
    }

    public void Die()
    {
        anim.SetTrigger("Die");
        StartCoroutine(CoDie());
    }

    IEnumerator CoDie()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        Revive();
    }

    public void Revive()
    {
        gameObject.SetActive(true);
        if (anchors && anchors.respawnPoint) transform.position = anchors.respawnPoint.position;
        sad = false; injured = false; sleeping = false; drunk = false;
        vitals.hp = Mathf.Max(vitals.hp, 50);
        vitals.mood = Mathf.Max(vitals.mood, 50);
        vitals.energy = Mathf.Max(vitals.energy, 50);
        anim.SetBool("Sad", false);
        anim.SetBool("IsInjured", false);
        anim.SetBool("Sleep", false);
        anim.SetBool("IsDrunk", false);
        anim.SetTrigger("Revive");
        SetMode(PetMode.Free);
    }

    public void Sleep()
    {
        if (sleeping) return;
        StartCoroutine(CoSleep());
    }

    IEnumerator CoSleep()
    {
        sleeping = true; anim.SetBool("Sleep", true);
        yield return new WaitForSeconds(sleepSeconds);
        vitals.hp = vitals.mood = vitals.energy = 100;
        sleeping = false; anim.SetBool("Sleep", false);
        anim.SetTrigger("Revive");
    }

    public void ResetPet()
    {
        vitals.hp = vitals.mood = vitals.energy = 100; vitals.Clamp();
        sad = injured = drunk = sleeping = false;
        anim.SetBool("Sad", false);
        anim.SetBool("IsInjured", false);
        anim.SetBool("Sleep", false);
        anim.SetBool("IsDrunk", false);
        SetMode(PetMode.Free);
        if (anchors && anchors.respawnPoint) transform.position = anchors.respawnPoint.position;
        anim.SetTrigger("Revive");
    }
}
