using System;
using UnityEngine;

/// <summary>
/// 处理键盘输入，并将其转换为游戏动作
/// 只使用方向键和空格键
/// </summary>
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    
    // 输入事件
    public event Action OnUpPressed;
    public event Action OnDownPressed;
    public event Action OnLeftPressed;
    public event Action OnRightPressed;
    public event Action OnConfirmPressed;
    
    [Header("输入设置")]
    public float inputCooldown = 0.2f; // 输入冷却时间，防止连续触发
    
    private float lastInputTime = 0f;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Update()
    {
        // 检查输入冷却
        if (Time.time - lastInputTime < inputCooldown)
        {
            return;
        }
        
        // 检测方向键输入
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            OnUpPressed?.Invoke();
            lastInputTime = Time.time;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            OnDownPressed?.Invoke();
            lastInputTime = Time.time;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            OnLeftPressed?.Invoke();
            lastInputTime = Time.time;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            OnRightPressed?.Invoke();
            lastInputTime = Time.time;
        }
        
        // 检测空格键（确认键）
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            OnConfirmPressed?.Invoke();
            lastInputTime = Time.time;
        }
    }
}

