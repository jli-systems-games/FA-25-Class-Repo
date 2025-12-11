using UnityEngine;
using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSlowController : MonoBehaviour
{
    [Header("视觉效果")]
    public Color SlowEffectColor = new Color(0.6f, 0.8f, 0.2f, 1.0f); // 史莱姆绿

    private Character _character;
    private CharacterMovement _characterMovement;
    private Coroutine _slowCoroutine;

    // 记录原始颜色
    private Dictionary<Renderer, Color> _originalColors = new Dictionary<Renderer, Color>();
    private bool _hasInitializedColors = false;

    void Awake()
    {
        _character = GetComponent<Character>();
        _characterMovement = GetComponent<CharacterMovement>();

        if (_characterMovement == null)
        {
            Debug.LogError($"❌ {name} 身上找不到 CharacterMovement！");
        }
    }

    void Start()
    {
        InitializeColors();
    }

    public void ApplySlow(float duration, float slowFactor)
    {
        if (_characterMovement == null) return;

        // 如果已经在减速，先停止之前的，防止多重减速叠加导致动不了
        if (_slowCoroutine != null)
        {
            StopCoroutine(_slowCoroutine);
            // 此时不用 Reset，直接开启新的计时即可，保持减速状态平滑
        }

        _slowCoroutine = StartCoroutine(SlowRoutine(duration, slowFactor));
    }

    IEnumerator SlowRoutine(float duration, float factor)
    {
        // 1. 【核心修改】直接修改全局速度倍率
        // 1.0 = 正常, 0.5 = 半速
        _characterMovement.MovementSpeedMultiplier = factor;

        // 2. 变色
        SetColor(SlowEffectColor);

        Debug.Log($"🐢 {name} 被减速了! 倍率: {factor} (持续 {duration}秒)");

        // 3. 等待
        yield return new WaitForSeconds(duration);

        // 4. 恢复
        ResetSpeed();
        _slowCoroutine = null;
    }

    void ResetSpeed()
    {
        if (_characterMovement != null)
        {
            // 恢复为 1.0 (正常速度)
            _characterMovement.MovementSpeedMultiplier = 1f;
        }

        RestoreOriginalColor();
        Debug.Log($"🐇 {name} 速度倍率恢复正常 (1.0)");
    }

    // --- 颜色处理 (保持不变) ---
    void InitializeColors()
    {
        if (_hasInitializedColors) return;
        Transform target = (_character != null && _character.CharacterModel != null) ? _character.CharacterModel.transform : transform;
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();

        foreach (var r in renderers)
        {
            if (r.material.HasProperty("_Color")) _originalColors[r] = r.material.color;
            else if (r.material.HasProperty("_BaseColor")) _originalColors[r] = r.material.GetColor("_BaseColor");
        }
        _hasInitializedColors = true;
    }

    void SetColor(Color c)
    {
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