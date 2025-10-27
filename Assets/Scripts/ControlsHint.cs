using UnityEngine;
using TMPro;

/// <summary>
/// 显示控制提示
/// </summary>
public class ControlsHint : MonoBehaviour
{
    private TextMeshProUGUI hintText;
    private bool isVisible = true;
    
    void Start()
    {
        // 创建提示文本
        CreateHintText();
        
        // 5秒后自动隐藏
        Invoke(nameof(HideHint), 5f);
    }
    
    void Update()
    {
        // 按H键切换显示/隐藏
        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleHint();
        }
    }
    
    void CreateHintText()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return;
        
        GameObject hintObj = new GameObject("ControlsHint");
        hintObj.transform.SetParent(canvas.transform, false);
        
        RectTransform rect = hintObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(1, 0.15f);
        rect.offsetMin = new Vector2(20, 20);
        rect.offsetMax = new Vector2(-20, -20);
        
        hintText = hintObj.AddComponent<TextMeshProUGUI>();
        hintText.text = "CONTROLS:\n↑↓ Select Menu | ←→ Select Option | SPACE Confirm | ESC Pause | H Hide/Show Help";
        hintText.fontSize = 24;
        hintText.color = new Color(1f, 1f, 1f, 0.7f);
        hintText.alignment = TextAlignmentOptions.Center;
        
        // 尝试使用像素字体
        UIBuilder builder = FindFirstObjectByType<UIBuilder>();
        if (builder != null && builder.pixelFont != null)
        {
            hintText.font = builder.pixelFont;
        }
    }
    
    void ToggleHint()
    {
        isVisible = !isVisible;
        if (hintText != null)
        {
            hintText.gameObject.SetActive(isVisible);
        }
    }
    
    void HideHint()
    {
        isVisible = false;
        if (hintText != null)
        {
            hintText.gameObject.SetActive(false);
        }
    }
}

