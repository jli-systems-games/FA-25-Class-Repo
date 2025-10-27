using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 一站式：点击移动 + 朝向修正（仅绕Y轴）+ 平滑喂Animator的Speed，杜绝闪烁
/// 默认用 CharacterController；也支持 NavMeshAgent/Transform 模式
/// </summary>
[DisallowMultipleComponent]
public class ClickMoveAnimatorDriver : MonoBehaviour
{
    public enum MoveMode { CharacterController, NavMeshAgent, TransformAdditive }

    [Header("References")]
    public Camera cam;
    public Animator animator;
    public LayerMask groundMask;

    [Header("Movement Mode")]
    public MoveMode moveMode = MoveMode.CharacterController;
    public CharacterController characterController;   // 选 CharacterController 模式时需要
    public NavMeshAgent agent;                       // 选 NavMesh 模式时需要

    [Header("Movement")]
    public float moveSpeed = 3.2f;       // 非RootMotion情况下前进速度
    public float turnSpeed = 9f;         // 旋转速度（越大越快）
    public float stopDistance = 0.12f;   // 到达阈值
    public float gravity = -18f;         // 给CC用的重力

    [Header("Animator")]
    public string speedParam = "Speed";  // 只用一个参数就能稳住切换
    public float speedDampTime = 0.12f;  // Animator参数阻尼，防抖
    public float speedNormalizeCap = 6f; // 归一化上限（速度超过这个不再增长）

    [Header("Click")]
    public float rayMaxDistance = 200f;
    public KeyCode clickKey = KeyCode.Mouse0;

    // 内部状态
    private Vector3 _target;
    private bool _hasTarget;
    private float _verticalVel;          // 给CC用的垂直速度
    private int _hashSpeed;

    void Reset()
    {
        cam = Camera.main;
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        // Ground 默认Everything，使用前请在Inspector里改为你的地面Layer
        groundMask = ~0;
    }

    void Awake()
    {
        if (!cam) cam = Camera.main;
        _hashSpeed = Animator.StringToHash(speedParam);

        if (moveMode == MoveMode.NavMeshAgent && agent != null)
        {
            agent.updateRotation = false; // 旋转我们自己管
            agent.updatePosition = true;
        }
    }

    void Update()
    {
        HandleClick();

        // 根据模式驱动位移/旋转，并得到“当前水平速度”
        Vector3 horizVel = Vector3.zero;

        switch (moveMode)
        {
            case MoveMode.CharacterController:
                horizVel = TickCharacterController();
                break;
            case MoveMode.TransformAdditive:
                horizVel = TickTransformTranslate();
                break;
            case MoveMode.NavMeshAgent:
                horizVel = TickNavMesh();
                break;
        }

        // 平滑喂 Animator 的 Speed，避免 0↔正数 抖动导致闪烁
        float s = Mathf.Min(horizVel.magnitude, speedNormalizeCap);
        animator.SetFloat(_hashSpeed, s, speedDampTime, Time.deltaTime);
    }

    // --- 点击取目标 ---
    void HandleClick()
    {
        if (Input.GetKeyDown(clickKey))
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out var hit, rayMaxDistance, groundMask))
            {
                _target = hit.point;
                _hasTarget = true;

                if (moveMode == MoveMode.NavMeshAgent && agent != null)
                {
                    agent.SetDestination(_target);
                }
            }
        }
    }

    // --- CharacterController 驱动 ---
    Vector3 TickCharacterController()
    {
        if (characterController == null) return Vector3.zero;

        Vector3 pos = transform.position;
        Vector3 to = _target - pos; to.y = 0f;
        float dist = to.magnitude;

        bool moving = _hasTarget && dist > stopDistance;

        // 水平旋转（仅绕Y）
        if (moving)
        {
            Quaternion rot = Quaternion.LookRotation(to.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * Time.deltaTime);
        }

        // 水平前进（不使用RootMotion时）
        Vector3 horiz = Vector3.zero;
        if (moving)
        {
            horiz = transform.forward * moveSpeed;
        }
        else
        {
            _hasTarget = false;
        }

        // 垂直重力
        if (characterController.isGrounded && _verticalVel < 0f)
            _verticalVel = -1f; // 贴地
        _verticalVel += gravity * Time.deltaTime;

        Vector3 velocity = new Vector3(horiz.x, _verticalVel, horiz.z);
        characterController.Move(velocity * Time.deltaTime);

        // 返回“水平速度”给 Animator 使用
        return horiz;
    }

    // --- 直接改 Transform 的简单模式 ---
    Vector3 TickTransformTranslate()
    {
        Vector3 pos = transform.position;
        Vector3 to = _target - pos; to.y = 0f;
        float dist = to.magnitude;

        bool moving = _hasTarget && dist > stopDistance;

        if (moving)
        {
            Quaternion rot = Quaternion.LookRotation(to.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * Time.deltaTime);

            Vector3 step = transform.forward * moveSpeed * Time.deltaTime;
            if (step.magnitude > dist) step = to.normalized * dist;
            transform.position += step;

            return step / Time.deltaTime; // 水平速度
        }
        else
        {
            _hasTarget = false;
            return Vector3.zero;
        }
    }

    // --- NavMeshAgent 驱动 ---
    Vector3 TickNavMesh()
    {
        if (agent == null) return Vector3.zero;

        bool moving = agent.hasPath && !agent.pathPending && agent.remainingDistance > stopDistance;

        // 平滑朝向 steeringTarget / desiredVelocity
        if (moving)
        {
            Vector3 dir = (agent.steeringTarget - transform.position);
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f) dir = agent.desiredVelocity;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.0001f)
            {
                Quaternion rot = Quaternion.LookRotation(dir.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * Time.deltaTime);
            }
        }

        if (!moving && _hasTarget && agent.remainingDistance <= stopDistance)
            _hasTarget = false;

        return new Vector3(agent.velocity.x, 0f, agent.velocity.z);
    }

    // —— 可选：对外暴露一次性动作（避免你再用 Play/CrossFade 乱跳）——
    public void TriggerDie(string dieTrigger = "DieTrigger")
    {
        animator.ResetTrigger(dieTrigger);
        animator.SetTrigger(dieTrigger);
    }

    public void TriggerRevive(string reviveTrigger = "ReviveTrigger")
    {
        animator.ResetTrigger(reviveTrigger);
        animator.SetTrigger(reviveTrigger);
    }
}
