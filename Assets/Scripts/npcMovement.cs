using UnityEngine;

public class npcMovement : MonoBehaviour
{
    public float leftBound = -5f;
    public float rightBound = 5f;
    public float speed = 2f;
    public float waitTime = 1f;
    private Rigidbody rb;
    private float targetX;
    private bool waiting;
    private float waitTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        PickNewTarget();
    }

    void FixedUpdate()
    {
        if (waiting)
        {
            waitTimer -= Time.fixedDeltaTime;
            if (waitTimer <= 0f)
            {
                waiting = false;
                PickNewTarget();
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
            return;
        }

        float dir = Mathf.Sign(targetX - transform.position.x);
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);

        if (Mathf.Abs(transform.position.x - targetX) < 0.1f)
        {
            waiting = true;
            waitTimer = waitTime;
        }

        if (dir != 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * dir, transform.localScale.y, transform.localScale.z);
        }
    }

    void PickNewTarget()
    {
        targetX = Random.Range(leftBound, rightBound);
    }
}