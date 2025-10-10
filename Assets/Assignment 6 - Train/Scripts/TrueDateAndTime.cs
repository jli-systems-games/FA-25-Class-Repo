using TMPro;
using UnityEngine;

public class TrueDateAndTime : MonoBehaviour
{
    public TrueValue trueValue;
    public TextMeshProUGUI yearText;
    public TextMeshProUGUI monthText;
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI hourText;

    void Start()
    {
        int currentHour = trueValue.hour - 1;

        yearText.text = trueValue.year.ToString();
        monthText.text = trueValue.month.ToString("D2");
        dayText.text = trueValue.day.ToString("D2");
        hourText.text = currentHour.ToString("D2");
    }
}
