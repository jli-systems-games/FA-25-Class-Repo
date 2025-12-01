using System.Collections;
using UnityEngine;

public class PuzzleStageManager : MonoBehaviour
{
    public GameObject secondIsland;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        secondIsland.SetActive(false);
    }

    public void CheckPuzzleSolved()
    {
        if (Data.firstPuzzleSolved)
        {
            StartCoroutine(DelayBeforeIslandOn(secondIsland));
            secondIsland.SetActive(true);
        }
    }

    private IEnumerator DelayBeforeIslandOn(GameObject island)
    {
        yield return new WaitForSeconds(1f);

        island.SetActive(true);
    }
}
