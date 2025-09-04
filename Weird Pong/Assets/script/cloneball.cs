using UnityEngine;

public class CloningBall : MonoBehaviour
{
    public float speed = 8f;
    private Rigidbody2D rb;
    private Vector3 startPosition;

    [Header("Cloning")]
    public GameObject ballPrefab; 
    public int clonesPerHit = 1;  

    void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        LaunchBall();
    }

    public void Reset()
    {
        rb.linearVelocity = Vector2.zero;
        transform.position = startPosition;
        LaunchBall();
    }

    void LaunchBall()
    {
        float x = Random.Range(0, 2) == 0 ? -1 : 1;
        float y = Random.Range(-1f, 1f);
        Vector2 dir = new Vector2(x, y).normalized;
        rb.linearVelocity = dir * speed;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            CloneBalls();
        }
    }

    void CloneBalls()
    {
        if (ballPrefab == null) return;

        for (int i = 0; i < clonesPerHit; i++)
        {
            
            GameObject clone = Instantiate(ballPrefab, transform.position, Quaternion.identity);

           
            Rigidbody2D cloneRb = clone.GetComponent<Rigidbody2D>();
            if (cloneRb != null)
            {
                float x = Random.Range(0, 2) == 0 ? -1 : 1;
                float y = Random.Range(-1f, 1f);
                Vector2 dir = new Vector2(x, y).normalized;
                cloneRb.linearVelocity = dir * speed;
            }
        }
    }
}
