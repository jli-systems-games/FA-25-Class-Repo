using UnityEngine;

public class ball : MonoBehaviour
{
    public float speed = 8f;
    private Rigidbody2D rb;
    public Vector3 startPosition;
    public AudioSource HitWallSound;
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
        
        if (HitWallSound != null)
            HitWallSound.Play();

 
    }
    
}
