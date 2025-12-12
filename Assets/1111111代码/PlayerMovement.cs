using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    private Rigidbody rb;
    private Vector3 moveInput;
    private float currentMoveSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("未找到Rigidbody组件，请添加到物体上");
        }

        currentMoveSpeed = moveSpeed;
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(horizontal, 0, vertical).normalized;

        // 如果有移动输入，就旋转朝向移动方向
        if (moveInput.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 应用红皇后技能的移动速度加成
        float speedMultiplier = PlayerAbilityManager.Instance.GetMoveSpeedMultiplier();
        currentMoveSpeed = moveSpeed * speedMultiplier;
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = moveInput * currentMoveSpeed;
        }
    }

    public float GetMoveSpeed()
    {
        return currentMoveSpeed;
    }

    public float GetBaseMoveSpeed()
    {
        return moveSpeed;
    }
}