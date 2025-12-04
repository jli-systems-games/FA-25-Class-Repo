using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FifthPuzzleManager : MonoBehaviour
{
    [Header("Animal Statue Parent Objects")]
    public GameObject rabbitStatueParent;
    public GameObject elephantStatueParent;
    public GameObject rhinoStatueParent;
    public GameObject deerStatueParent;

    private GameObject rabbitStatue;
    private GameObject elephantStatue;
    private GameObject rhinoStatue;
    private GameObject deerStatue;

    [Header("Platform Parent Objects")]
    public GameObject rabbitPlatformParent;
    public GameObject elephantPlatformParent;
    public GameObject rhinoPlatformParent;
    public GameObject deerPlatformParent;

    public GameObject rabbitPlatform;
    public GameObject elephantPlatform;
    public GameObject rhinoPlatform;
    public GameObject deerPlatform;

    [Header("Symbol Objects")]
    public GameObject circSymbol;
    public GameObject rectSymbol;
    public GameObject tearSymbol;
    public GameObject hexSymbol;

    [Header("Platform Rotations")]
    public float firstPlatformRotY;
    public float secondPlatformRotY;
    public float thirdPlatformRotY;
    public float lastPlatformRotY;

    [Header("Statue Rotations")]
    public float firstStatueParentRotY;
    public float secondStatueParentRotY;
    public float thirdStatueParentRotY;
    public float lastStatueParentRotY;

    [Header("Inscription Positions")]
    public float firstSymbolPosY;
    public float secondSymbolPosY;
    public float thirdSymbolPosY;
    public float lastSymbolPosY;

    private List<GameObject> passwordList;
    private List<GameObject> platformList;
    private List<GameObject> animalList;

    private Dictionary<GameObject, int> animalGemIndexMap;
    private GameObject[] childSymbolOrder;

    void Awake()
    {
        //Assign statue from parent
        rabbitStatue = rabbitStatueParent.transform.GetChild(0).gameObject;
        elephantStatue = elephantStatueParent.transform.GetChild(0).gameObject;
        rhinoStatue = rhinoStatueParent.transform.GetChild(0).gameObject;
        deerStatue = deerStatueParent.transform.GetChild(0).gameObject;

        //Assign platform from parent
        //Assign statue from parent
        rabbitPlatform = rabbitPlatformParent.transform.GetChild(0).gameObject;
        elephantPlatform = elephantPlatformParent.transform.GetChild(0).gameObject;
        rhinoPlatform = rhinoPlatformParent.transform.GetChild(0).gameObject;
        deerPlatform = deerPlatformParent.transform.GetChild(0).gameObject;

        //Reset Solution Map
        if (Data.FifthSolutionMap != null) Data.FifthSolutionMap.Clear();

        animalGemIndexMap = new Dictionary<GameObject, int>();
        childSymbolOrder = new GameObject[] { rectSymbol, tearSymbol, hexSymbol, circSymbol };

        //Disable all gems
        foreach (Transform childTransform in rabbitStatue.transform)
        {
            GameObject child = childTransform.gameObject;
            child.SetActive(false);
        }
        foreach (Transform childTransform in elephantStatue.transform)
        {
            GameObject child = childTransform.gameObject;
            child.SetActive(false);
        }
        foreach (Transform childTransform in rhinoStatue.transform)
        {
            GameObject child = childTransform.gameObject;
            child.SetActive(false);
        }
        foreach (Transform childTransform in deerStatue.transform)
        {
            GameObject child = childTransform.gameObject;
            child.SetActive(false);
        }

        //Set Random Symbol Order
        GameObject[] symbols = new GameObject[] { circSymbol, rectSymbol, tearSymbol, hexSymbol };
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
            thirdSymbolPosY,
            lastSymbolPosY
        };

        SetSymbolPositions(symbolYPositions);

        //Set Random Platform Order
        GameObject[] platforms = new GameObject[] { rabbitPlatform, elephantPlatform, rhinoPlatform, deerPlatform };
        System.Random randomPlatform = new System.Random();
        var shuffledPlatforms = platforms.OrderBy(x => randomPlatform.Next()).ToArray();

        platformList = shuffledPlatforms.ToList();
        string platformsContents = string.Join(", ", platformList);
        Debug.Log(platformsContents);

        //Set Platform Positions
        float[] platformParentYRotations = new float[]
        {
            firstPlatformRotY,
            secondPlatformRotY,
            thirdPlatformRotY,
            lastPlatformRotY
        };

        SetPlatformParentRotations(platformParentYRotations);

        //Set Random Animal Statue Gem Order
        GameObject[] animals = new GameObject[] { rabbitStatue, elephantStatue, rhinoStatue, deerStatue };
        System.Random randomAnimal = new System.Random();
        var shuffledAnimals = animals.OrderBy(x => randomAnimal.Next()).ToArray();

        animalList = shuffledAnimals.ToList();
        string animalsContents = string.Join(", ", animalList);
        Debug.Log(animalsContents);

        //Set Statue Parent Positions
        float[] statueParentsYRotations = new float[]
        {
            firstStatueParentRotY,
            secondStatueParentRotY,
            thirdStatueParentRotY,
            lastStatueParentRotY
        };

        SetStatueParentRotations(statueParentsYRotations);

        SetGemOnAnimal();

        GenerateSolutionMap();
    }

    private void GenerateSolutionMap()
    {
        Data.FifthSolutionMap = new Dictionary<GameObject, int>();

        //Assign animal platform order depending on symbol order and gem on animal statue
        foreach (KeyValuePair<GameObject, int> entry in animalGemIndexMap)
        {
            GameObject statueObject = entry.Key;
            int activeGemIndex = entry.Value;

            GameObject targetSymbolObject = childSymbolOrder[activeGemIndex];

            int passwordSymbolOrder = passwordList.IndexOf(targetSymbolObject);

            GameObject animalPlatform = GetPlatformForStatue(statueObject);

            Data.FifthSolutionMap.Add(animalPlatform, passwordSymbolOrder);
        }
    }

    private GameObject GetPlatformForStatue(GameObject statue)
    {
        if (statue == rabbitStatue) return rabbitPlatform;
        if (statue == elephantStatue) return elephantPlatform;
        if (statue == rhinoStatue) return rhinoPlatform;
        if (statue == deerStatue) return deerPlatform;
        return null;
    }

    private void SetGemOnAnimal()
    {
        for (int i = 0; i < animalList.Count; i++)
        {
            GameObject animalObject = animalList[i];
            int activeGemIndex = i;

            Transform selectedGem = animalObject.transform.GetChild(i);
            selectedGem.gameObject.SetActive(true);

            animalGemIndexMap.Add(animalObject, activeGemIndex);
        }
    }

    private void SetSymbolPositions(float[] symbolYPositions)
    {
        for (int i = 0; i < passwordList.Count; i++)
        {
            GameObject symbolObject = passwordList[i];
            float symbolY = symbolYPositions[i];

            Vector3 currentPos = symbolObject.transform.localPosition;

            symbolObject.transform.localPosition = new Vector3(
                currentPos.x,
                symbolY,
                currentPos.z
            );
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

    private void SetStatueParentRotations(float[] statueParentYRotations)
    {
        for (int i = 0; i < animalList.Count; i++)
        {
            GameObject statue = animalList[i];
            GameObject statueParent = statue.transform.parent.gameObject;

            float statueParentY = statueParentYRotations[i];

            statueParent.transform.localRotation = Quaternion.Euler(0, statueParentY, 0);
        }
    }
}
