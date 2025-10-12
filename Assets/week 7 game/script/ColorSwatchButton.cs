// ColorSwatchButton.cs (수정본)
using UnityEngine;
using UnityEngine.UI;

public class ColorSwatchButton : MonoBehaviour
{
    [SerializeField] CarCustomizerManager manager; // ← 매니저 참조
    [SerializeField] Color swatch = Color.red;

    void Awake()
    {
        var btn = GetComponent<Button>();
        if (btn) btn.onClick.AddListener(() =>
        {
            if (manager) manager.SetColor(swatch);  // ← CarColor가 아니라 매니저 호출
        });
    }
}
