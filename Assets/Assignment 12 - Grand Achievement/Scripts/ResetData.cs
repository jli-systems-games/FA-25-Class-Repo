using UnityEngine;

public class ResetData : MonoBehaviour
{
    private void Awake()
    {
        //if (Data.SolutionMap != null)
        //{
        //    Data.SolutionMap.Clear();
        //}

        ResetAllData();
    }

    private void ResetAllData()
    {
        Data.firstPuzzleSolved = false;
        Data.secondPuzzleSolved = false;
        Data.thirdPuzzleSolved = false;
        Data.fourthPuzzleSolved = false;
        Data.fifthPuzzleSolved = false;
        Data.sixthPuzzleSolved = false;
        Data.seventhPuzzleSolved = false;
        Data.eigthPuzzleSolved = false;
        Data.ninthPuzzleSolved = false;

        Data.hintVisionEnabled = false;
    }
}
