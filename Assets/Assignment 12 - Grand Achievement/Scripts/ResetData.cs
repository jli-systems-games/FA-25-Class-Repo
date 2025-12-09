using UnityEngine;

public class ResetData : MonoBehaviour
{
    private void Awake()
    {
        //if (Data.SolutionMap != null)
        //{
        //    Data.SolutionMap.Clear();
        //}

        Data.firstPuzzleSolved = false;
        Data.secondPuzzleSolved = false;
        Data.thirdPuzzleSolved = false;
        Data.fourthPuzzleSolved = false;
        Data.fifthPuzzleSolved = false;
        Data.sixthPuzzleSolved = false;
        Data.seventhPuzzleSolved = false;

        Data.hintVisionEnabled = false;
    }
}
