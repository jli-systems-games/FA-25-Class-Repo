using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum StatType
{
    Dirt,
    Hunger,
    Play
}

public class StatManager : MonoBehaviour
{
    private Slider slider;

    public GameStat statAsset;
    public StatType statToTrack;
    public float decreaseRate;

    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    void Update()
    {
        float currentValue = GetStatValue();
        float decreaseAmount = decreaseRate * Time.deltaTime;

        float newValue = currentValue - decreaseAmount;

        SetStatValue(newValue);

        slider.value = newValue;

        if (slider.value <= 1 )
        {
            slider.value = 0;
            //SceneManager.LoadScene("Game Over Scene");
        }
    }

    private float GetStatValue()
    {
        switch (statToTrack)
        {
            case StatType.Dirt:
                return statAsset.dirtStat;
            case StatType.Hunger:
                return statAsset.hungerStat;
            case StatType.Play:
                return statAsset.playStat;
            default:
                return 0f;
        }
    }

    private void SetStatValue(float value)
    {
        switch (statToTrack)
        {
            case StatType.Dirt:
                statAsset.dirtStat = value;
                break;
            case StatType.Hunger:
                statAsset.hungerStat = value;
                break;
            case StatType.Play:
                statAsset.playStat = value;
                break;
        }
    }
}
