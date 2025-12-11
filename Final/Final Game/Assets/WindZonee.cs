using UnityEngine;
using MoreMountains.TopDownEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class AreaWindZone : MonoBehaviour
{
    [Header("状态控制")]
    public bool IsActive = true;

    [Header("风力推力 (物理推人)")]
    [Tooltip("风把人推开的物理力度")]
    public float PushForce = 80f;
    [Tooltip("风吹向哪里？(相对于这个区域的朝向，默认向前)")]
    public Vector3 WindDirection = Vector3.forward;

    [Header("顶风减速 (自然手感)")]
    [Range(0.1f, 1f)]
    [Tooltip("当玩家正对着风移动时，速度降低到多少？(0.5 = 50% 速度, 0.1 = 举步维艰)")]
    public float HeadwindSpeedFactor = 0.5f;

    [Header("视觉效果")]
    public Transform FanBlades;
    public float RotationSpeed = 800f;
    public Vector3 RotationAxis = Vector3.forward;

    [Header("调试")]
    public bool ShowGizmos = true;

    // 记录受影响的玩家，以便离开时恢复速度
    private HashSet<CharacterMovement> _affectedCharacters = new HashSet<CharacterMovement>();

    private void Start()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void Update()
    {
        // 1. 扇叶旋转
        if (IsActive && FanBlades != null)
        {
            FanBlades.Rotate(RotationAxis * RotationSpeed * Time.deltaTime);
        }

        // 2. 安全检查：如果风扇突然关了，必须恢复所有人的速度
        if (!IsActive && _affectedCharacters.Count > 0)
        {
            ResetAllCharacters();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsActive) return;

        // 计算风的世界方向
        Vector3 worldWindDir = transform.TransformDirection(WindDirection).normalized;

        // --- 获取 TDE 核心组件 ---
        TopDownController3D controller = other.GetComponent<TopDownController3D>();
        CharacterMovement move = other.GetComponent<CharacterMovement>();
        Character character = other.GetComponent<Character>(); // 获取 Character 组件

        if (controller != null)
        {
            // A. 物理推力 (站着不动也会被吹跑)
            controller.AddForce(worldWindDir * PushForce * Time.deltaTime);

            // B. 顶风减速逻辑 (修复了之前的报错)
            if (move != null && character != null)
            {
                // 记录玩家，方便离开时恢复
                _affectedCharacters.Add(move);

                // --- 【修复点】使用 LinkedInputManager 获取输入，兼容所有版本 ---
                Vector3 playerInputDir = Vector3.zero;

                if (character.LinkedInputManager != null)
                {
                    // 获取手柄/键盘的原始输入 (Vector2)
                    Vector2 input2D = character.LinkedInputManager.PrimaryMovement;
                    // 转换为 3D 方向 (x, 0, y)
                    playerInputDir = new Vector3(input2D.x, 0f, input2D.y);
                }

                // 计算点积 (Dot Product) 判断方向关系
                // 1.0 = 顺风, 0 = 侧风, -1.0 = 顶风
                float dot = Vector3.Dot(worldWindDir, playerInputDir.normalized);

                float targetMultiplier = 1f;

                // 只有当 dot < 0 (顶风) 时才减速
                if (dot < 0)
                {
                    // 插值计算：越是对着风走，速度越慢
                    // dot = -1 时，Multiplier = HeadwindSpeedFactor
                    targetMultiplier = Mathf.Lerp(1f, HeadwindSpeedFactor, -dot);
                }

                // 应用速度倍率
                move.MovementSpeedMultiplier = targetMultiplier;
            }
        }
        // --- C. 普通物体 (只推) ---
        else if (other.attachedRigidbody != null && !other.attachedRigidbody.isKinematic)
        {
            other.attachedRigidbody.AddForce(worldWindDir * PushForce, ForceMode.Force);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 离开风区，恢复 100% 速度
        CharacterMovement move = other.GetComponent<CharacterMovement>();
        if (move != null && _affectedCharacters.Contains(move))
        {
            move.MovementSpeedMultiplier = 1f;
            _affectedCharacters.Remove(move);
        }
    }

    private void ResetAllCharacters()
    {
        foreach (var move in _affectedCharacters)
        {
            if (move != null)
            {
                move.MovementSpeedMultiplier = 1f;
            }
        }
        _affectedCharacters.Clear();
    }

    // --- 可视化辅助线 ---
    private void OnDrawGizmos()
    {
        if (!ShowGizmos) return;
        BoxCollider box = GetComponent<BoxCollider>();
        if (box == null) return;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = IsActive ? new Color(0, 1f, 1f, 0.2f) : new Color(0.5f, 0.5f, 0.5f, 0.1f);
        Gizmos.DrawCube(box.center, box.size);
        Gizmos.color = IsActive ? new Color(0, 1f, 1f, 0.8f) : Color.gray;
        Gizmos.DrawWireCube(box.center, box.size);

        Gizmos.matrix = Matrix4x4.identity;
        Vector3 dir = transform.TransformDirection(WindDirection).normalized;
        Vector3 center = transform.TransformPoint(box.center);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(center, dir * 3f);
        Gizmos.DrawSphere(center + dir * 3f, 0.2f);
    }
}