using System.Collections.Generic;
using UnityEngine;

public class Data
{
    public static Dictionary<GameObject, int> SolutionMap;
    public static Dictionary<GameObject, int> SecondSolutionMap;
    public static Dictionary<GameObject, int> ThirdSolutionMap;
    public static Dictionary<GameObject, int> FourthSolutionMap;
    public static Dictionary<GameObject, int> FifthSolutionMap;
    public static Dictionary<GameObject, int> SixthSolutionMap;

    public static bool firstPuzzleSolved = false;
    public static bool secondPuzzleSolved = false;
    public static bool thirdPuzzleSolved = false;
    public static bool fourthPuzzleSolved = false;
    public static bool fifthPuzzleSolved = false;
    public static bool sixthPuzzleSolved = false;

    public static bool hintVisionEnabled = false;
    public static bool inDisableZone = false;
}
