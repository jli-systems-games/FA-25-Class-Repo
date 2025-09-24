using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GearUI : MonoBehaviour
{
    [Header("Refs")]
    public FinalCarDrive drive;          // 拖你的 FinalCarDrive（Car 上）
    [Tooltip("优先使用 TextMeshProUGUI；没有就用 Text")]
    public TextMeshProUGUI tmp;
    public Text legacyText;

    [Header("Style")]
    public string prefix = "";           // 例如 "GEAR "
    public bool showSpeed = true;        // 是否显示速度
    public bool kmh = true;              // 用 km/h（否则 m/s）
    public Color driveColor = new Color(0.25f, 0.95f, 0.35f, 1f);
    public Color reverseColor = new Color(1f, 0.45f, 0.45f, 1f);

    void LateUpdate()
    {
        if (!drive) return;

        // 文本内容：D / R + 可选速度
        bool isD = drive.IsDrive;
        float spd = drive.CurrentSpeed;
        float val = kmh ? spd * 3.6f : spd;
        string unit = kmh ? " km/h" : " m/s";

        string text = prefix + (isD ? "D" : "R");
        if (showSpeed) text += $"  {val:0.#}{unit}";

        // 赋值 + 换色
        if (tmp)
        {
            tmp.text = text;
            tmp.color = isD ? driveColor : reverseColor;
        }
        if (legacyText)
        {
            legacyText.text = text;
            legacyText.color = isD ? driveColor : reverseColor;
        }
    }
}
