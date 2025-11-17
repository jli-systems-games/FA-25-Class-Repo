using UnityEngine;
using TMPro;

public class CovenBattleManager3D : MonoBehaviour
{
    public ArcaneBlade3D blade1;
    public ArcaneBlade3D blade2;
    public TMP_Text resultText;

    public void StartBattle()
    {
        resultText.text = "";
        blade1.gameObject.SetActive(true);
        blade2.gameObject.SetActive(true);
    }

    public void CheckWinner()
    {
        if (!blade1.IsSpinning() && !blade2.IsSpinning())
            resultText.text = "Both shattered ";
        else if (!blade1.IsSpinning())
            resultText.text = blade2.bladeName + " wins the duel!";
        else if (!blade2.IsSpinning())
            resultText.text = blade1.bladeName + " wins the duel!";
    }
}
