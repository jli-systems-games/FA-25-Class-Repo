using UnityEngine;
using TMPro;

public class EnergySystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI energyText;  
    [SerializeField] private int maxEnergy = 100;
    [SerializeField] private int clickCost = 5;
    [SerializeField] private int refillRate = 15;
    public GameObject video;
    private int currentEnergy;

    void Start()
    {
        currentEnergy = maxEnergy;
        UpdateEnergyText();
        InvokeRepeating(nameof(RefillEnergy), 1f, 1f); 
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryUseEnergy();
        }
        if (currentEnergy == 0)
            video.SetActive(true);
    }

    void TryUseEnergy()
    {
        if (currentEnergy >= clickCost)
        {
            currentEnergy -= clickCost;
            UpdateEnergyText();
            Debug.Log("Clicked! Energy now: " + currentEnergy);
        }
        else
        {
            Debug.Log("Not enough energy!");
        }
    }

    void RefillEnergy()
    {
        if (currentEnergy < maxEnergy)
        {
            currentEnergy += refillRate;
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy); 
            UpdateEnergyText();
        }
    }

    void UpdateEnergyText()
    {
        if (energyText != null)
            energyText.text = "Energy: " + currentEnergy;
    }
}
