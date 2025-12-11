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
    public GameObject sixthIsland;
    public GameObject seventhIsland;
    public GameObject eigthIsland;
    public GameObject ninthIsland;

    public GameObject restartTriggerZone;
    public GameObject ninthWholeIsland;

    public GameObject firstBlockPath;
    public GameObject secondBlockPath;
    public GameObject thirdBlockPath;   
    public GameObject fourthBlockPath;
    public GameObject fifthBlockPath;
    public GameObject sixthBlockPath;
    public GameObject seventhBlockPath;
    public GameObject eigthBlockPath;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        secondIsland.SetActive(false);
        thirdIsland.SetActive(false);
        fourthIsland.SetActive(false);
        fifthIsland.SetActive(false);
        sixthIsland.SetActive(false);
        seventhIsland.SetActive(false);
        eigthIsland.SetActive(false);
        ninthIsland.SetActive(false);

        restartTriggerZone.SetActive(false);

        Data.arrowDestination = secondIsland.transform.GetChild(0).gameObject.transform;
    }

    public void CheckPuzzleSolved()
    {
        if (Data.ninthPuzzleSolved)
        {
            ninthWholeIsland.SetActive(false);
            restartTriggerZone.SetActive(true);
            return;
        }
        else
        {
            Data.arrowEnabled = true;
        }

        if (Data.eigthPuzzleSolved)
        {
            ninthIsland.SetActive(true);
            Data.arrowDestination = ninthIsland.transform.GetChild(0).gameObject.transform;
            eigthBlockPath.SetActive(false);
        }
        else if (Data.seventhPuzzleSolved)
        {
            eigthIsland.SetActive(true);
            Data.arrowDestination = eigthIsland.transform.GetChild(0).gameObject.transform;
            seventhBlockPath.SetActive(false);
        }
        else if (Data.sixthPuzzleSolved)
        {
            seventhIsland.SetActive(true);
            Data.arrowDestination = seventhIsland.transform.GetChild(0).gameObject.transform;
            sixthBlockPath.SetActive(false);
        }
        else if (Data.fifthPuzzleSolved)
        {
            sixthIsland.SetActive(true);
            Data.arrowDestination = sixthIsland.transform.GetChild(0).gameObject.transform;
            fifthBlockPath.SetActive(false);
        }
        else if (Data.fourthPuzzleSolved)
        {
            fifthIsland.SetActive(true);
            Data.arrowDestination = fifthIsland.transform.GetChild(0).gameObject.transform;
            fourthBlockPath.SetActive(false);
        }
        else if (Data.thirdPuzzleSolved)
        {
            fourthIsland.SetActive(true);
            Data.arrowDestination = fourthIsland.transform.GetChild(0).gameObject.transform;
            thirdBlockPath.SetActive(false);
        }
        else if (Data.secondPuzzleSolved)
        {
            thirdIsland.SetActive(true);
            Data.arrowDestination = thirdIsland.transform.GetChild(0).gameObject.transform;
            secondBlockPath.SetActive(false);
        }
        else if (Data.firstPuzzleSolved)
        {
            secondIsland.SetActive(true);
            Data.arrowDestination = secondIsland.transform.GetChild(0).gameObject.transform;

            firstBlockPath.SetActive(false);
        }
    }
}
