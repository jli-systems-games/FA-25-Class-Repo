using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThirdPuzzleManager : MonoBehaviour
{
    [Header("Platform Parent Objects")]
    public GameObject circPlatformParent;
    public GameObject rectPlatformParent;
    public GameObject hexPlatformParent;

    public GameObject circPlatform;
    public GameObject rectPlatform;
    public GameObject hexPlatform;

    [Header("Symbol Objects")]
    public GameObject circSymbol;
    public GameObject rectSymbol;
    public GameObject hexSymbol;

    [Header("Platform Rotations")]
    public float firstPlatformRotY;
    public float secondPlatformRotY;
    public float lastPlatformRotY;

    [Header("Inscription Positions")]
    public float firstSymbolPosY;
    public float secondSymbolPosY;
    public float lastSymbolPosY;

    private List<GameObject> passwordList;
    private List<GameObject> platformList;

    void Awake()
    {
        //Assign platform from parent
        circPlatform = circPlatformParent.transform.GetChild(0).gameObject;
        rectPlatform = rectPlatformParent.transform.GetChild(0).gameObject;
        hexPlatform = hexPlatformParent.transform.GetChild(0).gameObject;

        //Reset Solution Map
        if (Data.ThirdSolutionMap != null) Data.ThirdSolutionMap.Clear();

        //Set Random Symbol Order
        GameObject[] symbols = new GameObject[] {circSymbol, rectSymbol, hexSymbol};
        System.Random randomSymbol = new System.Random();
        var shuffledSymbols = symbols.OrderBy(x => randomSymbol.Next()).ToArray();

        passwordList = shuffledSymbols.ToList();
        string symbolsContents = string.Join(", ", passwordList);
        Debug.Log(symbolsContents);

        //Set Symbol Positions
        float[] symbolYPositions = new float[]
        {
            firstSymbolPosY,
            secondSymbolPosY,
            lastSymbolPosY
        };

        SetSymbolPositions(symbolYPositions);

        //Set Random Platform Order
        GameObject[] platforms = new GameObject[] {circPlatform, rectPlatform, hexPlatform};
        System.Random randomPlatform = new System.Random();
        var shuffledPlatforms = platforms.OrderBy(x => randomPlatform.Next()).ToArray();

        platformList = shuffledPlatforms.ToList();
        string platformsContents = string.Join(", ", platformList);
        Debug.Log(platformsContents);

        float[] platformParentYRotations = new float[]
        {
            firstPlatformRotY,
            secondPlatformRotY,
            lastPlatformRotY
        };

        SetPlatformParentRotations(platformParentYRotations);

        GenerateSolutionMap();
    }

    private void GenerateSolutionMap()
    {
        Data.ThirdSolutionMap = new Dictionary<GameObject, int>();

        for (int i = 0; i < passwordList.Count; i++)
        {
            GameObject symbol = passwordList[i];
            GameObject platform = null;

            //assign symbols to corresponding platforms
            if (symbol == circSymbol) platform = circPlatform;
            else if (symbol == rectSymbol) platform = rectPlatform;
            else if (symbol == hexSymbol) platform = hexPlatform;

            Data.ThirdSolutionMap[platform] = i;
        }
    }

    private void SetSymbolPositions(float[] symbolYPositions)
    {
        for (int i = 0; i < passwordList.Count; i++)
        {
            GameObject symbolObject = passwordList[i];
            float symbolY = symbolYPositions[i];

            Vector3 currentPos = symbolObject.transform.localPosition;

            symbolObject.transform.localPosition = new Vector3(currentPos.x, symbolY, currentPos.z);
        }
    }

    private void SetPlatformParentRotations(float[] platformParentYRotations)
    {
        for (int i = 0; i < platformList.Count; i++)
        {
            GameObject platform = platformList[i];
            GameObject platformParent = platform.transform.parent.gameObject;

            float platformParentY = platformParentYRotations[i];

            platformParent.transform.localRotation = Quaternion.Euler(0, platformParentY, 0);
        }
    }
}
