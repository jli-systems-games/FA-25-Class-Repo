using UnityEngine;

public class PeeProbe : MonoBehaviour
{
    public float life = 2.0f;
    float t;

    void OnEnable() { t = life; }

    void Update()
    {
        t -= Time.deltaTime;
        if (t <= 0f) Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bowl"))
        {
            GameManager.I?.RegisterHit();
            Destroy(gameObject);
        }
        else if (other.CompareTag("MissZone"))
        {
            Destroy(gameObject);
        }
    }
}
