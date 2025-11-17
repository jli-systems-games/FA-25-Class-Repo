using UnityEngine;
using UnityEngine.UI;

public class StaminaUI2D : MonoBehaviour
{
    public Slider p1Slider;
    public Slider p2Slider;

    private BeybladeStats2D p1Stats;
    private BeybladeStats2D p2Stats;

    public void SetTargets(BeybladeStats2D p1, BeybladeStats2D p2)
    {
        p1Stats = p1;
        p2Stats = p2;

        // set max values
        p1Slider.maxValue = p1Stats.stamina;
        p2Slider.maxValue = p2Stats.stamina;
    }

    void Update()
    {
        if (p1Stats) p1Slider.value = p1Stats.stamina;
        if (p2Stats) p2Slider.value = p2Stats.stamina;
    }
}
