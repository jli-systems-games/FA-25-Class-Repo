using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScrewControl : MonoBehaviour
{
    public GameObject circleScrew;
    public GameObject squareScrew;

    private float screwPositionAmount;
    public float screwRotationAmount = 360f;
    public float screwFinalPosition = -1.351f;
    
    private bool firstStepComplete = false;
    private bool secondStepComplete = false;
    private bool thirdStepComplete = false;
    private bool fourthStepComplete = false;
    private bool hasCompletedCircle = false;

    public GameManager gameManager;
    public Timer timer;
    public ParticleSystem confettiParticle;
    [Space(10)]

    private int screwMode;

    private bool isClockwiseMode = false;
    private bool isAntiClockwiseMode = false;
    private bool isSquareClockwiseMode = false;
    private bool isSquareAntiClockwiseMode = false;

    private GameObject selectedScrew;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        confettiParticle.gameObject.SetActive(false);

        screwMode = Data.globalScrewMode;

        if (screwMode == 0)
        {
            isClockwiseMode = true;
            circleScrew.SetActive(true);
            squareScrew.SetActive(false);
            selectedScrew = circleScrew;
        }
        else if (screwMode == 1)
        {
            isAntiClockwiseMode = true;
            circleScrew.SetActive(true);
            squareScrew.SetActive(false);
            selectedScrew = circleScrew;
        }
        else if (screwMode == 2)
        {
            isSquareClockwiseMode = true;
            circleScrew.SetActive(false);
            squareScrew.SetActive(true);
            selectedScrew = squareScrew;
        }
        else if (screwMode == 3)
        {
            isSquareAntiClockwiseMode = true;
            circleScrew.SetActive(false);
            squareScrew.SetActive(true);
            selectedScrew = squareScrew;
        }

        if (Data.globalLevel == 1 || Data.globalLevel == 2)
        {
            screwPositionAmount = 0.07f;

        }
        else if (Data.globalLevel == 3 || Data.globalLevel == 4)
        {
            screwPositionAmount = 0.03f;
        }
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;

        if (isClockwiseMode)
        {
            if (mousePos.y > Screen.height * 3 / 4f && mousePos.x > Screen.width / 3f && mousePos.x < Screen.width * 2f / 3f) //Top
            {
                firstStepComplete = true;
                secondStepComplete = false;
                thirdStepComplete = false;
                Debug.Log("First step complete");

                if (fourthStepComplete)
                {
                    hasCompletedCircle = true;
                    fourthStepComplete = false;
                    Debug.Log("Circle complete");
                }
            }
            else if (mousePos.x > Screen.width * 2 / 3f && mousePos.y > Screen.height / 4f && mousePos.y < Screen.height * 3f / 4f) //Right
            {
                thirdStepComplete = false;
                fourthStepComplete = false;

                if (firstStepComplete)
                {
                    secondStepComplete = true;
                    firstStepComplete = false;

                    Debug.Log("Second step complete");
                }
            }
            else if (mousePos.y < Screen.height / 4f && mousePos.x > Screen.width / 3f && mousePos.x < Screen.width * 2f / 3f) //Bottom
            {
                firstStepComplete = false;
                fourthStepComplete = false;

                if (secondStepComplete)
                {
                    thirdStepComplete = true;
                    secondStepComplete = false;
                    Debug.Log("Third step complete");
                }
            }
            else if (mousePos.x < Screen.width / 3f && mousePos.y > Screen.height / 4f && mousePos.y < Screen.height * 3f / 4f) //Left
            {
                firstStepComplete = false;
                secondStepComplete = false;

                if (thirdStepComplete)
                {
                    fourthStepComplete = true;
                    thirdStepComplete = false;
                    Debug.Log("Fourth step complete");
                }
            }
        }
        else if (isAntiClockwiseMode)
        {
            if (mousePos.y > Screen.height * 3 / 4f && mousePos.x > Screen.width / 3f && mousePos.x < Screen.width * 2f / 3f) //Top
            {
                firstStepComplete = true;
                secondStepComplete = false;
                thirdStepComplete = false;
                Debug.Log("First step complete");

                if (fourthStepComplete)
                {
                    hasCompletedCircle = true;
                    fourthStepComplete = false;
                    Debug.Log("Circle complete");
                }
            }
            else if (mousePos.x < Screen.width / 3f && mousePos.y > Screen.height / 4f && mousePos.y < Screen.height * 3f / 4f) //Left
            {
                thirdStepComplete = false;
                fourthStepComplete = false;

                if (firstStepComplete)
                {
                    secondStepComplete = true;
                    firstStepComplete = false;

                    Debug.Log("Second step complete");
                }
            }
            else if (mousePos.y < Screen.height / 4f && mousePos.x > Screen.width / 3f && mousePos.x < Screen.width * 2f / 3f) //Bottom
            {
                firstStepComplete = false;
                fourthStepComplete = false;

                if (secondStepComplete)
                {
                    thirdStepComplete = true;
                    secondStepComplete = false;
                    Debug.Log("Third step complete");
                }
            }
            else if (mousePos.x > Screen.width * 2 / 3f && mousePos.y > Screen.height / 4f && mousePos.y < Screen.height * 3f / 4f) //Right
            {
                firstStepComplete = false;
                secondStepComplete = false;

                if (thirdStepComplete)
                {
                    fourthStepComplete = true;
                    thirdStepComplete = false;
                    Debug.Log("Fourth step complete");
                }
            }
        }
        else if (isSquareClockwiseMode)
        {
            if (mousePos.y > Screen.height * 4 / 5f && mousePos.x < Screen.width / 6f) //Top Left
            {
                firstStepComplete = true;
                secondStepComplete = false;
                thirdStepComplete = false;
                Debug.Log("First step complete");

                if (fourthStepComplete)
                {
                    hasCompletedCircle = true;
                    fourthStepComplete = false;
                    Debug.Log("Circle complete");
                }
            }
            else if (mousePos.y > Screen.height * 4 / 5f && mousePos.x > Screen.width * 5/ 6f) // Top Right
            {
                thirdStepComplete = false;
                fourthStepComplete = false;

                if (firstStepComplete)
                {
                    secondStepComplete = true;
                    firstStepComplete = false;

                    Debug.Log("Second step complete");
                }
            }
            else if (mousePos.y < Screen.height / 5f && mousePos.x > Screen.width * 5 / 6f) //Bottom Right
            {
                firstStepComplete = false;
                fourthStepComplete = false;

                if (secondStepComplete)
                {
                    thirdStepComplete = true;
                    secondStepComplete = false;
                    Debug.Log("Third step complete");
                }
            }
            else if (mousePos.y < Screen.height / 5f && mousePos.x < Screen.width / 6f) //Bottom Left
            {
                firstStepComplete = false;
                secondStepComplete = false;

                if (thirdStepComplete)
                {
                    fourthStepComplete = true;
                    thirdStepComplete = false;
                    Debug.Log("Fourth step complete");
                }
            }
        }
        else if (isSquareAntiClockwiseMode)
        {
            if (mousePos.y > Screen.height * 4 / 5f && mousePos.x < Screen.width / 6f) //Top Left
            {
                firstStepComplete = true;
                secondStepComplete = false;
                thirdStepComplete = false;
                Debug.Log("First step complete");

                if (fourthStepComplete)
                {
                    hasCompletedCircle = true;
                    fourthStepComplete = false;
                    Debug.Log("Circle complete");
                }
            }
            else if (mousePos.y < Screen.height / 5f && mousePos.x < Screen.width / 6f)  //Bottom Left
            {
                thirdStepComplete = false;
                fourthStepComplete = false;

                if (firstStepComplete)
                {
                    secondStepComplete = true;
                    firstStepComplete = false;

                    Debug.Log("Second step complete");
                }
            }
            else if (mousePos.y < Screen.height / 5f && mousePos.x > Screen.width * 5 / 6f) //Bottom Right
            {
                firstStepComplete = false;
                fourthStepComplete = false;

                if (secondStepComplete)
                {
                    thirdStepComplete = true;
                    secondStepComplete = false;
                    Debug.Log("Third step complete");
                }
            }
            else if (mousePos.y > Screen.height * 4 / 5f && mousePos.x > Screen.width * 5 / 6f) //Top Right
            {
                firstStepComplete = false;
                secondStepComplete = false;

                if (thirdStepComplete)
                {
                    fourthStepComplete = true;
                    thirdStepComplete = false;
                    Debug.Log("Fourth step complete");
                }
            }
        }

        if (hasCompletedCircle)
        {
            Vector3 newPos = selectedScrew.transform.position;
            newPos.z += screwPositionAmount;
            newPos.z = Mathf.Min(newPos.z, screwFinalPosition);
            selectedScrew.transform.position = newPos;

            audioSource.Play();

            if (newPos.z == screwFinalPosition)
            {
                Debug.Log("Screw complete");
                timer.isTimerRunning = false;
                Data.globalConsecutiveRound += 1;
                confettiParticle.gameObject.SetActive(true);
                StartCoroutine(CompletionDelay(2f));
            }
            else
            {
                if (isClockwiseMode || isSquareClockwiseMode)
                {
                    selectedScrew.transform.Rotate(0f, 0f, screwRotationAmount);
                }
                else if (isAntiClockwiseMode || isSquareAntiClockwiseMode)
                {
                    selectedScrew.transform.Rotate(0f, 0f, -screwRotationAmount);
                }
            }

            hasCompletedCircle = false;
        }
    }

    private IEnumerator CompletionDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        gameManager.LoadRandomGame();
    }
}
