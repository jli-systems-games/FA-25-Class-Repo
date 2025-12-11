using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;

public class PlayerExplosionController : MonoBehaviour
{
    [Header("视觉效果")]
    public Color ExplosionEffectColor = new Color(1f, 0.5f, 0f, 1.0f); // 橙色

    [Header("击退设置")]
    public float KnockbackDuration = 0.5f;    // 击退持续时间

    private Character _character;
    private CharacterMovement _characterMovement;
    private Coroutine _explosionCoroutine;

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

    public void ApplyExplosion(Vector3 direction, float force, float slowDuration, float slowFactor)
    {
        if (_explosionCoroutine != null)
        {
            StopCoroutine(_explosionCoroutine);
        }

        _explosionCoroutine = StartCoroutine(ExplosionRoutine(direction, force, slowDuration, slowFactor));
    }

    IEnumerator ExplosionRoutine(Vector3 direction, float force, float slowDuration, float slowFactor)
    {
        Debug.Log($"💥 {name} 被炸飞了！");

        // 1. 变橙色
        SetColor(ExplosionEffectColor);

        // 2. 【关键修改】暂时禁用角色控制，防止输入干扰击退
        bool wasInputAuthorized = false;
        if (_characterMovement != null)
        {
            wasInputAuthorized = _characterMovement.InputAuthorized;
            _characterMovement.InputAuthorized = false;
        }

        // 3. 应用击退（使用TDE的方式）
        yield return StartCoroutine(ApplyKnockbackTDE(direction, force));

        // 4. 恢复输入控制
        if (_characterMovement != null)
        {
            _characterMovement.InputAuthorized = wasInputAuthorized;
        }

        // 5. 立即减速
        if (_characterMovement != null)
        {
            _characterMovement.MovementSpeedMultiplier = slowFactor;
        }

        // 6. 等待减速结束
        yield return new WaitForSeconds(slowDuration);

        // 7. 恢复正常
        ResetEffect();
        _explosionCoroutine = null;
    }

    // 👇 使用TDE的方式实现击退
    IEnumerator ApplyKnockbackTDE(Vector3 direction, float force)
    {
        if (_character == null) yield break;

        float elapsed = 0f;

        // 2D方向
        Vector3 knockbackDirection = new Vector3(direction.x, 0f, direction.z).normalized;

        // 暂时禁用CharacterMovement的更新
        if (_characterMovement != null)
        {
            _characterMovement.enabled = false;
        }

        while (elapsed < KnockbackDuration)
        {
            float t = elapsed / KnockbackDuration;
            float currentForce = Mathf.Lerp(force, 0f, t);

            // 直接移动 transform
            Vector3 movement = knockbackDirection * currentForce * Time.deltaTime;
            _character.transform.position += movement;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 重新启用
        if (_characterMovement != null)
        {
            _characterMovement.enabled = true;
        }
    }

    void ResetEffect()
    {
        if (_characterMovement != null)
        {
            _characterMovement.MovementSpeedMultiplier = 1f;
        }

        RestoreOriginalColor();
        Debug.Log($"✅ {name} 爆炸效果结束");
    }

    // --- 颜色处理 ---
    void InitializeColors()
    {
        if (_hasInitializedColors) return;

        Transform target = (_character != null && _character.CharacterModel != null)
            ? _character.CharacterModel.transform
            : transform;

        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            if (r.material.HasProperty("_Color"))
                _originalColors[r] = r.material.color;
            else if (r.material.HasProperty("_BaseColor"))
                _originalColors[r] = r.material.GetColor("_BaseColor");
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
                if (kvp.Key.material.HasProperty("_Color"))
                    kvp.Key.material.color = c;
                if (kvp.Key.material.HasProperty("_BaseColor"))
                    kvp.Key.material.SetColor("_BaseColor", c);
            }
        }
    }

    void RestoreOriginalColor()
    {
        foreach (var kvp in _originalColors)
        {
            if (kvp.Key != null)
            {
                if (kvp.Key.material.HasProperty("_Color"))
                    kvp.Key.material.color = kvp.Value;
                if (kvp.Key.material.HasProperty("_BaseColor"))
                    kvp.Key.material.SetColor("_BaseColor", kvp.Value);
            }
        }
    }

    public void ForceReset()
    {
        if (_explosionCoroutine != null)
        {
            StopCoroutine(_explosionCoroutine);
            _explosionCoroutine = null;
        }
        ResetEffect();
    }
}