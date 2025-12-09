using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SeventhPuzzleManager : MonoBehaviour
{
    [Header("Animal Statue Parent Objects")]
    public GameObject rabbitStatueParent1;
    public GameObject rabbitStatueParent2;
    public GameObject rabbitStatueParent3;
    public GameObject rabbitStatueParent4;
    public GameObject elephantStatueParent1;
    public GameObject elephantStatueParent2;
    public GameObject elephantStatueParent3;
    public GameObject elephantStatueParent4;
    public GameObject rhinoStatueParent1;
    public GameObject rhinoStatueParent2;
    public GameObject rhinoStatueParent3;
    public GameObject rhinoStatueParent4;
    public GameObject deerStatueParent1;
    public GameObject deerStatueParent2;
    public GameObject deerStatueParent3;
    public GameObject deerStatueParent4;

    private GameObject rabbitStatue1;
    private GameObject rabbitStatue2;
    private GameObject rabbitStatue3;
    private GameObject rabbitStatue4;
    private GameObject elephantStatue1;
    private GameObject elephantStatue2;
    private GameObject elephantStatue3;
    private GameObject elephantStatue4;
    private GameObject rhinoStatue1;
    private GameObject rhinoStatue2;
    private GameObject rhinoStatue3;
    private GameObject rhinoStatue4;
    private GameObject deerStatue1;
    private GameObject deerStatue2;
    private GameObject deerStatue3;
    private GameObject deerStatue4;

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
    public float fourthStatueParentRotY;
    public float fifthStatueParentRotY;
    public float sixthStatueParentRotY;
    public float seventhStatueParentRotY;
    public float eigthStatueParentRotY;
    public float ninthStatueParentRotY;
    public float tenthStatueParentRotY;
    public float eleventhStatueParentRotY;
    public float twelfthStatueParentRotY;
    public float thirteenthStatueParentRotY;
    public float fourteenthStatueParentRotY;
    public float fifteenthStatueParentRotY;
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
    public Material yellowMaterial;
    public Material whiteMaterial;

    private Material[] gemMaterials;

    private Color[] passwordColors = new Color[]
    {
    Color.green,
    Color.blue,
    new Color(0.812f, 0f, 1f),
    Color.yellow
    };

    private Color[] shuffledPasswordColors;
    private int[] shuffledColorIndices;

    private int chosenGemIndex;            

    private List<GameObject> passwordList;
    private List<GameObject> platformList;
    private List<GameObject> animalList;

    private List<GameObject[]> animalGroups;

    private GameObject[] symbolByGem;

    private Dictionary<GameObject, int> coloredStatueToPasswordIndex;

    void Awake()
    {
        //Assign statue from parent
        rabbitStatue1 = rabbitStatueParent1.transform.GetChild(0).gameObject;
        rabbitStatue2 = rabbitStatueParent2.transform.GetChild(0).gameObject;
        rabbitStatue3 = rabbitStatueParent3.transform.GetChild(0).gameObject;
        rabbitStatue4 = rabbitStatueParent4.transform.GetChild(0).gameObject;

        elephantStatue1 = elephantStatueParent1.transform.GetChild(0).gameObject;
        elephantStatue2 = elephantStatueParent2.transform.GetChild(0).gameObject;
        elephantStatue3 = elephantStatueParent3.transform.GetChild(0).gameObject;
        elephantStatue4 = elephantStatueParent4.transform.GetChild(0).gameObject;

        rhinoStatue1 = rhinoStatueParent1.transform.GetChild(0).gameObject;
        rhinoStatue2 = rhinoStatueParent2.transform.GetChild(0).gameObject;
        rhinoStatue3 = rhinoStatueParent3.transform.GetChild(0).gameObject;
        rhinoStatue4 = rhinoStatueParent4.transform.GetChild(0).gameObject;

        deerStatue1 = deerStatueParent1.transform.GetChild(0).gameObject;
        deerStatue2 = deerStatueParent2.transform.GetChild(0).gameObject;
        deerStatue3 = deerStatueParent3.transform.GetChild(0).gameObject;
        deerStatue4 = deerStatueParent4.transform.GetChild(0).gameObject;

        //Assign platform from parent
        rabbitPlatform = rabbitPlatformParent.transform.GetChild(0).gameObject;
        elephantPlatform = elephantPlatformParent.transform.GetChild(0).gameObject;
        rhinoPlatform = rhinoPlatformParent.transform.GetChild(0).gameObject;
        deerPlatform = deerPlatformParent.transform.GetChild(0).gameObject;

        chosenGemIndex = UnityEngine.Random.Range(0, 4);

        gemMaterials = new Material[] { greenMaterial, blueMaterial, purpleMaterial, yellowMaterial };

        //Reset Solution Map
        if (Data.SeventhSolutionMap != null) Data.SeventhSolutionMap.Clear();

        coloredStatueToPasswordIndex = new Dictionary<GameObject, int>();
        symbolByGem = new GameObject[] { rectSymbol, tearSymbol, hexSymbol, circSymbol };

        //Set Symbol Displayed
        GameObject selectedSymbol = GetSymbolForGem(chosenGemIndex);

        //Put each animal statue into groups
        animalGroups = new List<GameObject[]>
        {
            new GameObject[] { rabbitStatue1, rabbitStatue2, rabbitStatue3, rabbitStatue4 },
            new GameObject[] { elephantStatue1, elephantStatue2, elephantStatue3, elephantStatue4 },
            new GameObject[] { rhinoStatue1, rhinoStatue2, rhinoStatue3, rhinoStatue4 },
            new GameObject[] { deerStatue1, deerStatue2, deerStatue3, deerStatue4 }
        };
        
        //Disable all gems on statue
        foreach (var group in animalGroups)
        {
            foreach (var statue in group)
            {
                foreach (Transform child in statue.transform)
                    child.gameObject.SetActive(false);
            }
        }

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

        shuffledColorIndices = Enumerable.Range(0, 4)
                                 .OrderBy(x => UnityEngine.Random.value)
                                 .ToArray();

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

        //Set Platform Rotation
        float[] platformParentYRotations = new float[]
        {
            firstPlatformRotY,
            secondPlatformRotY,
            thirdPlatformRotY,
            lastPlatformRotY
        };

        SetPlatformParentRotations(platformParentYRotations);

        //Set Random Animal Statue Gem Order
        GameObject[] animals = new GameObject[]
        {
            rabbitStatue1, rabbitStatue2, rabbitStatue3, rabbitStatue4,
            elephantStatue1, elephantStatue2, elephantStatue3, elephantStatue4,
            rhinoStatue1, rhinoStatue2, rhinoStatue3, rhinoStatue4,
            deerStatue1, deerStatue2, deerStatue3, deerStatue4
        };
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
            fourthStatueParentRotY,
            fifthStatueParentRotY,
            sixthStatueParentRotY,
            seventhStatueParentRotY,
            eigthStatueParentRotY,
            ninthStatueParentRotY,
            tenthStatueParentRotY,
            eleventhStatueParentRotY,
            twelfthStatueParentRotY,
            thirteenthStatueParentRotY,
            fourteenthStatueParentRotY,
            fifteenthStatueParentRotY,
            lastStatueParentRotY
        };

        SetStatueParentRotations(statueParentsYRotations);

        AssignGemsToStatues();

        GenerateSolutionMap();
    }

    private void GenerateSolutionMap()
    {
        Data.SeventhSolutionMap = new Dictionary<GameObject, int>();

        //Assign animal platform order depending on symbol order and gem on animal statue
        foreach (KeyValuePair<GameObject, int> entry in coloredStatueToPasswordIndex)
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
            Data.SeventhSolutionMap.Add(platform, passwordOrder);
        }
    }

    private GameObject GetSymbolForGem(int gemIndex)
    {
        return symbolByGem[gemIndex];
    }

    private GameObject GetPlatformForStatue(GameObject statue)
    {
        if (statue == rabbitStatue1 || statue == rabbitStatue2 || statue == rabbitStatue3 || statue == rabbitStatue4) return rabbitPlatform;
        if (statue == elephantStatue1 || statue == elephantStatue2 || statue == elephantStatue3 || statue == elephantStatue4) return elephantPlatform;
        if (statue == rhinoStatue1 || statue == rhinoStatue2 || statue == rhinoStatue3 || statue == rhinoStatue4) return rhinoPlatform;
        if (statue == deerStatue1 || statue == deerStatue2 || statue == deerStatue3 || statue == deerStatue4) return deerPlatform;
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

    private void AssignGemsToStatues()
    {
        for (int animalIndex = 0; animalIndex < animalGroups.Count; animalIndex++)
        {
            GameObject[] group = animalGroups[animalIndex];

            int coloredStatueLocalIndex = UnityEngine.Random.Range(0, 3);

            int colorMaterialIndex = shuffledColorIndices[animalIndex];
            Material coloredMaterial = gemMaterials[colorMaterialIndex];

            for (int i = 0; i < 3; i++)
            {
                GameObject statue = group[i];

                for (int childIdx = 0; childIdx < statue.transform.childCount; childIdx++)
                {
                    Transform gemChild = statue.transform.GetChild(childIdx);
                    gemChild.gameObject.SetActive(childIdx == chosenGemIndex);
                }

                Transform gemTransform = statue.transform.GetChild(chosenGemIndex);
                Renderer gemRenderer = gemTransform.GetComponent<Renderer>();

                if (i == coloredStatueLocalIndex)
                {
                    gemRenderer.material = coloredMaterial;
                    coloredStatueToPasswordIndex[statue] = colorMaterialIndex;
                }
                else
                {
                    gemRenderer.material = whiteMaterial;
                }
            }
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
