using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimalPuzzleManager : MonoBehaviour
{
    [Header("Animal Statue Objects")]
    public GameObject rabbitStatue;
    public GameObject elephantStatue;
    public GameObject rhinoStatue;
    public GameObject deerStatue;

    [Header("Platform Objects")]
    public GameObject rabbitPlatform;
    public GameObject elephantPlatform;
    public GameObject rhinoPlatform;
    public GameObject deerPlatform;

    [Header("Symbol Objects")]
    public GameObject circSymbol;
    public GameObject rectSymbol;
    public GameObject tearSymbol;
    public GameObject hexSymbol;

    [Header("Platform Positions")]
    public float firstPlatformPosX;
    public float secondPlatformPosX;
    public float thirdPlatformPosX;
    public float lastPlatformPosX;

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

    //void Awake()
    //{
    //    //Reset Solution Map
    //    if (Data.SolutionMap != null) Data.SolutionMap.Clear();

    //    animalGemIndexMap = new Dictionary<GameObject, int>();
    //    childSymbolOrder = new GameObject[] {rectSymbol, tearSymbol, hexSymbol, circSymbol};

    //    //Disable all gems
    //    foreach (Transform childTransform in rabbitStatue.transform)
    //    {
    //        GameObject child = childTransform.gameObject;
    //        child.SetActive(false);
    //    }
    //    foreach (Transform childTransform in elephantStatue.transform)
    //    {
    //        GameObject child = childTransform.gameObject;
    //        child.SetActive(false);
    //    }
    //    foreach (Transform childTransform in rhinoStatue.transform)
    //    {
    //        GameObject child = childTransform.gameObject;
    //        child.SetActive(false);
    //    }
    //    foreach (Transform childTransform in deerStatue.transform)
    //    {
    //        GameObject child = childTransform.gameObject;
    //        child.SetActive(false);
    //    }

    //    //Set Random Symbol Order
    //    GameObject[] symbols = new GameObject[] {circSymbol, rectSymbol, tearSymbol, hexSymbol};
    //    System.Random randomSymbol = new System.Random();
    //    var shuffledSymbols = symbols.OrderBy(x => randomSymbol.Next()).ToArray();

    //    passwordList = shuffledSymbols.ToList();
    //    string symbolsContents = string.Join(", ", passwordList);
    //    Debug.Log(symbolsContents);

    //    //Set Symbol Positions
    //    float[] symbolYPositions = new float[]
    //    {
    //        firstSymbolPosY,
    //        secondSymbolPosY,
    //        thirdSymbolPosY,
    //        lastSymbolPosY
    //    };

    //    SetSymbolPositions(symbolYPositions);

    //    //Set Random Platform Order
    //    GameObject[] platforms = new GameObject[] {rabbitPlatform, elephantPlatform, rhinoPlatform, deerPlatform};
    //    System.Random randomPlatform = new System.Random();
    //    var shuffledPlatforms = platforms.OrderBy(x => randomPlatform.Next()).ToArray();

    //    platformList = shuffledPlatforms.ToList();
    //    string platformsContents = string.Join(", ", platformList);
    //    Debug.Log(platformsContents);

    //    //Set Platform Positions
    //    float[] platformXPositions = new float[]
    //    {
    //        firstPlatformPosX,
    //        secondPlatformPosX,
    //        thirdPlatformPosX,
    //        lastPlatformPosX
    //    };

    //    SetPlatformPositions(platformXPositions);

    //    //Set Random Animal Statue Gem Order
    //    GameObject[] animals = new GameObject[] {rabbitStatue, elephantStatue, rhinoStatue, deerStatue};
    //    System.Random randomAnimal = new System.Random();
    //    var shuffledAnimals = animals.OrderBy(x => randomAnimal.Next()).ToArray();

    //    animalList = shuffledAnimals.ToList();
    //    string animalsContents = string.Join(", ", animalList);
    //    Debug.Log(animalsContents);

    //    SetGemOnAnimal();

    //    GenerateSolutionMap();
    //}

    //private void GenerateSolutionMap()
    //{
    //    Data.SolutionMap = new Dictionary<GameObject, int>();

    //    //Assign animal platform order depending on symbol order and gem on animal statue
    //    foreach (KeyValuePair<GameObject, int> entry in animalGemIndexMap)
    //    {
    //        GameObject statueObject = entry.Key;
    //        int activeGemIndex = entry.Value;

    //        GameObject targetSymbolObject = childSymbolOrder[activeGemIndex];

    //        int passwordSymbolOrder = passwordList.IndexOf(targetSymbolObject);

    //        GameObject animalPlatform = GetPlatformForStatue(statueObject);

    //        Data.SolutionMap.Add(animalPlatform, passwordSymbolOrder);
    //    }
    //}

    //private GameObject GetPlatformForStatue(GameObject statue)
    //{
    //    if (statue == rabbitStatue) return rabbitPlatform;
    //    if (statue == elephantStatue) return elephantPlatform;
    //    if (statue == rhinoStatue) return rhinoPlatform;
    //    if (statue == deerStatue) return deerPlatform;
    //    return null;
    //}

    //private void SetGemOnAnimal()
    //{
    //    for (int i = 0; i < animalList.Count; i++)
    //    {
    //        GameObject animalObject = animalList[i];
    //        int activeGemIndex = i;

    //        Transform selectedGem = animalObject.transform.GetChild(i);
    //        selectedGem.gameObject.SetActive(true);

    //        animalGemIndexMap.Add(animalObject, activeGemIndex);
    //    }
    //}

    //private void SetSymbolPositions(float[] symbolYPositions)
    //{
    //    for (int i = 0; i < passwordList.Count; i++)
    //    {
    //        GameObject symbolObject = passwordList[i];
    //        float symbolY = symbolYPositions[i];

    //        Vector3 currentPos = symbolObject.transform.localPosition;

    //        symbolObject.transform.localPosition = new Vector3(
    //            currentPos.x,
    //            symbolY,
    //            currentPos.z
    //        );
    //    }
    //}

    //private void SetPlatformPositions(float[] platformXPositions)
    //{
    //    for (int i = 0; i < platformList.Count; i++)
    //    {
    //        GameObject platformObject = platformList[i];
    //        float platformX = platformXPositions[i];

    //        Vector3 currentPos = platformObject.transform.localPosition;

    //        platformObject.transform.localPosition = new Vector3(
    //            platformX,
    //            currentPos.y,
    //            currentPos.z
    //        );
    //    }
    //}
}
