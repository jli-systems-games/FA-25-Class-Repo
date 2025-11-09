using UnityEngine;
using System;                       // Action 델리게이트용
using Random = UnityEngine.Random;  // 모호성 방지

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class AnimalController : MonoBehaviour
{
    public AnimalStats stats;
    public int HP { get; private set; }

    public Action<string> OnLog;    // 전투 로그 콜백(선택)

    Rigidbody2D rb;
    float nextImpulse;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Start()
    {
        HP = stats.maxHP;
        Kick();
        nextImpulse = Time.time + stats.impulseInterval;
    }

    void Update()
    {
        if (Time.time >= nextImpulse)
        {
            Kick();
            nextImpulse = Time.time + stats.impulseInterval;
        }

        if (HP <= 0) Destroy(gameObject);
    }

    void Kick()
    {
        Vector2 dir = Random.insideUnitCircle.normalized;
        rb.AddForce(dir * stats.moveImpulse, ForceMode2D.Impulse);
    }

    static bool Roll(float p) => Random.value < Mathf.Clamp01(p);

    void OnCollisionEnter2D(Collision2D col)
    {
        var other = col.collider.GetComponent<AnimalController>();
        if (!other) return;

        TryAttack(this, other);
        TryAttack(other, this);
    }

    void TryAttack(AnimalController atk, AnimalController def)
    {
        if (!Roll(atk.stats.attackChance)) return;

        if (Roll(def.stats.evadeChance))
        {
            OnLog?.Invoke($"{def.stats.displayName} 회피 성공!");
            return;
        }
        if (Roll(def.stats.blockChance))
        {
            OnLog?.Invoke($"{def.stats.displayName} 방어 성공!");
            return;
        }

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
