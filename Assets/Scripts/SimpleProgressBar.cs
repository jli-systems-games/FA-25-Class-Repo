using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 简单的进度条脚本
/// 直接附加到进度条Image上即可
/// </summary>
[RequireComponent(typeof(Image))]
public class SimpleProgressBar : MonoBehaviour
{
    [Header("进度条设置")]
    [Tooltip("进度条类型")]
    public BarType barType = BarType.Hunger;
    
    [Tooltip("最大值")]
    public float maxValue = 100f;
    
    [Tooltip("当前值（自动更新）")]
    [SerializeField]
    private float currentValue = 100f;
    
    [Header("颜色设置（可选）")]
    [Tooltip("自定义颜色（不勾选则使用Inspector设置的颜色）")]
    public bool useCustomColor = false;
    
    [Tooltip("自定义颜色")]
    public Color customColor = Color.green;
    
    // 私有变量
    private Image barImage;
    
    /// <summary>
    /// 进度条类型
    /// </summary>
    public enum BarType
    {
        Hunger,     // 饱食度
        Happiness,  // 心情
        Hygiene     // 清洁度
    }
    
    void Awake()
    {
        barImage = GetComponent<Image>();
        InitializeBar();
    }
    
    void Start()
    {
        // 确保初始化
        InitializeBar();
    }
    
    private void InitializeBar()
    {
        if (barImage == null)
        {
            barImage = GetComponent<Image>();
        }
        
        // 强制设置为Filled类型
        barImage.type = Image.Type.Filled;
        barImage.fillMethod = Image.FillMethod.Horizontal;
        barImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        barImage.fillAmount = 1.0f;
        
        // 如果使用自定义颜色
        if (useCustomColor)
        {
            barImage.color = customColor;
        }
        
        Debug.Log($"SimpleProgressBar: 初始化 {barType} 进度条，Type={barImage.type}, FillAmount={barImage.fillAmount}");
    }
    
    void Update()
    {
        UpdateBar();
    }
    
    /// <summary>
    /// 更新进度条
    /// </summary>
    private void UpdateBar()
    {
        if (CatManager.Instance == null || CatManager.Instance.catData == null)
            return;
        
        if (barImage == null)
        {
            barImage = GetComponent<Image>();
            if (barImage == null) return;
        }
        
        CatData data = CatManager.Instance.catData;
        
        // 根据类型获取对应的数值
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
        
        // 限制currentValue在0-maxValue之间
        currentValue = Mathf.Clamp(currentValue, 0, maxValue);
        
        // 计算fillAmount（0-1之间）
        float targetFillAmount = currentValue / maxValue;
        targetFillAmount = Mathf.Clamp01(targetFillAmount);
        
        // 强制设置类型（防止被重置）
        if (barImage.type != Image.Type.Filled)
        {
            barImage.type = Image.Type.Filled;
            barImage.fillMethod = Image.FillMethod.Horizontal;
            barImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        }
        
        // 更新fillAmount
        barImage.fillAmount = targetFillAmount;
    }
    
    /// <summary>
    /// 手动设置进度值
    /// </summary>
    public void SetValue(float value)
    {
        currentValue = Mathf.Clamp(value, 0, maxValue);
        if (barImage != null)
        {
            barImage.fillAmount = currentValue / maxValue;
        }
    }
    
    /// <summary>
    /// 获取当前值
    /// </summary>
    public float GetValue()
    {
        return currentValue;
    }
    
    /// <summary>
    /// 获取百分比（0-1）
    /// </summary>
    public float GetPercentage()
    {
        return currentValue / maxValue;
    }
}

