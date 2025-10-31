using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    
    public TextMeshProUGUI textField;

    //update accuracy rate
    public void UpdateAccuracy(float accuracyPercent)
    {
        if (textField != null)
        {
            textField.text = accuracyPercent.ToString("F2") + "%";
        }
    }

    
    public void UpdateSuccessCount(int count)
    {
        if (textField != null)
        {
            textField.text = "HIT: " + count;
        }
    }


    public void UpdateFailCount(int count)
    {
        if (textField != null)
        {
            textField.text = "MISS: " + count;
        }
    }
}