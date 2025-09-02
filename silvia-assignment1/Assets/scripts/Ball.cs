using UnityEngine;

public class Ball : MonoBehaviour
{
    public float initialSpeed = 8f;
    public float speedUpOnHit = 0.5f;
    public float maxSpeed = 16f;
    public float randomAngle = 0.3f;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        ResetAndLaunch(Random.value < 0.5f ? Vector2.left : Vector2.right);
    }

    public void ResetAndLaunch(Vector2 dir)
    {
        transform.position = Vector3.zero;
        rb.linearVelocity = Vector2.zero;
        dir.y = Random.Range(-0.5f, 0.5f);
        rb.linearVelocity = dir.normalized * initialSpeed;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Vector2 v = rb.linearVelocity;
        v.y += Random.Range(-randomAngle, randomAngle);

        {
            float newSpeed = Mathf.Min(v.magnitude + speedUpOnHit, maxSpeed);
            v = v.normalized * newSpeed;
            float offset = transform.position.y - col.transform.position.y;
            v.y += offset * 1.2f;
        }

        rb.linearVelocity = v.normalized * Mathf.Clamp(v.magnitude, initialSpeed, maxSpeed);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name.Contains("LeftGoal"))
        {
            GameManager.Instance.AddScoreRight();
            ResetAndLaunch(Vector2.left);
        }
        else if (other.name.Contains("RightGoal"))
        {
            GameManager.Instance.AddScoreLeft();
            ResetAndLaunch(Vector2.right);
        }
    }
}
