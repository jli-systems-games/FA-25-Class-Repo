using UnityEngine;

/// <summary>
/// 使用Scale缩放的进度条（备用方案）
/// 直接改变RectTransform的Scale来显示进度
/// 更可靠，适配Unity 6
/// </summary>
public class ScaleProgressBar : MonoBehaviour
{
    [Header("进度条设置")]
    [Tooltip("进度条类型")]
    public BarType barType = BarType.Hunger;
    
    [Tooltip("最大值")]
    public float maxValue = 100f;
    
    [Tooltip("当前值（只读）")]
    [SerializeField]
    private float currentValue = 100f;
    
    [Tooltip("进度条方向")]
    public Direction direction = Direction.Horizontal;
    
    [Header("调试信息")]
    [SerializeField]
    private float fillPercentage = 1f;
    
    // 私有变量
    private RectTransform rectTransform;
    private Vector3 originalScale;
    
    public enum BarType
    {
        Hunger,
        Happiness,
        Hygiene
    }
    
    public enum Direction
    {
        Horizontal,  // 水平（X轴）
        Vertical     // 垂直（Y轴）
    }
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        
        if (rectTransform != null)
        {
            // 设置Pivot为左侧（0, 0.5），这样缩放时从右边收缩
            rectTransform.pivot = new Vector2(0f, 0.5f);
            Debug.Log($"ScaleProgressBar ({barType}): Pivot设置为左侧 (0, 0.5)");
        }
    }
    
    void Start()
    {
        // 在Start中保存原始缩放，确保正确
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        
        // 确保原始缩放是(1,1,1)
        if (originalScale == Vector3.zero)
        {
            originalScale = new Vector3(1, 1, 1);
            rectTransform.localScale = originalScale;
        }
        
        Debug.Log($"ScaleProgressBar ({barType}): 原始Scale = {originalScale}");
    }
    
    void Update()
    {
        UpdateBar();
    }
    
    private void UpdateBar()
    {
        if (CatManager.Instance == null || CatManager.Instance.catData == null)
            return;
        
        CatData data = CatManager.Instance.catData;
        
        // 获取当前值
        switch (barType)
        {
            case BarType.Hunger:
                currentValue = data.hunger;
                break;
            case BarType.Happiness:
                currentValue = data.happiness;
                break;
            case BarType.Hygiene:
                currentValue = data.hygiene;
                break;
        }
        
        // 限制范围
        currentValue = Mathf.Clamp(currentValue, 0, maxValue);
        
        // 计算百分比
        fillPercentage = currentValue / maxValue;
        fillPercentage = Mathf.Clamp01(fillPercentage);
        
        // 更新缩放
        UpdateScale();
    }
    
    private void UpdateScale()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
            if (rectTransform == null) return;
        }
        
        // 确保originalScale已设置
        if (originalScale == Vector3.zero)
        {
            originalScale = new Vector3(1, 1, 1);
        }
        
        Vector3 newScale = originalScale;
        
        if (direction == Direction.Horizontal)
        {
            // 水平方向缩放X
            newScale.x = fillPercentage;  // 直接使用百分比，因为原始是1
        }
        else
        {
            // 垂直方向缩放Y
            newScale.y = fillPercentage;
        }
        
        // 强制应用缩放
        rectTransform.localScale = newScale;
    }
    
    /// <summary>
    /// 手动设置进度值
    /// </summary>
    public void SetValue(float value)
    {
        currentValue = Mathf.Clamp(value, 0, maxValue);
        fillPercentage = currentValue / maxValue;
        UpdateScale();
    }
    
    /// <summary>
    /// 重置为原始缩放
    /// </summary>
    public void ResetScale()
    {
        if (rectTransform != null)
        {
            rectTransform.localScale = originalScale;
        }
    }
}

