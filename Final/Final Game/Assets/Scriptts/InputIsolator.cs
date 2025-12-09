using System.Reflection;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

/// <summary>
/// 运行时输入隔离器 - 挂在P3/P4身上，持续阻止键盘干扰
/// </summary>
public class InputIsolator : MonoBehaviour
{
    [Header("配置")]
    [Tooltip("是否是手柄玩家（P3/P4）")]
    public bool IsGamepadPlayer = true;

    [Tooltip("手柄索引 (1-4)")]
    [Range(1, 4)]
    public int GamepadIndex = 1;

    [Tooltip("玩家ID")]
    public string PlayerID = "Player3";

    [Header("调试")]
    public bool EnableDebugLogs = false;

    private InputManager _inputManager;
    private FieldInfo _axisHorizontalField;
    private FieldInfo _axisVerticalField;
    private bool _isInitialized = false;

    void Start()
    {
        _inputManager = GetComponent<InputManager>();
        if (_inputManager == null)
        {
            Debug.LogError($"[InputIsolator] {gameObject.name} 没有 InputManager 组件！");
            enabled = false;
            return;
        }

        // 使用反射获取私有字段
        var type = typeof(InputManager);
        _axisHorizontalField = type.GetField("AxisHorizontal", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        _axisVerticalField = type.GetField("AxisVertical", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        if (_axisHorizontalField == null || _axisVerticalField == null)
        {
            Debug.LogError($"[InputIsolator] 无法找到 InputManager 的轴字段！请检查插件版本。");
            enabled = false;
            return;
        }

        _isInitialized = true;

        if (EnableDebugLogs)
            Debug.Log($"[InputIsolator] {gameObject.name} 初始化完成 - 手柄索引: {GamepadIndex}, PlayerID: {PlayerID}");

        // 立即执行一次修复
        EnforceInputIsolation();
    }

    void Update()
    {
        if (!_isInitialized || !IsGamepadPlayer) return;

        // 每帧检查并强制清空主要轴（防止 fallback 到键盘）
        EnforceInputIsolation();
    }

    void EnforceInputIsolation()
    {
        if (_inputManager == null) return;

        // 强制清空主要轴，防止键盘输入
        string currentHorizontal = (string)_axisHorizontalField.GetValue(_inputManager);
        string currentVertical = (string)_axisVerticalField.GetValue(_inputManager);

        if (!string.IsNullOrEmpty(currentHorizontal) || !string.IsNullOrEmpty(currentVertical))
        {
            _axisHorizontalField.SetValue(_inputManager, "");
            _axisVerticalField.SetValue(_inputManager, "");

            if (EnableDebugLogs)
                Debug.Log($"[InputIsolator] 阻止了键盘干扰！已清空主要轴。");
        }

        // 检查次要轴是否正确配置
        var secondaryHField = _inputManager.GetType().GetField("AxisSecondaryHorizontal", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        var secondaryVField = _inputManager.GetType().GetField("AxisSecondaryVertical", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        if (secondaryHField != null && secondaryVField != null)
        {
            string expectedH = $"Joystick{GamepadIndex}Axis1";
            string expectedV = $"Joystick{GamepadIndex}Axis2";

            string currentH = (string)secondaryHField.GetValue(_inputManager);
            string currentV = (string)secondaryVField.GetValue(_inputManager);

            if (currentH != expectedH || currentV != expectedV)
            {
                secondaryHField.SetValue(_inputManager, expectedH);
                secondaryVField.SetValue(_inputManager, expectedV);

                if (EnableDebugLogs)
                    Debug.Log($"[InputIsolator] 修复了次要轴配置！");
            }
        }
    }

    void OnValidate()
    {
        // 在Inspector中修改参数时自动更新
        if (Application.isPlaying && _isInitialized)
        {
            EnforceInputIsolation();
        }
    }
}