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
    public GameObject timerObject;
    [Space(10)]

    public Canvas TimeOverCanvas;
    public Canvas InstructionCanvas;
    public Canvas GameOverCanvas;
    public Canvas CompleteCanvas;
    [Space(10)]

    public TextMeshProUGUI changeText;
    public TextMeshProUGUI instructionsText;
    [Space(10)]

    public GameObject healthParent;
    public int currentHealth;
    [Space(10)]

    private int randomIndex;
    private bool hasLoadedScene;
    private string nextScene;

    public int paintMode;
    public int malletMode;
    public int screwMode;
    public int level;

    public Image clockCircle;
    public Image antiCircle;
    public Image clockRect;
    public Image antiRect;

    private void Start()
    {
        clockCircle.enabled = false;
        antiCircle.enabled = false;
        clockRect.enabled = false;
        antiRect.enabled = false;

        hasLoadedScene = false;

        TimeOverCanvas.gameObject.SetActive(false);
        InstructionCanvas.gameObject.SetActive(false);
        GameOverCanvas.gameObject.SetActive(false);

        timerObject.SetActive(true);
    }

    private void Update()
    {
        currentHealth = Data.globalHealthAmount;

        //Remove health sprites when health decreases
        for (int i = 0; i < healthParent.transform.childCount; i++)
        {
            if (i >= currentHealth)
            {
                healthParent.transform.GetChild(i).gameObject.SetActive(false);
            }
            else
            {
                healthParent.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        
        if (currentHealth <= 0 && !Data.globalHasGameOver)
        {
            Data.globalHasGameOver = true;

            StartCoroutine(DelayBeforeGameOver(1.5f));
        }
    }

    private IEnumerator DelayBeforeGameOver(float delay)
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene("End Scene");
    }

    public void LoadRandomGame()
    {
        Debug.Log("Current game level: " + Data.globalLevel);

        if (hasLoadedScene) return; //Stop duplicating calls

        //Set level change
        if (Data.globalLevel == 1 && Data.globalConsecutiveRound > 4)
        {
            Data.globalLevel = 2;

            changeText.text = ("SPEED UP!");

            Data.globalConsecutiveRound = 0;
        }
        else if (Data.globalLevel == 2 && Data.globalConsecutiveRound > 3)
        {
            Data.globalLevel = 3;

            changeText.text = ("LEVEL UP!");

            Data.globalConsecutiveRound = 0;
        }
        else if (Data.globalLevel == 3 && Data.globalConsecutiveRound > 3)
        {
            Data.globalLevel = 4;

            changeText.text = ("SPEED UP!");

            Data.globalConsecutiveRound = 0;
        }
        else if (Data.globalLevel == 4 && Data.globalConsecutiveRound > 3)
        {
            Data.globalConsecutiveRound = 0;
            CompleteGame();
            return;
        }

        randomIndex = Random.Range(0, gameSceneNames.Length);
        nextScene = gameSceneNames[randomIndex];

        //Set paint mode
        if (Data.globalLevel == 1 || Data.globalLevel == 2)
        {
            paintMode = Random.Range(0, 4);
        }
        else if (Data.globalLevel == 3 || Data.globalLevel == 4)
        {
            paintMode = Random.Range(1, 3);
        }

        Data.globalPaintMode = paintMode;

        //Set mallet mode
        malletMode = Random.Range(0, 2);

        Data.globalMalletMode = malletMode;

        //Set screw mode
        screwMode = Random.Range(0, 4);

        Data.globalScrewMode = screwMode;

        if (timer.isTimeOver)
        {
            hasLoadedScene = true;
            TimeOverCanvas.gameObject.SetActive(true);
            timerObject.SetActive(false);

            Data.globalConsecutiveRound = 0;
            LoseHealth();

            StartCoroutine(DelayAfterTimeOver(2f));
        }
        else
        {
            if (Data.globalConsecutiveRound <= 3 || Data.globalLevel != 4)
            {
                hasLoadedScene = true;

                timerObject.SetActive(false);
                ShowInstructions();
            }
            else
            {
                CompleteGame();
            }
        }
    }

    void CompleteGame()
    {
        Debug.Log("Complete building!");

        SceneManager.LoadScene("End Scene");
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
            if (malletMode == 0)
            {
                instructionsText.text = ("Hit all the nails down!");
            }
            else if (malletMode == 1)
            {
                instructionsText.text = ("Hit all the red nails down!");
            }
        }
        else if (nextScene == "Screw Scene")
        {
            Debug.Log(nextScene);

            if (screwMode == 0)
            {
                instructionsText.text = ("Screw the screw by turning it clockwise!");
                clockCircle.enabled = true;
            }
            else if (screwMode == 1)
            {
                instructionsText.text = ("Screw the screw by turning it anticlockwise!");
                antiCircle.enabled = true;
            }
            else if (screwMode == 2)
            {
                instructionsText.text = ("Screw the screw by turning it clockwise in a rectangular shape!");
                clockRect.enabled = true;
            }
            else if (screwMode == 3)
            {
                instructionsText.text = ("Screw the screw by turning it anticlockwise in a rectangular shape!");
                antiRect.enabled = true;
            }
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
