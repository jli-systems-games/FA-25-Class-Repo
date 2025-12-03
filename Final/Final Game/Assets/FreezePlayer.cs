using UnityEngine;
using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerFreezeController : MonoBehaviour
{
    private Character _character;
    private Coroutine _freezeCoroutine;

    // 记录颜色的小本本
    private Dictionary<Renderer, Color> _originalColors = new Dictionary<Renderer, Color>();
    private bool _hasInitializedColors = false; // 是否已经记录过颜色了

    void Start()
    {
        _character = GetComponent<Character>();
        // 注意：把 Start 里的记录逻辑删掉了，移到了后面
    }

    // --- 【核心修改】懒加载初始化 ---
    // 只有在第一次准备变色之前，才去记录原始颜色
    // 这样能确保记录到的是已经加载好的红/蓝衣服，而不是开局的白模
    void InitializeColors()
    {
        if (_hasInitializedColors) return; // 如果记过了就不再记

        if (_character.CharacterModel != null)
        {
            Renderer[] renderers = _character.CharacterModel.GetComponentsInChildren<Renderer>();

            foreach (var r in renderers)
            {
                // 优先尝试获取 _Color (标准), 如果没有再尝试 _BaseColor (URP)
                if (r.material.HasProperty("_Color"))
                {
                    _originalColors[r] = r.material.color;
                }
                else if (r.material.HasProperty("_BaseColor"))
                {
                    _originalColors[r] = r.material.GetColor("_BaseColor");
                }
            }
        }
        _hasInitializedColors = true;
        // Debug.Log($"🎨 {name} 的原始颜色已记录！(数量: {_originalColors.Count})");
    }

    public void ApplyFreeze(float duration)
    {
        // 1. 在冻结之前，先确保我们记住了原本的颜色！
        InitializeColors();

        // 2. 刷新时间逻辑
        if (_freezeCoroutine != null) StopCoroutine(_freezeCoroutine);
        _freezeCoroutine = StartCoroutine(FreezeRoutine(duration));
    }

    IEnumerator FreezeRoutine(float duration)
    {
        // 只有未冻结时才执行变色
        if (_character.ConditionState.CurrentState != CharacterStates.CharacterConditions.Frozen)
        {
            _character.Freeze();

            // [修复报错] 使用 MovementState.ChangeState 替代 SetMovementState
            // 强制停止移动 (双重保险)
            if (_character.MovementState != null)
            {
                _character.MovementState.ChangeState(CharacterStates.MovementStates.Idle);
            }

            // 变成冰蓝色
            SetColor(Color.cyan);
            // Debug.Log($"❄️ {name} 被冻结！");
        }

        yield return new WaitForSeconds(duration);

        // 解冻
        _character.UnFreeze();

        // 恢复原本颜色
        RestoreOriginalColor();
        // Debug.Log($"🔥 {name} 解冻恢复！");

        _freezeCoroutine = null;
    }

    void SetColor(Color color)
    {
        foreach (var kvp in _originalColors)
        {
            if (kvp.Key != null)
            {
                // 同时设置 _Color 和 _BaseColor 以兼容所有材质
                if (kvp.Key.material.HasProperty("_Color")) kvp.Key.material.color = color;
                if (kvp.Key.material.HasProperty("_BaseColor")) kvp.Key.material.SetColor("_BaseColor", color);
            }
        }
    }

    void RestoreOriginalColor()
    {
        foreach (var kvp in _originalColors)
        {
            if (kvp.Key != null)
            {
                // 还原回小本本上记的颜色
                if (kvp.Key.material.HasProperty("_Color")) kvp.Key.material.color = kvp.Value;
                if (kvp.Key.material.HasProperty("_BaseColor")) kvp.Key.material.SetColor("_BaseColor", kvp.Value);
            }
        }
    }
}