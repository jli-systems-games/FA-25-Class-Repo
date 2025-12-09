using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimalPlatformCheck : MonoBehaviour
{
    public float resetTime;

    private Color emissionColor;

    private int platformClickedCount;
    private int currentStepIndex;

    private Dictionary<GameObject, bool> secondPlatformClickedStatus;
    private Dictionary<GameObject, bool> thirdPlatformClickedStatus;
    private Dictionary<GameObject, bool> fourthPlatformClickedStatus;
    private Dictionary<GameObject, bool> fifthPlatformClickedStatus;
    private Dictionary<GameObject, bool> sixthPlatformClickedStatus;
    private Dictionary<GameObject, bool> seventhPlatformClickedStatus;

    private bool isFalseOrder;

    public IslandStageManager stageManager;

    private void Start()
    {
        secondPlatformClickedStatus = new Dictionary<GameObject, bool>();
        thirdPlatformClickedStatus = new Dictionary<GameObject, bool>();
        fourthPlatformClickedStatus = new Dictionary<GameObject, bool>();
        fifthPlatformClickedStatus = new Dictionary<GameObject, bool>();
        sixthPlatformClickedStatus = new Dictionary<GameObject, bool>();
        seventhPlatformClickedStatus = new Dictionary<GameObject, bool>();

        ResetVariables(Data.SecondSolutionMap, "Second Platform", secondPlatformClickedStatus);
        ResetVariables(Data.ThirdSolutionMap, "Third Platform", thirdPlatformClickedStatus);
        ResetVariables(Data.FourthSolutionMap, "Fourth Platform", fourthPlatformClickedStatus);
        ResetVariables(Data.FifthSolutionMap, "Fifth Platform", fifthPlatformClickedStatus);
        ResetVariables(Data.SixthSolutionMap, "Sixth Platform", sixthPlatformClickedStatus);
        ResetVariables(Data.SeventhSolutionMap, "Seventh Platform", seventhPlatformClickedStatus);
    }

    private void ResetVariables(Dictionary<GameObject, int> solutionMap, string tagName, Dictionary<GameObject, bool> platformClickedStatus)
    {
        platformClickedStatus.Clear();

        foreach (GameObject platformKey in solutionMap.Keys)
        {
            platformClickedStatus[platformKey] = false;
        }

        PlatformAllAppearance(tagName, Color.black);

        platformClickedCount = 0;
        currentStepIndex = 0;
        isFalseOrder = false;
    }

    private void ResetPreviousPuzzleVariables()
    {
        platformClickedCount = 0;
        currentStepIndex = 0;
        isFalseOrder = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject incomingPlatform = other.gameObject;
        
        if (other.gameObject.CompareTag("Second Platform"))
        {
            Debug.Log("Clicked second platform");
            PlatformOnClick(Data.SecondSolutionMap, other.gameObject, "Second Platform", 3, secondPlatformClickedStatus);
        }
        else if (other.gameObject.CompareTag("Third Platform"))
        {
            Debug.Log("Clicked third platform");
            PlatformOnClick(Data.ThirdSolutionMap, other.gameObject, "Third Platform", 3, thirdPlatformClickedStatus);
        }
        else if (other.gameObject.CompareTag("Fourth Platform"))
        {
            Debug.Log("Clicked fourth platform");
            PlatformOnClick(Data.FourthSolutionMap, other.gameObject, "Fourth Platform", 3, fourthPlatformClickedStatus);
        }
        else if (other.gameObject.CompareTag("Fifth Platform"))
        {
            Debug.Log("Clicked fifth platform");
            PlatformOnClick(Data.FifthSolutionMap, other.gameObject, "Fifth Platform", 4, fifthPlatformClickedStatus);
        }
        else if (other.gameObject.CompareTag("Sixth Platform"))
        {
            Debug.Log("Clicked sixth platform");
            PlatformOnClick(Data.SixthSolutionMap, other.gameObject, "Sixth Platform", 4, sixthPlatformClickedStatus);
        }
        else if (other.gameObject.CompareTag("Seventh Platform"))
        {
            Debug.Log("Clicked seventh platform");
            PlatformOnClick(Data.SeventhSolutionMap, other.gameObject, "Seventh Platform", 4, seventhPlatformClickedStatus);
        }
    }

    private void PlatformOnClick(Dictionary<GameObject, int> solutionMap, GameObject platformObject, string tagName, int platformCount, Dictionary<GameObject, bool> platformClickedStatus)
    {
        if (solutionMap.ContainsKey(platformObject))
        {
            if (!platformClickedStatus[platformObject])
            {
                CheckOrder(solutionMap, platformObject, tagName, platformCount, platformClickedStatus);
            }
        }
    }

    private void CheckOrder(Dictionary<GameObject, int> solutionMap, GameObject clickedPlatform, string tagName, int platformCount, Dictionary<GameObject, bool> platformClickedStatus)
    {
        int requiredStep = solutionMap[clickedPlatform];

        platformClickedStatus[clickedPlatform] = true;
        PlatformClickedAppearance(clickedPlatform);

        platformClickedCount++;

        if (requiredStep == currentStepIndex)
        {
            Debug.Log("Correct platform order clicked!");

            currentStepIndex++;
        }
        else
        {
            Debug.Log("Incorrect platform order clicked!");

            isFalseOrder = true;
        }

        if (platformClickedCount == platformCount)
        {
            if (!isFalseOrder)
            {
                Debug.Log("Puzzle Solved");

                StartCoroutine(PuzzleCorrect(tagName));
            }
            else
            {
                Debug.Log("Puzzle incorrect");

                StartCoroutine(PuzzleIncorrectReset(solutionMap, tagName, platformClickedStatus));
            }
        }

        Debug.Log(currentStepIndex);
    }

    private IEnumerator PuzzleCorrect(string tagName)
    {
        yield return new WaitForSeconds(1f);

        PlatformAllAppearance(tagName, Color.green);

        if (tagName == "Second Platform")
        {
            Data.secondPuzzleSolved = true;
        }
        else if (tagName == "Third Platform")
        {
            Data.thirdPuzzleSolved = true;
        }
        else if (tagName == "Fourth Platform")
        {
            Data.fourthPuzzleSolved = true;
        }
        else if (tagName == "Fifth Platform")
        {
            Data.fifthPuzzleSolved = true;
        }
        else if (tagName == "Sixth Platform")
        {
            Data.sixthPuzzleSolved = true;
        }
        else if (tagName == "Seventh Platform")
        {
            Data.seventhPuzzleSolved = true;
        }

        stageManager.CheckPuzzleSolved();

        ResetPreviousPuzzleVariables();
    }

    private IEnumerator PuzzleIncorrectReset(Dictionary<GameObject, int> solutionMap, string tagName, Dictionary<GameObject, bool> platformClickedStatus)
    {
        yield return new WaitForSeconds(1f);

        PlatformAllAppearance(tagName, Color.red);

        yield return new WaitForSeconds(resetTime);

        ResetVariables(solutionMap, tagName, platformClickedStatus);
    }

    public IEnumerator PuzzleEnd(string tagName, Color col)
    {
        yield return new WaitForSeconds(1f);

        PlatformAllAppearance(tagName, col);

        stageManager.CheckPuzzleSolved();
    }

    public void PlatformAllAppearance(string tagName, Color endColor)
    {
        GameObject[] allPlatformsArray = GameObject.FindGameObjectsWithTag(tagName);

        foreach (GameObject platform in allPlatformsArray)
        {
            Renderer platformRenderer = platform.GetComponent<Renderer>();
            Material platformMaterial = platformRenderer.material;
            emissionColor = platformMaterial.GetColor("_EmissionColor");
            emissionColor = endColor;
            platformMaterial.SetColor("_EmissionColor", emissionColor);
        }
    }

    public void PlatformClickedAppearance(GameObject platform)
    {
        Renderer platformRenderer = platform.GetComponent<Renderer>();
        Material platformMaterial = platformRenderer.material;
        emissionColor = platformMaterial.GetColor("_EmissionColor");
        emissionColor = Color.yellow;
        platformMaterial.SetColor("_EmissionColor", emissionColor);
    }

    //private void Start()
    //{
    //    platformClickedStatus = new Dictionary<GameObject, bool>();

    //    ResetVariables();
    //}

    //private void Update()
    //{
    //    if (platformClickedCount == 4)
    //    {
    //        Debug.Log("All platform clicked!");
    //    }
    //}

    //private void ResetVariables()
    //{
    //    platformClickedStatus.Clear();

    //    foreach (GameObject platformKey in Data.SolutionMap.Keys)
    //    {
    //        platformClickedStatus.Add(platformKey, false);

    //        PlatformAllAppearance("Animal Platform", Color.black);
    //    }

    //    platformClickedCount = 0;
    //    currentStepIndex = 0;

    //    isFalseOrder = false;
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    GameObject incomingPlatform = other.gameObject;

    //    if (other.gameObject.CompareTag("Animal Platform"))
    //    {
    //        if (Data.SolutionMap.ContainsKey(other.gameObject))
    //        {
    //            if (!platformClickedStatus[other.gameObject])
    //            {
    //                CheckOrder(other.gameObject);
    //            }
    //        }
    //    }
    //}

    //private void CheckOrder(GameObject clickedPlatform)
    //{
    //    int requiredStep = Data.SolutionMap[clickedPlatform];

    //    platformClickedStatus[clickedPlatform] = true;
    //    PlatformClickedAppearance(clickedPlatform);

    //    platformClickedCount++;

    //    if (requiredStep == currentStepIndex)
    //    {
    //        Debug.Log("Correct platform order clicked!");

    //        currentStepIndex++;
    //    }
    //    else
    //    {
    //        Debug.Log("Incorrect platform order clicked!");

    //        isFalseOrder = true;
    //    }

    //    if (platformClickedCount == 4)
    //    {
    //        if (!isFalseOrder)
    //        {
    //            Debug.Log("Puzzle Solved");

    //            StartCoroutine(PuzzleCorrect());
    //        }
    //        else
    //        {
    //            Debug.Log("Puzzle incorrect");

    //            StartCoroutine(PuzzleIncorrectReset());
    //        }
    //    }

    //    Debug.Log(currentStepIndex);
    //}

    //private IEnumerator PuzzleCorrect()
    //{
    //    yield return new WaitForSeconds(1f);

    //    PlatformAllAppearance("Animal Platform", Color.green);

    //    yield return new WaitForSeconds(2f);

    //    SceneManager.LoadScene("End Scene");
    //}

    //private IEnumerator PuzzleIncorrectReset()
    //{
    //    yield return new WaitForSeconds(1f);

    //    PlatformAllAppearance("Animal Platform", Color.red);

    //    yield return new WaitForSeconds(resetTime);

    //    ResetVariables();
    //}

    //public IEnumerator PuzzleEnd(string tagName, Color col)
    //{
    //    yield return new WaitForSeconds(1f);

    //    PlatformAllAppearance(tagName, col);

    //    stageManager.CheckPuzzleSolved();
    //}

    //public void PlatformAllAppearance(string tagName, Color endColor)
    //{
    //    GameObject[] allPlatformsArray = GameObject.FindGameObjectsWithTag(tagName);

    //    foreach (GameObject platform in allPlatformsArray)
    //    {
    //        Renderer platformRenderer = platform.GetComponent<Renderer>();
    //        Material platformMaterial = platformRenderer.material;
    //        emissionColor = platformMaterial.GetColor("_EmissionColor");
    //        emissionColor = endColor;
    //        platformMaterial.SetColor("_EmissionColor", emissionColor);
    //    }
    //}

    //public void PlatformClickedAppearance(GameObject platform)
    //{
    //    Renderer platformRenderer = platform.GetComponent<Renderer>();
    //    Material platformMaterial = platformRenderer.material;
    //    emissionColor = platformMaterial.GetColor("_EmissionColor");
    //    emissionColor = Color.yellow;
    //    platformMaterial.SetColor("_EmissionColor", emissionColor);
    //}
}
