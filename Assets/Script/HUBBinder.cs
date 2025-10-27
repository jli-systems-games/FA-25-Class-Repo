using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDBinder : MonoBehaviour
{
    public TMP_Text happyText, hungerText, healthText;


    void OnEnable()
    {
        var S = GameState.Instance;
        if (S == null) return;

        S.OnChanged += UpdateUI;
        S.ForceSync();
    }

    void OnDisable()
    {
        var S = GameState.Instance;
        if (S != null) S.OnChanged -= UpdateUI;
    }

    void UpdateUI(int happy, int hunger, int health)
    {
        if (happyText) happyText.text = $"{happy}";
        if (hungerText) hungerText.text = $"{hunger}";
        if (healthText) healthText.text = $"{health}";
    }
}