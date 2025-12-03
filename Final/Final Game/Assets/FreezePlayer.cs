using UnityEngine;
using MoreMountains.TopDownEngine;
using System.Collections;

public class PlayerFreezeController : MonoBehaviour
{
    private Character _character;
    private Coroutine _freezeCoroutine;

    // 记录原始材质颜色，用于变色反馈
    private Renderer[] _renderers;
    private Color _originalColor;

    void Start()
    {
        _character = GetComponent<Character>();

        // 获取模型渲染器以便做变色效果 (可选)
        if (_character.CharacterModel != null)
        {
            _renderers = _character.CharacterModel.GetComponentsInChildren<Renderer>();
        }
    }

    // --- 供子弹调用 ---
    public void ApplyFreeze(float duration)
    {
        // 如果之前正在冻结，先停止之前的倒计时（实现“刷新”效果）
        if (_freezeCoroutine != null) StopCoroutine(_freezeCoroutine);

        // 开启新的冻结倒计时
        _freezeCoroutine = StartCoroutine(FreezeRoutine(duration));
    }

    IEnumerator FreezeRoutine(float duration)
    {
        // 1. 执行冻结
        if (_character.ConditionState.CurrentState != CharacterStates.CharacterConditions.Frozen)
        {
            _character.Freeze(); // TDE 自带方法，会禁止移动和操作

            // 变个色 (变成冰蓝色)
            ChangeColor(Color.cyan);
            Debug.Log($"❄️ {name} 被冻结了！时长: {duration}秒");
        }

        // 2. 等待
        yield return new WaitForSeconds(duration);

        // 3. 解冻
        _character.UnFreeze();
        ChangeColor(Color.white); // 恢复颜色 (假设原色是白)
        Debug.Log($"🔥 {name} 解冻了！");

        _freezeCoroutine = null;
    }

    void ChangeColor(Color color)
    {
        if (_renderers != null)
        {
            foreach (var r in _renderers)
            {
                // 简单的材质变色，如果材质不支持可以忽略
                if (r.material.HasProperty("_Color")) r.material.color = color;
            }
        }
    }
}