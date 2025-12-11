using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EigthPuzzleManager : MonoBehaviour
{
    [Header("Orb")]
    public GameObject truthOrb;
    public float orbMoveSpeed = 3f;
    public float orbPauseTime = 1f;

    [Header("Platform Parent Objects")]
    public GameObject circPlatformParent1;
    public GameObject circPlatformParent2;
    public GameObject circPlatformParent3;
    public GameObject circPlatformParent4;
    public GameObject hexPlatformParent1;
    public GameObject hexPlatformParent2;
    public GameObject hexPlatformParent3;
    public GameObject hexPlatformParent4;
    public GameObject rectPlatformParent1;
    public GameObject rectPlatformParent2;
    public GameObject rectPlatformParent3;
    public GameObject rectPlatformParent4;
    public GameObject tearPlatformParent1;
    public GameObject tearPlatformParent2;
    public GameObject tearPlatformParent3;
    public GameObject tearPlatformParent4;

    public GameObject circPlatform1;
    public GameObject circPlatform2;
    public GameObject circPlatform3;
    public GameObject circPlatform4;
    public GameObject hexPlatform1;
    public GameObject hexPlatform2;
    public GameObject hexPlatform3;
    public GameObject hexPlatform4;
    public GameObject rectPlatform1;
    public GameObject rectPlatform2;
    public GameObject rectPlatform3;
    public GameObject rectPlatform4;
    public GameObject tearPlatform1;
    public GameObject tearPlatform2;
    public GameObject tearPlatform3;
    public GameObject tearPlatform4;

    [Header("Symbol Objects")]
    public GameObject circSymbol;
    public GameObject hexSymbol;
    public GameObject rectSymbol;
    public GameObject tearSymbol;

    [Header("Platform Rotations")]
    public float firstPlatformParentRotY;
    public float secondPlatformParentRotY;
    public float thirdPlatformParentRotY;
    public float fourthPlatformParentRotY;
    public float fifthPlatformParentRotY;
    public float sixthPlatformParentRotY;
    public float seventhPlatformParentRotY;
    public float eigthPlatformParentRotY;
    public float ninthPlatformParentRotY;
    public float tenthPlatformParentRotY;
    public float eleventhPlatformParentRotY;
    public float twelfthPlatformParentRotY;
    public float thirteenthPlatformParentRotY;
    public float fourteenthPlatformParentRotY;
    public float fifteenthPlatformParentRotY;
    public float lastPlatformParentRotY;

    [Header("Inscription Positions")]
    public float firstSymbolPosY;
    public float secondSymbolPosY;
    public float thirdSymbolPosY;
    public float lastSymbolPosY;

    private List<GameObject> passwordList;
    private List<GameObject> platformList;
    private List<GameObject[]> platformGroups;
    private GameObject[] childSymbolOrder;

    private GameObject chosenCirclePlatform;
    private GameObject chosenHexPlatform;
    private GameObject chosenRectPlatform;
    private GameObject chosenTearPlatform;

    void Awake()
    {
        // Assign platforms from parents
        circPlatform1 = circPlatformParent1.transform.GetChild(0).gameObject;
        circPlatform2 = circPlatformParent2.transform.GetChild(0).gameObject;
        circPlatform3 = circPlatformParent3.transform.GetChild(0).gameObject;
        circPlatform4 = circPlatformParent4.transform.GetChild(0).gameObject;

        hexPlatform1 = hexPlatformParent1.transform.GetChild(0).gameObject;
        hexPlatform2 = hexPlatformParent2.transform.GetChild(0).gameObject;
        hexPlatform3 = hexPlatformParent3.transform.GetChild(0).gameObject;
        hexPlatform4 = hexPlatformParent4.transform.GetChild(0).gameObject;

        rectPlatform1 = rectPlatformParent1.transform.GetChild(0).gameObject;
        rectPlatform2 = rectPlatformParent2.transform.GetChild(0).gameObject;
        rectPlatform3 = rectPlatformParent3.transform.GetChild(0).gameObject;
        rectPlatform4 = rectPlatformParent4.transform.GetChild(0).gameObject;

        tearPlatform1 = tearPlatformParent1.transform.GetChild(0).gameObject;
        tearPlatform2 = tearPlatformParent2.transform.GetChild(0).gameObject;
        tearPlatform3 = tearPlatformParent3.transform.GetChild(0).gameObject;
        tearPlatform4 = tearPlatformParent4.transform.GetChild(0).gameObject;

        // Reset solution map
        if (Data.EigthSolutionMap != null) Data.EigthSolutionMap.Clear();

        childSymbolOrder = new GameObject[] { circSymbol, hexSymbol, rectSymbol, tearSymbol };

        // Group platforms by shape
        platformGroups = new List<GameObject[]>
        {
            new GameObject[] { circPlatform1, circPlatform2, circPlatform3, circPlatform4 },
            new GameObject[] { hexPlatform1, hexPlatform2, hexPlatform3, hexPlatform4 },
            new GameObject[] { rectPlatform1, rectPlatform2, rectPlatform3, rectPlatform4 },
            new GameObject[] { tearPlatform1, tearPlatform2, tearPlatform3, tearPlatform4 }
        };

        // Pick one random platform per shape
        PickRandomPlatformPerShape();

        // Randomize symbol order
        GameObject[] symbols = new GameObject[] { circSymbol, rectSymbol, tearSymbol, hexSymbol };
        passwordList = symbols.OrderBy(x => Random.value).ToList();

        // Set symbol positions (keep your original logic)
        float[] symbolYPositions = new float[]
        {
            firstSymbolPosY,
            secondSymbolPosY,
            thirdSymbolPosY,
            lastSymbolPosY
        };
        SetSymbolPositions(symbolYPositions);


        Data.EigthSolutionMap = new Dictionary<GameObject, int>();

        for (int i = 0; i < passwordList.Count; i++)
        {
            GameObject symbol = passwordList[i];
            GameObject chosenPlatform = null;

            switch (symbol.name)
            {
                case "circle": chosenPlatform = chosenCirclePlatform; break;
                case "hexagon": chosenPlatform = chosenHexPlatform; break;
                case "rectangle": chosenPlatform = chosenRectPlatform; break;
                case "tear": chosenPlatform = chosenTearPlatform; break;
            }

            Data.EigthSolutionMap[chosenPlatform] = i; // i = step index
        }

        foreach (var group in platformGroups)
            foreach (var platform in group)
                if (!Data.EigthSolutionMap.ContainsKey(platform))
                    Data.EigthSolutionMap[platform] = -1;

        // Set random platform order (keep your original logic)
        platformList = platformGroups.SelectMany(g => g).OrderBy(x => Random.value).ToList();
        float[] platformParentYRotations = new float[]
        {
            firstPlatformParentRotY,
            secondPlatformParentRotY,
            thirdPlatformParentRotY,
            fourthPlatformParentRotY,
            fifthPlatformParentRotY,
            sixthPlatformParentRotY,
            seventhPlatformParentRotY,
            eigthPlatformParentRotY,
            ninthPlatformParentRotY,
            tenthPlatformParentRotY,
            eleventhPlatformParentRotY,
            twelfthPlatformParentRotY,
            thirteenthPlatformParentRotY,
            fourteenthPlatformParentRotY,
            fifteenthPlatformParentRotY,
            lastPlatformParentRotY
        };
        SetPlatformParentRotations(platformParentYRotations);

        // Start orb movement
        StartCoroutine(MoveOrbThroughPlatforms());
    }

    private void PickRandomPlatformPerShape()
    {
        chosenCirclePlatform = platformGroups[0][Random.Range(0, platformGroups[0].Length)];
        chosenHexPlatform = platformGroups[1][Random.Range(0, platformGroups[1].Length)];
        chosenRectPlatform = platformGroups[2][Random.Range(0, platformGroups[2].Length)];
        chosenTearPlatform = platformGroups[3][Random.Range(0, platformGroups[3].Length)];

        Debug.Log($"Chosen Circle Platform: {chosenCirclePlatform.name}");
        Debug.Log($"Chosen Hex Platform: {chosenHexPlatform.name}");
        Debug.Log($"Chosen Rect Platform: {chosenRectPlatform.name}");
        Debug.Log($"Chosen Tear Platform: {chosenTearPlatform.name}");
    }

    private IEnumerator MoveOrbThroughPlatforms()
    {
        GameObject[] cyclePlatforms = new GameObject[]
        {
        chosenCirclePlatform,
        chosenHexPlatform,
        chosenRectPlatform,
        chosenTearPlatform
        };

        int currentIndex = 0;

        //Start orb at the first platform
        truthOrb.transform.position = cyclePlatforms[currentIndex].transform.position;

        while (true) //infinite loop
        {
            int nextIndex = (currentIndex + 1) % cyclePlatforms.Length;
            Vector3 startPos = cyclePlatforms[currentIndex].transform.position;
            Vector3 targetPos = cyclePlatforms[nextIndex].transform.position;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * orbMoveSpeed;
                truthOrb.transform.position = Vector3.Lerp(startPos, targetPos, t);
                yield return null;
            }

            truthOrb.transform.position = targetPos;
            yield return new WaitForSeconds(orbPauseTime);

            currentIndex = nextIndex;
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
