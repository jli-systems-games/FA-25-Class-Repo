// AnimalController.cs
using UnityEngine;
using System;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class AnimalController : MonoBehaviour
{
    public AnimalStats stats;
    public int HP { get; private set; }
    public Action<string> OnLog;

    Rigidbody2D rb;
    float nextImpulse;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;          // ✅ 떨어지지 않게
        rb.freezeRotation = true;      // ✅ Z 회전 고정(원형 아이콘이면 권장)
    }

    // ✅ 외부에서 확실히 스탯을 넣은 뒤 호출
    public void Setup(AnimalStats s)
    {
        stats = s;
        HP = Mathf.Max(1, stats.maxHP);                 // 혹시 0이면 최소 1
        Kick();
        nextImpulse = Time.time + Mathf.Max(0.1f, stats.impulseInterval);
    }

    void Update()
    {
        if (stats == null) return; // 안전장치
        if (Time.time >= nextImpulse)
        {
            Kick();
            nextImpulse = Time.time + stats.impulseInterval;
        }
        if (HP <= 0) Destroy(gameObject);
    }

    void Kick()
    {
        if (stats == null) return;
        Vector2 dir = Random.insideUnitCircle.normalized;
        GetComponent<Rigidbody2D>().AddForce(dir * Mathf.Max(0.1f, stats.moveImpulse), ForceMode2D.Impulse);
    }

    static bool Roll(float p) => Random.value < Mathf.Clamp01(p);

    void OnCollisionEnter2D(Collision2D col)
    {
        var other = col.collider.GetComponent<AnimalController>();
        if (!other || stats == null || other.stats == null) return;

        TryAttack(this, other);
        TryAttack(other, this);
    }

    void TryAttack(AnimalController atk, AnimalController def)
    {
        if (!Roll(atk.stats.attackChance)) return;

        if (Roll(def.stats.evadeChance)) { OnLog?.Invoke($"{def.stats.displayName} 회피 성공!"); return; }
        if (Roll(def.stats.blockChance)) { OnLog?.Invoke($"{def.stats.displayName} 방어 성공!"); return; }

        int dmg = atk.stats.baseDamage * (Roll(atk.stats.critChance) ? 2 : 1);
        def.HP -= dmg;
        OnLog?.Invoke($"{atk.stats.displayName} 공격 성공! {def.stats.displayName} 체력 -{dmg}");

        if (def.HP > 0 && Roll(def.stats.counterChance))
        {
            atk.HP -= 5;
            OnLog?.Invoke($"{def.stats.displayName} 반격 성공!! {atk.stats.displayName} 체력 -5");
        }
    }
}
