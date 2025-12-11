using UnityEngine;
using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic; // [新增] 需要这个来使用 Dictionary

public class PlayerSlowController : MonoBehaviour
{
    [Header("视觉效果")]
    public Color SlowEffectColor = new Color(0.6f, 0.8f, 0.2f, 1.0f); // [新增] 默认: 史莱姆绿

    private Character _character;
    private CharacterMovement _characterMovement;
    private CharacterRun _characterRun;
    private Coroutine _slowCoroutine;

    // 用来记录原始速度
    private float _initialWalkSpeed;
    private float _initialRunSpeed;

    // [新增] 记录颜色的字典
    private Dictionary<Renderer, Color> _originalColors = new Dictionary<Renderer, Color>();
    private bool _hasInitializedColors = false;

    void Awake()
    {
        _character = GetComponent<Character>();
        _characterMovement = GetComponent<CharacterMovement>();
        _characterRun = GetComponent<CharacterRun>();

        if (_characterMovement != null)
        {
            _initialWalkSpeed = _characterMovement.WalkSpeed;

            // 尝试获取跑步速度
            if (_characterRun != null)
            {
                _initialRunSpeed = _characterRun.RunSpeed;
                Debug.Log($"✅ {name} 减速模块就绪 (含跑步)。Walk: {_initialWalkSpeed}, Run: {_initialRunSpeed}");
            }
            else
            {
                // 如果没有跑步组件，就让跑速等于走速，防止计算出错
                _initialRunSpeed = _initialWalkSpeed;
                Debug.Log($"✅ {name} 减速模块就绪 (仅走路)。Walk: {_initialWalkSpeed}");
            }
        }
        else
        {
            Debug.LogError($"❌ {name} 身上找不到 CharacterMovement 组件！减速将无效。");
        }
    }

    // [新增] 在 Start 中记录原始颜色，防止记录到被冻结后的蓝色
    void Start()
    {
        InitializeColors();
    }

    public void ApplySlow(float duration, float slowFactor)
    {
        if (_characterMovement == null) return;

        // 如果已经在减速中，先停止之前的协程，并立即恢复速度（这也包含恢复颜色）
        if (_slowCoroutine != null)
        {
            StopCoroutine(_slowCoroutine);
            ResetSpeed();
        }

        _slowCoroutine = StartCoroutine(SlowRoutine(duration, slowFactor));
    }

    IEnumerator SlowRoutine(float duration, float factor)
    {
        // 1. 计算目标速度
        float newWalkSpeed = _initialWalkSpeed * factor;
        float newRunSpeed = (_characterRun != null) ? _initialRunSpeed * factor : newWalkSpeed;

        // 2. 修改配置参数 (这只影响下一次状态切换)
        _characterMovement.WalkSpeed = newWalkSpeed;
        if (_characterRun != null)
        {
            _characterRun.RunSpeed = newRunSpeed;
        }

        // 3. 强制立即应用当前速度
        ForceUpdateCurrentSpeed(newWalkSpeed, newRunSpeed);

        // [新增] 变色！变成恶心的绿色
        SetColor(SlowEffectColor);

        Debug.Log($"🐢 {name} 全面减速生效! Walk: {newWalkSpeed}, Run: {newRunSpeed}");

        // 4. 等待持续时间
        yield return new WaitForSeconds(duration);

        // 5. 恢复
        ResetSpeed();
        _slowCoroutine = null;
        Debug.Log($"🐇 {name} 速度恢复正常!");
    }

    void ResetSpeed()
    {
        // 恢复参数配置
        if (_characterMovement != null)
        {
            _characterMovement.WalkSpeed = _initialWalkSpeed;
        }

        if (_characterRun != null)
        {
            _characterRun.RunSpeed = _initialRunSpeed;
        }

        // 恢复时也强制刷新一次
        ForceUpdateCurrentSpeed(_initialWalkSpeed, _initialRunSpeed);

        // [新增] 恢复原始颜色
        RestoreOriginalColor();
    }

    // --- 辅助方法 ---
    void ForceUpdateCurrentSpeed(float targetWalk, float targetRun)
    {
        if (_character == null || _characterMovement == null) return;

        // 改为判断 Character 的当前状态是否为 "Running"
        if (_character.MovementState.CurrentState == CharacterStates.MovementStates.Running)
        {
            // 如果正在跑，强行更新为新的跑步速度
            _characterMovement.MovementSpeed = targetRun;
        }
        else
        {
            // 否则 (走路、发呆等)，更新为走路速度
            _characterMovement.MovementSpeed = targetWalk;
        }
    }

    // --- [新增] 颜色处理逻辑 (照搬 FreezeController) ---
    void InitializeColors()
    {
        if (_hasInitializedColors) return;

        // 优先从 CharacterModel 找渲染器，如果没配置就从 transform 找
        Transform target = (_character != null && _character.CharacterModel != null) ? _character.CharacterModel.transform : transform;
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();

        foreach (var r in renderers)
        {
            // 兼容普通材质(_Color)和URP材质(_BaseColor)
            if (r.material.HasProperty("_Color"))
            {
                _originalColors[r] = r.material.color;
            }
            else if (r.material.HasProperty("_BaseColor"))
            {
                _originalColors[r] = r.material.GetColor("_BaseColor");
            }
        }
        _hasInitializedColors = true;
    }

    void SetColor(Color c)
    {
        // 确保颜色已经初始化
        if (!_hasInitializedColors) InitializeColors();

        foreach (var kvp in _originalColors)
        {
            if (kvp.Key != null)
            {
                if (kvp.Key.material.HasProperty("_Color")) kvp.Key.material.color = c;
                if (kvp.Key.material.HasProperty("_BaseColor")) kvp.Key.material.SetColor("_BaseColor", c);
            }
        }
    }

    void RestoreOriginalColor()
    {
        foreach (var kvp in _originalColors)
        {
            if (kvp.Key != null)
            {
                if (kvp.Key.material.HasProperty("_Color")) kvp.Key.material.color = kvp.Value;
                if (kvp.Key.material.HasProperty("_BaseColor")) kvp.Key.material.SetColor("_BaseColor", kvp.Value);
            }
        }
    }
}