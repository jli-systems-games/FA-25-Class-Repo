using UnityEngine;
using System;

public static class CentralData
{
    // 과제 명시: 아무것도 상속받지 않는 중앙 데이터(엔진 생명주기와 분리)
    public struct CarConfig
    {
        public int index;
        public Color color;

        public CarConfig(int index, Color color)
        {
            this.index = index;
            this.color = color;
        }
    }

    public static CarConfig Selected = new CarConfig(0, Color.red);

    public static event Action OnChanged;

    public static void SetSelected(int index, Color color)
    {
        Selected = new CarConfig(index, color);
        OnChanged?.Invoke();
    }

    // (선택) PlayerPrefs 브리지: 세션을 넘어 저장하고 싶을 때 사용
    public static void SaveToPrefs()
    {
        PlayerPrefs.SetInt("car_index", Selected.index);
        PlayerPrefs.SetFloat("car_color_r", Selected.color.r);
        PlayerPrefs.SetFloat("car_color_g", Selected.color.g);
        PlayerPrefs.SetFloat("car_color_b", Selected.color.b);
        PlayerPrefs.SetFloat("car_color_a", Selected.color.a);
        PlayerPrefs.Save();
    }

    public static void LoadFromPrefsIfAny()
    {
        if (PlayerPrefs.HasKey("car_index"))
        {
            int idx = PlayerPrefs.GetInt("car_index", 0);
            float r = PlayerPrefs.GetFloat("car_color_r", 1f);
            float g = PlayerPrefs.GetFloat("car_color_g", 0f);
            float b = PlayerPrefs.GetFloat("car_color_b", 0f);
            float a = PlayerPrefs.GetFloat("car_color_a", 1f);
            SetSelected(idx, new Color(r, g, b, a));
        }
    }
}
