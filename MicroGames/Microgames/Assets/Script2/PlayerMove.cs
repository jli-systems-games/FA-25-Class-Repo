using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("移动/跳跃")]
    public float moveSpeed = 5f;     // 左右移动速度
    public float jumpForce = 12f;    // 跳跃冲量（注意：是 Impulse）

    [Header("地面检测（两种方案，任意一种就行）")]
    public Transform groundCheck;    // 选用：放在脚底的空物体
    public float groundRadius = 0.15f;
    public LayerMask groundLayer;    // 把地面的 Layer 勾上
    // 如果不设置 groundCheck，会退化为用自身 Collider 与 groundLayer 的接触判断

    Rigidbody2D rb;
    Collider2D col;
    bool grounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        if (!rb) Debug.LogError("需要 Rigidbody2D");
        if (!col) Debug.LogError("需要 Collider2D（非 Trigger）");
    }

    void Update()
    {
        // 左右移动
        float x = Input.GetAxisRaw("Horizontal"); // A/D 或 ←/→
        if (rb) rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);

        // 跳跃（只在落地时）
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            // 先清掉下落速度，再给向上冲量，手感更稳定
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void FixedUpdate()
    {
        grounded = IsGrounded();
    }

    bool IsGrounded()
    {
        // 方案 A：脚底圆形检测
        if (groundCheck)
        {
            return Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        }

        // 方案 B：不用 groundCheck，就看自身 Collider 是否接触 groundLayer
        if (col)
        {
            return col.IsTouchingLayers(groundLayer);
        }

        return false;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }
}
