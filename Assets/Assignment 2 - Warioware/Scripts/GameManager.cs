using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string[] gameSceneNames;
    public GridPlacementSystem gridPlacementSystem;
    public Timer timer;
    [Space(10)]

    public Canvas TimeOverCanvas;
    public Canvas InstructionCanvas;

    private int randomIndex;
    private bool hasHandledTimeOver;

    private void Start()
    {
        hasHandledTimeOver = false;

        TimeOverCanvas.gameObject.SetActive(false);
        InstructionCanvas.gameObject.SetActive(false);
    }

    public void LoadRandomGame()
    {
        randomIndex = Random.Range(0, gameSceneNames.Length);

        if (gridPlacementSystem != null)
        {
            gridPlacementSystem.paintMode = Random.Range(0, 4);
            gridPlacementSystem.paintLevel = Random.Range(0, 2);
        }

        if (hasHandledTimeOver) return; //Stop duplicating calls

        if (timer.isTimeOver)
        {
            hasHandledTimeOver = true;
            TimeOverCanvas.gameObject.SetActive(true);
            StartCoroutine(DelayAfterTimeOver(2f));
        }
        else
        {
            ShowInstructions();
        }
    }

    private IEnumerator DelayAfterTimeOver(float delay)
    {
        yield return new WaitForSeconds(delay);

        TimeOverCanvas.gameObject.SetActive(false);

        ShowInstructions();
    }

    private void ShowInstructions()
    {
        InstructionCanvas.gameObject.SetActive(true);

        StartCoroutine(DelayAfterInstructions(5f));
    }

    private IEnumerator DelayAfterInstructions(float delay)
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(gameSceneNames[randomIndex]);
    }
}
