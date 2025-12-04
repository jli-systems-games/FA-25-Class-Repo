using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FourthPuzzleManager : MonoBehaviour
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
    public float firstPlatformPosZ;
    public float secondPlatformPosZ;
    public float thirdPlatformPosZ;
    public float lastPlatformPosZ;

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

    [Header("Gem Materials")]
    public Material greenMaterial;
    public Material blueMaterial;
    public Material purpleMaterial;
    public Material whiteMaterial;

    private Material[] gemMaterials;

    private Color[] passwordColors = new Color[]
    {
    Color.green,
    Color.blue,
    new Color(0.812f, 0f, 1f),
    Color.white
    };

    private Color[] shuffledPasswordColors;

    private int chosenGemIndex;

    private List<GameObject> passwordList;
    private List<GameObject> platformList;
    private List<GameObject> animalList;

    private Dictionary<GameObject, int> animalGemIndexMap;
    private GameObject[] symbolByGem;

    void Awake()
    {
        //Assign statue from parent
        rabbitStatue = rabbitStatueParent.transform.GetChild(0).gameObject;
        elephantStatue = elephantStatueParent.transform.GetChild(0).gameObject;
        rhinoStatue = rhinoStatueParent.transform.GetChild(0).gameObject;
        deerStatue = deerStatueParent.transform.GetChild(0).gameObject;

        chosenGemIndex = UnityEngine.Random.Range(0, 4);

        gemMaterials = new Material[] { greenMaterial, blueMaterial, purpleMaterial, whiteMaterial };

        //Reset Solution Map
        if (Data.FourthSolutionMap != null) Data.FourthSolutionMap.Clear();

        animalGemIndexMap = new Dictionary<GameObject, int>();
        symbolByGem = new GameObject[] { rectSymbol, tearSymbol, hexSymbol, circSymbol };

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

        //Set Symbol Displayed
        GameObject selectedSymbol = GetSymbolForGem(chosenGemIndex);

        // Create 4 copies of symbol
        passwordList = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            GameObject newSymbol = Instantiate(selectedSymbol, selectedSymbol.transform.parent);
            newSymbol.name = selectedSymbol.name + i;
            newSymbol.SetActive(true);
            passwordList.Add(newSymbol);
        }

        passwordList = passwordList.OrderBy(x => UnityEngine.Random.value).ToList();


        //Assign Password Colors
        shuffledPasswordColors = passwordColors.OrderBy(x => UnityEngine.Random.value).ToArray();

        AssignSymbolColors();

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
        float[] platformZPositions = new float[]
        {
            firstPlatformPosZ,
            secondPlatformPosZ,
            thirdPlatformPosZ,
            lastPlatformPosZ
        };

        SetPlatformPositions(platformZPositions);

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

        SetGemsOnAnimals();

        GenerateSolutionMap();
    }

    private void GenerateSolutionMap()
    {
        Data.FourthSolutionMap = new Dictionary<GameObject, int>();

        //Assign animal platform order depending on symbol order and gem on animal statue
        foreach (KeyValuePair<GameObject, int> entry in animalGemIndexMap)
        {
            GameObject statueObject = entry.Key;
            int gemMaterialIndex = entry.Value;

            Color targetColor = passwordColors[gemMaterialIndex];

            int passwordOrder = -1;

            for (int i = 0; i < passwordList.Count; i++)
            {
                SpriteRenderer sr = passwordList[i].GetComponent<SpriteRenderer>();
                if (sr != null && sr.color == targetColor)
                {
                    passwordOrder = i;
                    break;
                }
            }

            GameObject platform = GetPlatformForStatue(statueObject);
            Data.FourthSolutionMap.Add(platform, passwordOrder);
        }
    }

    private GameObject GetSymbolForGem(int gemIndex)
    {
        return symbolByGem[gemIndex];
    }

    private GameObject GetPlatformForStatue(GameObject statue)
    {
        if (statue == rabbitStatue) return rabbitPlatform;
        if (statue == elephantStatue) return elephantPlatform;
        if (statue == rhinoStatue) return rhinoPlatform;
        if (statue == deerStatue) return deerPlatform;
        return null;
    }

    private void AssignSymbolColors()
    {
        for (int i = 0; i < passwordList.Count; i++)
        {
            SpriteRenderer sr = passwordList[i].GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = shuffledPasswordColors[i];
        }
    }

    private void SetGemsOnAnimals()
    {

        Material[] shuffledGemMaterials = gemMaterials.OrderBy(x => UnityEngine.Random.value).ToArray();

        for (int i = 0; i < animalList.Count; i++)
        {
            GameObject animal = animalList[i];

            Transform gem = animal.transform.GetChild(chosenGemIndex);
            gem.gameObject.SetActive(true);

            Material mat = shuffledGemMaterials[i];
            gem.GetComponent<Renderer>().material = mat;

            int materialIndex = Array.IndexOf(gemMaterials, mat);

            animalGemIndexMap.Add(animal, materialIndex);
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

    private void SetPlatformPositions(float[] platformZPositions)
    {
        for (int i = 0; i < platformList.Count; i++)
        {
            GameObject platformObject = platformList[i];
            float platformZ = platformZPositions[i];

            Vector3 currentPos = platformObject.transform.localPosition;

            platformObject.transform.localPosition = new Vector3(
                currentPos.x,
                currentPos.y,
                platformZ
            );
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
