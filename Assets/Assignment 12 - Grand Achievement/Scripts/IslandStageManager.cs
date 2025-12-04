using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IslandStageManager : MonoBehaviour
{
    public GameObject secondIsland;
    public GameObject thirdIsland;
    public GameObject fourthIsland;
    public GameObject fifthIsland;
    //public GameObject sixthIsland;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        secondIsland.SetActive(false);
        thirdIsland.SetActive(false);
        fourthIsland.SetActive(false);
        fifthIsland.SetActive(false);
        //sixthIsland.SetActive(false);
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

        if (Data.thirdPuzzleSolved)
        {
            fourthIsland.SetActive(true);
        }

        if (Data.fourthPuzzleSolved)
        {
            fifthIsland.SetActive(true);
        }

        if (Data.fifthPuzzleSolved)
        {
            SceneManager.LoadScene("End Scene");
            //sixthIsland.SetActive(true);
        }
    }
}
