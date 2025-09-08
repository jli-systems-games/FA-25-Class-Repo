using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using JetBrains.Annotations;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string[] gameSceneNames;
    public Timer timer;
    [Space(10)]

    public Canvas TimeOverCanvas;
    public Canvas InstructionCanvas;
    public Canvas GameOverCanvas;
    [Space(10)]

    public TextMeshProUGUI instructionsText;
    [Space(10)]

    public GameObject healthParent;
    public int currentHealth;
    [Space(10)]

    public bool hasGameOver = false;

    private int randomIndex;
    private bool hasLoadedScene;
    private string nextScene;

    public int paintMode;
    public int paintLevel;

    private void Start()
    {
        hasLoadedScene = false;

        TimeOverCanvas.gameObject.SetActive(false);
        InstructionCanvas.gameObject.SetActive(false);
        GameOverCanvas.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (hasGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //Restart game
            }
        }

        Debug.Log("Child 0: " + healthParent.transform.GetChild(0).name);
        Debug.Log("Child 1: " + healthParent.transform.GetChild(1).name);
        Debug.Log("Child 2: " + healthParent.transform.GetChild(2).name);

        //Remove health sprites when health decreases
        if (currentHealth == 2)
        {
            healthParent.transform.GetChild(0).GetComponent<Image>().enabled = false;
        }
        else if (currentHealth == 1)
        {
            healthParent.transform.GetChild(0).GetComponent<Image>().enabled = false;
        }
        else if (Data.globalHealthAmount == 0)
        {
            healthParent.transform.GetChild(0).GetComponent<Image>().enabled = false;
            GameOverCanvas.gameObject.SetActive(true);
            hasGameOver = true;
        }

        Debug.Log("Current health = " + Data.globalHealthAmount);
    }

    public void LoadRandomGame()
    {
        if (hasLoadedScene) return; //Stop duplicating calls

        randomIndex = Random.Range(0, gameSceneNames.Length);
        nextScene = gameSceneNames[randomIndex];

        paintMode = Random.Range(0, 4);
        paintLevel = Random.Range(0, 1);

        Data.globalPaintMode = paintMode;
        Data.globalPaintLevel = paintLevel;

        Debug.Log("Paint Mode set to: " + paintMode);
        

        if (timer.isTimeOver)
        {
            hasLoadedScene = true;
            TimeOverCanvas.gameObject.SetActive(true);

            LoseHealth();

            StartCoroutine(DelayAfterTimeOver(2f));
        }
        else
        {
            hasLoadedScene = true;
            timer.isTimerRunning = false;

            ShowInstructions();
        }
    }

    private IEnumerator DelayAfterTimeOver(float delay)
    {
        yield return new WaitForSeconds(delay);

        TimeOverCanvas.gameObject.SetActive(false);

        ShowInstructions();
    }

    private void LoseHealth()
    {
        currentHealth = Data.globalHealthAmount - 1;
        Data.globalHealthAmount = currentHealth;
    }

    private void ShowInstructions()
    {
        if (nextScene == "Mallet Scene")
        {
            Debug.Log(nextScene);
            instructionsText.text = ("Hit all the nails down!");
        }
        else if (nextScene == "Screw Scene")
        {
            Debug.Log(nextScene);
            instructionsText.text = ("Screw the screw by turning it clockwise!");
        }
        else if (nextScene == "Paint Scene")
        {
            Debug.Log(nextScene);
            if (paintMode == 0)
            {
                instructionsText.text = ("Paint the whole wall!");
            }
            else if (paintMode == 1)
            {
                instructionsText.text = ("Paint vertical stripes on the wall!");
            }
            else if (paintMode == 2)
            {
                instructionsText.text = ("Paint horizontal stripes on the wall!");
            }
            else if (paintMode == 3)
            {
                instructionsText.text = ("Paint a checkerboard pattern on the wall!");
            }
        }

        InstructionCanvas.gameObject.SetActive(true);

        StartCoroutine(DelayAfterInstructions(5f));
    }

    private IEnumerator DelayAfterInstructions(float delay)
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(gameSceneNames[randomIndex]);
    }
}
