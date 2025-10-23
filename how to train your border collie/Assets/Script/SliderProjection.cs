using UnityEngine;
using UnityEngine.UI;

public class SOToSliders : MonoBehaviour
{
    public BorderCollieStats stats;  

    [Header("Sliders (0~100)")]
    public Slider energySlider;
    public Slider happinessSlider;
    public Slider fullnessSlider;
    public Slider skillSlider;

    void Awake()
    {
        InitSlider(energySlider);
        InitSlider(happinessSlider);
        InitSlider(fullnessSlider);
        InitSlider(skillSlider);
    }

    void Update()
    {

        if (stats == null) return;
        energySlider.value = stats.Energy;
        happinessSlider.value = stats.Happiness;
        fullnessSlider.value = stats.Fullness;
        skillSlider.value = stats.Skill;
    }

    void InitSlider(Slider s)
    {
        if (s == null) return;
        s.minValue = 0;
        s.maxValue = 100;
        s.wholeNumbers = true;
    }
}