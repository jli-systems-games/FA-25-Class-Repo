using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum StatType
{
    Dirt,
    Hunger,
    Growth
}

public class StatManager : MonoBehaviour
{
    private Slider slider;

    public GameStat statAsset;
    public StatType statToTrack;
    public float decreaseRate;
    public float growthAmount = 2f;

    public Canvas growingCanvas;
    public Canvas gameOverCanvas;
    public Canvas winCanvas;
    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    void Update()
    {
        float currentValue = GetStatValue();

        float decreaseAmount = decreaseRate * Time.deltaTime;

        float newValue = currentValue - decreaseAmount;

        if (newValue <= 0)
        {
            newValue = 0;
        }
        else if (newValue > 100)
        {
            newValue = 100;
        }

        SetStatValue(newValue);

        slider.value = newValue;

        if (statAsset.dirtStat <= 1 || statAsset.hungerStat <= 1)
        {
            gameOverCanvas.gameObject.SetActive(true);
            Data.isFinished = true;
        }

        //Check for growth condition
        if (statAsset.dirtStat > 70 && statAsset.hungerStat > 70)
        {
            growingCanvas.gameObject.SetActive(true);
            statAsset.growthStat += growthAmount;

            if (statAsset.growthStat >= 99)
            {
                growingCanvas.gameObject.SetActive(false);
                winCanvas.gameObject.SetActive(true);
                Data.isFinished = true;
            }
        }
        else
        {
            statAsset.growthStat = 0;
            growingCanvas.gameObject.SetActive(false);
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
            case StatType.Growth:
                return statAsset.growthStat;
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
            case StatType.Growth:
                statAsset.growthStat = value;
                break;
        }
    }
}
