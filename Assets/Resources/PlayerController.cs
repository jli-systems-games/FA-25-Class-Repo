using UnityEngine;

/// <summary>
/// 玩家控制器 - 处理玩家的移动、跳跃和动画
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;

    [Header("跳跃设置")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private int maxJumps = 2; // 允许二段跳
    
    [Header("地面检测")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    
    [Header("墙面检测")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance = 0.5f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private float wallJumpForce = 10f;
    [SerializeField] private Vector2 wallJumpDirection = new Vector2(1f, 2f);

    // 组件引用
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // 状态变量
    private float horizontalInput;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private int jumpCount;
    private float currentSpeed;
    private bool isSprinting;
    private bool isCrouching;

    // 动画参数名称
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int VerticalVelocity = Animator.StringToHash("VerticalVelocity");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int IsSprinting = Animator.StringToHash("IsSprinting");
    private static readonly int IsCrouching = Animator.StringToHash("IsCrouching");
    private static readonly int IsWallSliding = Animator.StringToHash("IsWallSliding");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // 如果没有设置 groundCheck，自动创建
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.parent = transform;
            groundCheckObj.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheck = groundCheckObj.transform;
        }

        // 如果没有设置 wallCheck，自动创建
        if (wallCheck == null)
        {
            GameObject wallCheckObj = new GameObject("WallCheck");
            wallCheckObj.transform.parent = transform;
            wallCheckObj.transform.localPosition = new Vector3(0.3f, 0, 0);
            wallCheck = wallCheckObj.transform;
        }
    }

    private void Update()
    {
        HandleInput();
        CheckGrounded();
        CheckWallContact();
        HandleWallSlide();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        ApplyJumpPhysics();
    }

    /// <summary>
    /// 处理玩家输入
    /// </summary>
    private void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        
        // 冲刺
        isSprinting = Input.GetKey(KeyCode.LeftShift);
        
        // 蹲下
        isCrouching = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C);
        
        // 跳跃
        if (Input.GetButtonDown("Jump"))
        {
            HandleJump();
        }
    }

    /// <summary>
    /// 检测是否在地面上
    /// </summary>
    private void CheckGrounded()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
        // 如果刚落地，重置跳跃次数
        if (!wasGrounded && isGrounded)
        {
            jumpCount = 0;
        }
    }

    /// <summary>
    /// 检测是否接触墙面
    /// </summary>
    private void CheckWallContact()
    {
        isTouchingWall = Physics2D.Raycast(wallCheck.position, Vector2.right * transform.localScale.x, 
                                           wallCheckDistance, wallLayer);
    }

    /// <summary>
    /// 处理墙面滑行
    /// </summary>
    private void HandleWallSlide()
    {
        if (isTouchingWall && !isGrounded && rb.linearVelocity.y < 0 && horizontalInput != 0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlideSpeed, float.MaxValue));
            jumpCount = 0; // 重置跳跃次数，允许蹬墙跳
        }
        else
        {
            isWallSliding = false;
        }
    }

    /// <summary>
    /// 处理水平移动
    /// </summary>
    private void HandleMovement()
    {
        if (isCrouching && isGrounded)
        {
            // 蹲下时减速
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            float targetSpeed = isSprinting ? sprintSpeed : moveSpeed;
            float speedChange = horizontalInput != 0 ? acceleration : deceleration;
            
            currentSpeed = Mathf.MoveTowards(currentSpeed, 
                                            horizontalInput * targetSpeed, 
                                            speedChange * Time.fixedDeltaTime);
        }

        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);

        // 翻转角色朝向
        if (horizontalInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    /// <summary>
    /// 处理跳跃
    /// </summary>
    private void HandleJump()
    {
        if (isWallSliding)
        {
            // 蹬墙跳
            Vector2 wallJump = new Vector2(wallJumpDirection.x * -transform.localScale.x, wallJumpDirection.y);
            rb.linearVelocity = new Vector2(wallJump.x * wallJumpForce, wallJump.y);
            jumpCount = 0;
            
            // 翻转角色朝向
            transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
            
            animator.SetTrigger(Jump);
        }
        else if (isGrounded || jumpCount < maxJumps)
        {
            // 普通跳跃或二段跳
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount++;
            animator.SetTrigger(Jump);
        }
    }

    /// <summary>
    /// 应用跳跃物理（更好的跳跃手感）
    /// </summary>
    private void ApplyJumpPhysics()
    {
        if (rb.linearVelocity.y < 0)
        {
            // 下落时增加重力
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            // 松开跳跃键时降低跳跃高度
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    /// <summary>
    /// 更新动画参数
    /// </summary>
    private void UpdateAnimations()
    {
        animator.SetFloat(Speed, Mathf.Abs(currentSpeed));
        animator.SetBool(IsGrounded, isGrounded);
        animator.SetFloat(VerticalVelocity, rb.linearVelocity.y);
        animator.SetBool(IsSprinting, isSprinting && Mathf.Abs(currentSpeed) > moveSpeed * 0.9f);
        animator.SetBool(IsCrouching, isCrouching && isGrounded);
        animator.SetBool(IsWallSliding, isWallSliding);
    }

    /// <summary>
    /// 在编辑器中绘制调试信息
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (wallCheck != null)
        {
            Gizmos.color = isTouchingWall ? Color.green : Color.red;
            Gizmos.DrawLine(wallCheck.position, 
                          wallCheck.position + Vector3.right * transform.localScale.x * wallCheckDistance);
        }
    }
}

