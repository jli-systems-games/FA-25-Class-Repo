using UnityEngine;

public class ArcaneBlade3D : MonoBehaviour
{
    public string bladeName;
    public float spinPower = 400f;
    public float defense = 1f;
    public float stamina = 100f;
    public float decayRate = 5f;
    public ParticleSystem auraFX;

    private Rigidbody rb;
    private bool spinning = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionY |
                         RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ;

        rb.AddTorque(Vector3.up * Random.Range(spinPower * 0.9f, spinPower * 1.1f), ForceMode.Impulse);
        if (auraFX) auraFX.Play();
    }

    void Update()
    {
        if (!spinning) return;

        stamina -= decayRate * Time.deltaTime;
        if (stamina <= 0)
            StopBlade();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.TryGetComponent(out ArcaneBlade3D other))
        {
            float impact = Mathf.Abs(rb.angularVelocity.y) * 0.1f;
            other.stamina -= Mathf.Max(impact - other.defense, 0);
        }
    }

    void StopBlade()
    {
        spinning = false;
        rb.angularVelocity = Vector3.zero;
        if (auraFX) auraFX.Stop();
        FindObjectOfType<CovenBattleManager3D>().CheckWinner();
    }

    public bool IsSpinning() => spinning;
}
