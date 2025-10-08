using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TMP_Text label;
    public float time = 65f;

    void Update()
    {
        if (time > 0f)
        {
            time -= Time.deltaTime;
            if (time < 0f) time = 0f;
        }

        int sec = Mathf.CeilToInt(time);
        int m = sec / 60;
        int s = sec % 60;
        if (label) label.text = $"{m:00}:{s:00}";
    }
}
