using System.Collections.Generic;
using UnityEngine;

public class Data
{
    public static Dictionary<GameObject, int> SolutionMap;
    public static Dictionary<GameObject, int> SecondSolutionMap;
    public static Dictionary<GameObject, int> ThirdSolutionMap;

    public static bool firstPuzzleSolved = false;
    public static bool secondPuzzleSolved = false;
    public static bool thirdPuzzleSolved = false;

    public static bool hintVisionEnabled = false;
}
