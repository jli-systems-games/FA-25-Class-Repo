using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class BallController : MonoBehaviour
{

    [Header("Refs")]
    public Transform paddle;                 // 拍子（可不填，仅作回退定位）
    public Transform ballSpawnPoint;         // 球出生点（放高一点，让它自然掉下）
    public TMP_Text counterText;             // 颠球次数显示（TMP）
    // public Image resultImage;                // 结果图

    [Header("Rules")]
    public int requiredBounces = 5;          // 目标次数
    public float minValidYSpeed = 1.0f;      // 过滤“刮擦式”接触（向上速度阈值）

    [Header("Scene")]
    public string nextSceneName; // 胜利后跳转的下一个场景名
    public string failSceneName; // 失败后跳转的场景名

    [Header("Audio")]
    public AudioSource audioSource;          // 播放音效
    public AudioClip bounceClip;             // 弹起音效

    private Rigidbody2D rb;
    private int currentBounces = 0;
    private bool gameEnded = false;

    // 防抖：避免一次碰撞多次计数
    private float lastHitTime = -999f;
    private const float hitCooldown = 0.05f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        if (!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
            if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void Start()
    {
        ResetRound();
    }

    public void ResetRound()
    {
        gameEnded = false;
        currentBounces = 0;
        lastHitTime = -999f;

        UpdateCounterUI();

        // 只定位，不施加任何力；让重力自然作用
        if (ballSpawnPoint != null)
            transform.position = ballSpawnPoint.position;
        else if (paddle != null)
            transform.position = paddle.position + Vector3.up * 1.5f;

        rb.linearVelocity = Vector2.zero; // 干净起步
    }

    // 用 Exit 检测：保证已经完成一次反弹，再看“向上速度”
    void OnCollisionExit2D(Collision2D col)
    {
        if (gameEnded) return;
        if (!col.collider.CompareTag("Paddle")) return;

        // 防抖，避免一次接触产生多次 Exit 事件
        if (Time.time - lastHitTime < hitCooldown) return;
        lastHitTime = Time.time;

        // 只有在“离开拍子后速度向上且足够快”才计数
        if (rb.linearVelocity.y >= minValidYSpeed)
        {
            currentBounces++;
            UpdateCounterUI();

            // 播放弹起音效
            if (audioSource && bounceClip)
            {
                audioSource.PlayOneShot(bounceClip);
            }

            if (currentBounces >= requiredBounces)
            {
                Win();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (gameEnded) return;

        if (other.CompareTag("Floor"))
        {
            Lose();
        }
    }

    void Win()
    {
        gameEnded = true;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("成功：达到要求的颠球次数。");

        // 跳转到下一个场景
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    void Lose()
    {
        gameEnded = true;
        Debug.Log("失败：球触地。");
        // 跳转到失败场景
        if (!string.IsNullOrEmpty(failSceneName))
        {
            SceneManager.LoadScene(failSceneName);
        }
    }

    void UpdateCounterUI()
    {
        if (counterText)
        {
            counterText.text = $"{currentBounces}";
        }
    }

    // 可供外部 UI 调整
    public void SetRequiredBounces(int n)
    {
        requiredBounces = Mathf.Max(1, n);
        UpdateCounterUI();
    }
}