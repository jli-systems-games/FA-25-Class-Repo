using UnityEngine;

public class BeybladeStats2D : MonoBehaviour
{
    public float spinPower;
    public float defense;
    public float stamina;
    public float weight;
    public float maxStamina;

    public Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetupStats(float sp, float def, float stam, float wt)
    {
        spinPower = sp;
        defense = def;
        weight = wt;

        maxStamina = stam * 20f;   // scale up stamina
        stamina = maxStamina;      // start at full stamina
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        BeybladeStats2D enemy = col.gameObject.GetComponent<BeybladeStats2D>();
        if (enemy == null) return;

        float impact = Mathf.Max(0.1f, spinPower - enemy.defense);
        float damage = impact * 0.2f;

        enemy.stamina = Mathf.Max(0, enemy.stamina - damage);

        Vector2 normal = col.GetContact(0).normal;
        Vector2 force = -normal * impact * weight * 6f;

        enemy.rb.AddForce(force, ForceMode2D.Impulse);
    }

    // ? FIXED: Blade dies as soon as stamina reaches 0 ?
    public bool IsDead(float threshold)
    {
        return stamina <= 0;
    }
}
