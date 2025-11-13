using UnityEngine;
using TMPro;

public class BettingSystem : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown betDropdown;
    [SerializeField] private TMP_Text settleText;
    int betSide = 0;

    private void Awake()
    {
        if (betDropdown) betDropdown.onValueChanged.AddListener(i => betSide = i);
    }

    public void Settle(bool leftWin, bool rightWin)
    {
        if (!settleText) return;
        if (leftWin && betSide == 0) settleText.text = "Bet Result: WIN!";
        else if (rightWin && betSide == 1) settleText.text = "Bet Result: WIN!";
        else settleText.text = "Bet Result: LOSE!";
    }
}
