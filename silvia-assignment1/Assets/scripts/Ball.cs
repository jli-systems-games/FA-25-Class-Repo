using UnityEngine;

public class Ball : MonoBehaviour
{
    public float initialSpeed = 8f;
    public float speedUpOnHit = 0.5f;
    public float maxSpeed = 16f;
    public float randomAngle = 0.3f;

    Rigidbody2D rb;
    Transform tf; 

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        tf = transform;
    }

    public void ResetToCenter()
    {
        transform.position = Vector3.zero;
        var rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
    }

    public void Launch(Vector2 dir)
    {
        ResetToCenter();
        dir.y = Random.Range(-0.5f, 0.5f);
        GetComponent<Rigidbody2D>().linearVelocity = dir.normalized * initialSpeed;
    }

    public void LaunchRandom()
    {
        Launch(Random.value < 0.5f ? Vector2.left : Vector2.right);
    }

    public void ResetAndLaunch(Vector2 dir)
    {
        tf.position = Vector3.zero;
        rb.linearVelocity = Vector2.zero;

        //if its zero make it move randomly
        if (ApproximatelyZero(dir))
            dir = Random.value < 0.5f ? Vector2.left : Vector2.right;

        // a bit vertical
        dir.y = Random.Range(-0.5f, 0.5f);

        rb.linearVelocity = dir.normalized * initialSpeed;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Vector2 v = rb.linearVelocity;

        // tremble
        v.y += Random.Range(-randomAngle, randomAngle);

        float offset = tf.position.y - col.transform.position.y;
        v.y += offset * 1.2f;

        // acclerate
        float newSpeed = Mathf.Min(v.magnitude + speedUpOnHit, maxSpeed);
        v = v.sqrMagnitude > 0.0001f ? v.normalized * newSpeed : v;

        // avoid horizontal moves
        v = EnsureMinVertical(v, 0.001f);

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

    static bool ApproximatelyZero(Vector2 v)
    {
        return Mathf.Approximately(v.x, 0f) && Mathf.Approximately(v.y, 0f);
    }

    static Vector2 EnsureMinVertical(Vector2 v, float minYAbs)
    {
        // avoid moving horizontally
        if (Mathf.Abs(v.y) < minYAbs)
        {
            float sign = (Random.value < 0.5f) ? -1f : 1f;
            v.y = sign * minYAbs;
            v = v.normalized * Mathf.Max(v.magnitude, minYAbs);
        }
        return v;
    }
}
