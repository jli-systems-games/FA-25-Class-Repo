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
}
}
