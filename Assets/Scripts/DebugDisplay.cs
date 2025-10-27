using UnityEngine;
using TMPro;

/// <summary>
/// 调试信息显示
/// 按F1键切换显示/隐藏
/// </summary>
public class DebugDisplay : MonoBehaviour
{
    [Header("设置")]
    public bool showOnStart = false;
    public KeyCode toggleKey = KeyCode.F1;
    
    private TextMeshProUGUI debugText;
    private bool isVisible = false;
    
    void Start()
    {
        CreateDebugText();
        isVisible = showOnStart;
        
        if (debugText != null)
        {
            debugText.gameObject.SetActive(isVisible);
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleDebug();
        }
        
        if (isVisible && debugText != null)
        {
            UpdateDebugInfo();
        }
    }
    
    void CreateDebugText()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return;
        
        GameObject debugObj = new GameObject("DebugDisplay");
        debugObj.transform.SetParent(canvas.transform, false);
        
        RectTransform rect = debugObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0.5f);
        rect.anchorMax = new Vector2(0.25f, 1);
        rect.offsetMin = new Vector2(10, 10);
        rect.offsetMax = new Vector2(-10, -120);
        
        debugText = debugObj.AddComponent<TextMeshProUGUI>();
        debugText.fontSize = 18;
        debugText.color = Color.white;
        debugText.alignment = TextAlignmentOptions.TopLeft;
        
        // 添加背景
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(debugObj.transform, false);
        bgObj.transform.SetAsFirstSibling();
        
        RectTransform bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = new Vector2(-5, -5);
        bgRect.offsetMax = new Vector2(5, 5);
        
        UnityEngine.UI.Image bgImage = bgObj.AddComponent<UnityEngine.UI.Image>();
        bgImage.color = new Color(0, 0, 0, 0.8f);
    }
    
    void UpdateDebugInfo()
    {
        if (CatManager.Instance == null || CatManager.Instance.catData == null)
        {
            debugText.text = "DEBUG INFO\n\nCatManager not found";
            return;
        }
        
        CatData data = CatManager.Instance.catData;
        
        string info = "=== DEBUG INFO ===\n\n";
        info += $"<color=yellow>STATS:</color>\n";
        info += $"Hunger: {data.hunger:F1}/100\n";
        info += $"Happiness: {data.happiness:F1}/100\n";
        info += $"Hygiene: {data.hygiene:F1}/100\n\n";
        
        info += $"<color=yellow>GAME DATA:</color>\n";
        info += $"Play Time: {data.GetFormattedPlayTime()}\n";
        info += $"Age: {data.age} days\n";
        info += $"Survival: {data.survivalTime:F1}s\n\n";
        
        info += $"<color=yellow>STATUS:</color>\n";
        info += $"State: {CatManager.Instance.currentState}\n";
        info += $"Critical: {data.IsInCriticalState()}\n";
        info += $"Critical Time: {data.currentCriticalTime:F1}s\n\n";
        
        info += $"<color=yellow>DECAY RATES:</color>\n";
        info += $"Hunger: {data.hungerDecayRate}/s\n";
        info += $"Happiness: {data.happinessDecayRate}/s\n";
        info += $"Hygiene: {data.hygieneDecayRate}/s\n\n";
        
        info += $"<color=gray>Press {toggleKey} to hide</color>";
        
        debugText.text = info;
    }
    
    void ToggleDebug()
    {
        isVisible = !isVisible;
        
        if (debugText != null)
        {
            debugText.gameObject.SetActive(isVisible);
        }
    }
}

