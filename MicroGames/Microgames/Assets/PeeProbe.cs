using UnityEngine;

public class PeeProbe : MonoBehaviour
{
    [Header("Lifetime")]
    public float life = 10.0f; // 固定为 10 秒
    public float gravityScale = 0.5f; // 自定义重力系数
    public ParticleSystem splashFX;
    public AudioClip hitSound;
    public AudioClip missSound;
    private float timer;
    bool hit;
    public ObjectPool<PeeProbe> myPool;

    void OnEnable()
    {
        timer = life;
        hit = false;
        var rb = GetComponent<Rigidbody2D>();
        if (rb) rb.gravityScale = gravityScale;
        Debug.Log($"PeeProbe initialized with life: {timer} seconds");
    }

    void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            Debug.Log($"Remaining time: {timer} seconds");
            if (timer <= 0f)
            {
                Miss();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("HitZone"))
        {
            hit = true;
            if (GameManager.I != null) GameManager.I.RegisterHit();
            if (splashFX) Instantiate(splashFX, transform.position, Quaternion.identity);
            if (hitSound) AudioSource.PlayClipAtPoint(hitSound, transform.position);
            Deactivate();
        }
    }

    void Miss()
    {
        if (!hit && GameManager.I != null)
        {
            GameManager.I.RegisterMiss();
            if (splashFX) Instantiate(splashFX, transform.position, Quaternion.identity);
            if (missSound) AudioSource.PlayClipAtPoint(missSound, transform.position);
        }
        Deactivate();
    }

    void Deactivate()
    {
        if (myPool != null)
        {
            myPool.Return(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}