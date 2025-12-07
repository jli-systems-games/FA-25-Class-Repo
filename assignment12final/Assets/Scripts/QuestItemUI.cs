using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestItemUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI labelText;
    public Image checkmarkImage;

    private bool completed = false;

    public void Setup(string title, bool isCompleted)
    {
        if (labelText != null)
        {
            labelText.text = title;
        }

        SetCompleted(isCompleted);
    }

    public void SetCompleted(bool isCompleted)
    {
        completed = isCompleted;

        if (checkmarkImage != null)
        {
            checkmarkImage.enabled = completed;
        }

        if (labelText != null)
        {
            labelText.color = completed ? new Color(0.8f, 0.8f, 0.8f) : Color.white;
        }
    }
}
