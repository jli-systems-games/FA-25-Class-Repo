using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReputationManager : MonoBehaviour
{
    public static ReputationManager Instance;

    [Header("Reputation Values")]
    public int familyFear = 0;
    public int familyRespect = 0;

    [Header("UI (optional)")]
    public Slider fearSlider;
    public Slider respectSlider;
    public TextMeshProUGUI fearLabel;
    public TextMeshProUGUI respectLabel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddReputation(int fearDelta, int respectDelta)
    {
        familyFear = Mathf.Clamp(familyFear + fearDelta, 0, 100);
        familyRespect = Mathf.Clamp(familyRespect + respectDelta, 0, 100);
        UpdateUI();
    }

    public void ResetReputation()
    {
        familyFear = 0;
        familyRespect = 0;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (fearSlider != null) fearSlider.value = familyFear;
        if (respectSlider != null) respectSlider.value = familyRespect;

        if (fearLabel != null) fearLabel.text = $"FEAR: {familyFear}";
        if (respectLabel != null) respectLabel.text = $"RESPECT: {familyRespect}";

        if (ReputationUI.Instance != null)
        {
            ReputationUI.Instance.UpdateReputation(familyFear, familyRespect);
        }
    }
}
