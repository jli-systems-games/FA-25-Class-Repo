using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Text timerText;
    public SpriteRenderer warningFrame;
    public GameObject recIcon;

    void Start() { SetWarningFrame(false); }

    public void SetTimer(float secLeft)
    {
        if (!timerText) return;
        secLeft = Mathf.Max(0, secLeft);
        int m = Mathf.FloorToInt(secLeft / 60f);
        int s = Mathf.FloorToInt(secLeft % 60f);
        timerText.text = $"{m:00}:{s:00}";
    }

    public void SetWarningFrame(bool on)
    {
        if (warningFrame) warningFrame.enabled = on;
        if (recIcon) recIcon.SetActive(on);
    }
}
