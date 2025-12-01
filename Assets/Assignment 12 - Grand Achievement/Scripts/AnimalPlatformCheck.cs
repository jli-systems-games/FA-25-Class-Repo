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

    private Dictionary<GameObject, bool> platformClickedStatus;

    private bool isFalseOrder;

    public PuzzleStageManager stageManager;

    private void Start()
    {
        platformClickedStatus = new Dictionary<GameObject, bool>();

        ResetVariables();
    }

    private void Update()
    {
        if (platformClickedCount == 4)
        {
            Debug.Log("All platform clicked!");
        }
    }

    private void ResetVariables()
    {
        platformClickedStatus.Clear();

        foreach (GameObject platformKey in Data.SolutionMap.Keys)
        {
            platformClickedStatus.Add(platformKey, false);

            PlatformAllAppearance("Animal Platform", Color.black);
        }

        platformClickedCount = 0;
        currentStepIndex = 0;

        isFalseOrder = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject incomingPlatform = other.gameObject;

        if (other.gameObject.CompareTag("Animal Platform"))
        {
            if (Data.SolutionMap.ContainsKey(other.gameObject))
            {
                if (!platformClickedStatus[other.gameObject])
                {
                    CheckOrder(other.gameObject);
                }
            }
        }
    }

    private void CheckOrder(GameObject clickedPlatform)
    {
        int requiredStep = Data.SolutionMap[clickedPlatform];

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

        if (platformClickedCount == 4)
        {
            if (!isFalseOrder)
            {
                Debug.Log("Puzzle Solved");

                StartCoroutine(PuzzleCorrect());
            }
            else
            {
                Debug.Log("Puzzle incorrect");

                StartCoroutine(PuzzleIncorrectReset());
            }
        }

        Debug.Log(currentStepIndex);
    }

    private IEnumerator PuzzleCorrect()
    {
        yield return new WaitForSeconds(1f);

        PlatformAllAppearance("Animal Platform", Color.green);

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("End Scene");
    }

    private IEnumerator PuzzleIncorrectReset()
    {
        yield return new WaitForSeconds(1f);

        PlatformAllAppearance("Animal Platform", Color.red);

        yield return new WaitForSeconds(resetTime);

        ResetVariables();
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
}
