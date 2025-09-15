using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class PaddleClickController : MonoBehaviour
{
    [Header("Settings")]
    public float activeTime = 0.2f;       // 激活持续时间
    public KeyCode triggerKey = KeyCode.Space;

    [Header("Effect")]
    public Color activeColor = Color.red; // 激活时颜色
    public float punchScale = 1.2f;       // 激活时缩放倍数
    public float scaleLerpSpeed = 8f;     // 缩放恢复速度

    private Collider2D col;
    private SpriteRenderer sr;
    private Color originalColor;
    private Vector3 originalScale;

    private bool isActive = false;
    private float timer = 0f;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        col.enabled = false; // 默认关闭
        originalColor = sr.color;
        originalScale = transform.localScale;
    }

    void Update()
    {
        // 空格触发
        if (Input.GetKeyDown(triggerKey))
        {
            ActivatePaddle();
        }

        // 计时
        if (isActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                DeactivatePaddle();
            }
        }

        // 缩放平滑恢复
        transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * scaleLerpSpeed);
    }

    void ActivatePaddle()
    {
        isActive = true;
        timer = activeTime;
        col.enabled = true;

        // 视觉效果
        sr.color = activeColor;
        transform.localScale = originalScale * punchScale;
    }

    void DeactivatePaddle()
    {
        isActive = false;
        col.enabled = false;

        // 恢复颜色
        sr.color = originalColor;
    }
}