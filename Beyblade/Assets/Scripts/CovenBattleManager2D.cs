using UnityEngine;
using TMPro;

public class CovenBattleManager2D : MonoBehaviour
{
    [Header("Blades")]
    public ArcaneBlade2D blade1;
    public ArcaneBlade2D blade2;

    [Header("UI")]
    public TMP_Text resultText;

    [Header("Battle Settings")]
    public Transform blade1Spawn;
    public Transform blade2Spawn;

    private bool battleActive = false;

    void Start()
    {
        // Optional: auto-start for testing
        // StartBattle();
    }

    public void StartBattle()
    {
        // Reset the scene
        resultText.text = "";
        battleActive = true;

        // Instantiate blades if needed
        if (blade1 != null && blade2 != null)
        {
            // Move them to spawn points
            blade1.transform.position = blade1Spawn.position;
            blade2.transform.position = blade2Spawn.position;

            // Reset rotation and reactivate
            blade1.gameObject.SetActive(true);
            blade2.gameObject.SetActive(true);

            // Begin spinning
            blade1.BeginSpin();
            blade2.BeginSpin();
        }
        else
        {
            Debug.LogWarning("⚠️ Blades not assigned in Inspector!");
        }
    }

    void Update()
    {
        if (!battleActive) return;

        // Check if either blade has stopped spinning
        if (!blade1.IsSpinning() || !blade2.IsSpinning())
        {
            battleActive = false;
            DetermineWinner();
        }
    }

    void DetermineWinner()
    {
        if (!blade1.IsSpinning() && !blade2.IsSpinning())
            resultText.text = "Draw! Both blades stopped.";
        else if (!blade1.IsSpinning())
            resultText.text = $"{blade2.bladeName} wins!";
        else if (!blade2.IsSpinning())
            resultText.text = $"{blade1.bladeName} wins!";
    }
}
