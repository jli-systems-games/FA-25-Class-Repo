using UnityEngine;
using System.Reflection;

[RequireComponent(typeof(AudioSource))]
public class PlayerSfx : MonoBehaviour
{
    [Header("Footstep")]
    public AudioClip[] footstepClips;
    [Tooltip("走路脚步声间隔（秒）")]
    public float stepInterval = 0.45f;
    [Tooltip("触发脚步声的最小速度")]
    public float minSpeedForStep = 0.2f;

    [Header("Ground Check（优先用你现有的 FPC）")]
    [Tooltip("把你的 FirstPersonController 脚本拖进来（可选）")]
    public Component groundedSource;                 // 你的FPC脚本（可选）
    [Tooltip("该脚本里代表着地的布尔名，如 Grounded / isGrounded / IsGrounded")]
    public string groundedMember = "Grounded";      // 对应布尔字段/属性名
    [Tooltip("如果上面没配成功，就用这个球形检测兜底")]
    public Transform groundCheck;                   // 脚底点（兜底用）
    public float groundCheckRadius = 0.25f;
    public LayerMask groundMask = ~0;

    [Header("Actions")]
    public AudioClip jumpClip;
    public AudioClip landClip;
    public AudioClip photoClip;
    public AudioClip fallClip;

    AudioSource src;
    Rigidbody rb;
    CharacterController cc;

    float stepTimer;
    bool wasGrounded = true;
    Vector3 lastPos;

    // 反射缓存
    FieldInfo groundedField;
    PropertyInfo groundedProp;

    void Awake()
    {
        src = GetComponent<AudioSource>();
        src.playOnAwake = false;
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CharacterController>();
        lastPos = transform.position;
        if (!groundCheck) groundCheck = transform; // 兜底：没指定就用自身

        // 如果有拖入 groundedSource，则尝试缓存 Grounded 字段/属性
        if (groundedSource)
        {
            var t = groundedSource.GetType();
            groundedField = t.GetField(groundedMember, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            groundedProp = t.GetProperty(groundedMember, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
    }

    void Update()
    {
        // ① 按 F 的瞬间就播放“拍照”音效
        if (Input.GetKeyDown(KeyCode.F) && photoClip)
            src.PlayOneShot(photoClip);

        bool grounded = IsGrounded();

        // 落地音
        if (!wasGrounded && grounded && landClip)
            src.PlayOneShot(landClip);
        wasGrounded = grounded;

        // ② 只有“确认着地”时才可能有脚步声
        float speed = GetSpeed();
        if (grounded && speed > minSpeedForStep && footstepClips != null && footstepClips.Length > 0)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                stepTimer = stepInterval;
                var clip = footstepClips[Random.Range(0, footstepClips.Length)];
                src.PlayOneShot(clip);
            }
        }
        else
        {
            // 停下时快速复位，让再次移动能很快踩出第一步
            stepTimer = 0.05f;
        }

        // 跳跃键音效（你自己控制的跳跃逻辑触发，这里仅在地面且按空格时播）
        if (Input.GetKeyDown(KeyCode.Space) && grounded && jumpClip)
            src.PlayOneShot(jumpClip);

        lastPos = transform.position;
    }

    bool IsGrounded()
    {
        // 1) 优先：读你FPC脚本里的 Grounded / isGrounded / IsGrounded（bool）
        if (groundedSource)
        {
            if (groundedField != null && groundedField.FieldType == typeof(bool))
                return (bool)groundedField.GetValue(groundedSource);
            if (groundedProp != null && groundedProp.PropertyType == typeof(bool))
                return (bool)groundedProp.GetValue(groundedSource);
        }

        // 2) 其次：CharacterController 的 isGrounded
        if (cc) return cc.isGrounded;

        // 3) 兜底：球形检测
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);
    }

    float GetSpeed()
    {
        if (rb) return rb.linearVelocity.magnitude; // 修正：Rigidbody 没有 linearVelocity
        return (transform.position - lastPos).magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
    }

    // 给别的脚本直接调用
    public void PlayPhoto() { if (photoClip) src.PlayOneShot(photoClip); }
    public void PlayFall() { if (fallClip) src.PlayOneShot(fallClip); }
}
