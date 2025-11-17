using UnityEngine;

public class ArcaneBlade2D : MonoBehaviour
{
    [Header("Stats")]
    public string bladeName = "Default Blade";
    public float spinPower = 300f;
    public float defense = 3f;
    public float stamina = 100f;
    public float decayRate = 5f;

    private Rigidbody2D rb;
    private bool spinning = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // ✅ Called by the Battle Manager to start the battle
    public void BeginSpin()
    {
        spinning = true;
        stamina = Mathf.Max(stamina, 1f);
        rb.angularVelocity = 0;
        rb.AddTorque(spinPower);
    }

    void Update()
    {
        if (!spinning) return;

        stamina -= decayRate * Time.deltaTime;
        if (stamina <= 0)
            StopBlade();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.TryGetComponent(out ArcaneBlade2D other))
        {
            float impact = Mathf.Abs(rb.angularVelocity) * 0.05f;
            other.stamina -= Mathf.Max(impact - other.defense, 0);
        }
    }

    void StopBlade()
    {
        spinning = false;
        rb.angularVelocity = 0;
    }

    public bool IsSpinning() => spinning;
}
