using UnityEngine;

public static class GameData
{
    public static int selectedCarIndex = 0;
    public static Color selectedColor = Color.red;

    public static void SetSelection(int index, Color color)
    {
        selectedCarIndex = index;
        selectedColor = color;
    }
 
    public static void SaveToPrefs()
    {
        PlayerPrefs.SetInt("car_index", selectedCarIndex);
        PlayerPrefs.SetFloat("car_color_r", selectedColor.r);
        PlayerPrefs.SetFloat("car_color_g", selectedColor.g);
        PlayerPrefs.SetFloat("car_color_b", selectedColor.b);
        PlayerPrefs.SetFloat("car_color_a", selectedColor.a);
        PlayerPrefs.Save();
    }
    public static void LoadFromPrefs()
    {
        selectedCarIndex = PlayerPrefs.GetInt("car_index", 0);
        selectedColor = new Color(
            PlayerPrefs.GetFloat("car_color_r", 1f),
            PlayerPrefs.GetFloat("car_color_g", 0f),
            PlayerPrefs.GetFloat("car_color_b", 0f),
            PlayerPrefs.GetFloat("car_color_a", 1f)
        );
    }
}
