using UnityEngine;

public class BallCollision : MonoBehaviour
{
    public PongGame game;   
    private Rigidbody2D rb;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject hit = collision.collider.gameObject;

        
        if (hit == game.topWall || hit == game.bottomWall)
        {
            Vector2 v = rb.linearVelocity;
            v.y = -v.y;
            rb.linearVelocity = v;
            return;
        }

        
        if (hit == game.leftPaddle || hit == game.rightPaddle)
        {
            
            float offsetY = transform.position.y - collision.collider.bounds.center.y;
            float norm = Mathf.Clamp(offsetY / collision.collider.bounds.extents.y, -1f, 1f);

            Vector2 v = rb.linearVelocity;
            float speed = v.magnitude;

            
            v.x = -v.x;
            v.y = v.y + norm * game.ballSpeed * game.verticalInfluence;

            
            v = v.normalized * speed * 1.1f;
            if (Mathf.Abs(v.y) < 0.5f) v.y = Mathf.Sign(v.y == 0 ? Random.Range(-1f, 1f) : v.y) * 0.5f;

            rb.linearVelocity = v;
        }
    }

    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == game.leftGoal)     
        {
            game.AddRightScore();
            game.ResetAndStartNewRound();
        }
        else if (other.gameObject == game.rightGoal) 
        {
            game.AddLeftScore();
            game.ResetAndStartNewRound();
        }
    }
}
