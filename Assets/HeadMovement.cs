using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms.Impl;
using TMPro;

public class HeadMovement : MonoBehaviour
{
    public float speed = 3f;
    public Rigidbody2D head;
    public Transform centerPoint;

    public float stuckTime = 0.5f;
    private float stuckTimer = 0f;
    public float attractForce = 5f;
    public float stuckThreshold = 0.05f;

    public float fadeDuration = 0.5f;
    public SpriteRenderer spriteRenderer;

    public TMP_Text score;
    public int scoreVal = 0;

    public Animator animator;
    private bool ifAnimationTriggered=false;

    void Start()
    {
        score.text = "";
        if (head != null)
            head.gravityScale = 0f;

        LaunchHead();
    }

    private void Update()
    {
        Debug.Log(scoreVal);
    }

    void LaunchHead()
    {
        head.linearVelocity = Vector2.zero;
        float x;
        if (Random.value < 0.5f)
        {
            x = -1f;
        }
        else
        {
            x = 1f;
        }

        float y = Random.Range(-1f, 1f);

        Vector2 dir = new Vector2(x, y).normalized;
        head.linearVelocity = dir * speed;
    }

    void FixedUpdate()
    {
        if (head.linearVelocity.magnitude < stuckThreshold)
        {
            stuckTimer += Time.fixedDeltaTime;
            if (stuckTimer >= stuckTime)
            {
                Vector2 toCenter = ((Vector2)centerPoint.position - head.position).normalized;
                head.linearVelocity = toCenter * speed;
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Respawn"))
        {
            scoreVal++;
            ifAnimationTriggered = true;
            if (ifAnimationTriggered && scoreVal != 0)
            {
                animator.SetTrigger("OnAnimation");
            }
            //score.text = scoreVal.ToString();
            RespawnHead();
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            //score.text = "";
            scoreVal = 0;
            RespawnHead();
        }
        else
        {
            Vector2 direction = -head.linearVelocity.normalized;

            speed = Random.Range(2f, 4f);

            head.linearVelocity = direction * speed;
        }
    }

    void RespawnHead()
    {
        StartCoroutine(RespawnCoroutine());
    }
    private IEnumerator RespawnCoroutine()
    {
        Color c = spriteRenderer.color;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            spriteRenderer.color = new Color(c.r, c.g, c.b, Mathf.Lerp(1f, 0f, t / fadeDuration));
            yield return null;
        }
        spriteRenderer.color = new Color(c.r, c.g, c.b, 0f);

        head.transform.position = centerPoint.position;
        head.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(2.5f);

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            spriteRenderer.color = new Color(c.r, c.g, c.b, Mathf.Lerp(0f, 1f, t / fadeDuration));
            yield return null;
        }
        spriteRenderer.color = new Color(c.r, c.g, c.b, 1f);

        LaunchHead();
    }
}