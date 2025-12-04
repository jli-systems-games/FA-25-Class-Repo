using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PuzzleStageManager : MonoBehaviour
{
    public GameObject secondIsland;
    public GameObject thirdIsland;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        secondIsland.SetActive(false);
        thirdIsland.SetActive(false);
    }

    public void CheckPuzzleSolved()
    {
        if (Data.firstPuzzleSolved)
        {
            secondIsland.SetActive(true);
        }
        
        if (Data.secondPuzzleSolved)
        {
            thirdIsland.SetActive(true);
        }
    }
}
