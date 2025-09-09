using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScrewControl : MonoBehaviour
{
    public float screwPositionAmount = 0.03f;
    public float screwRotationAmount = 360f;
    public float screwFinalPosition = -1.351f;
    
    private bool firstStepComplete = false;
    private bool secondStepComplete = false;
    private bool thirdStepComplete = false;
    private bool fourthStepComplete = false;
    private bool hasCompletedCircle = false;

    public GameManager gameManager;
    public Timer timer;

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;

        if (mousePos.y > Screen.height * 3 / 4f && mousePos.x > Screen.width / 3f && mousePos.x < Screen.width * 2f / 3f)
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
        else if (mousePos.x > Screen.width * 2 / 3f && mousePos.y > Screen.height / 4f && mousePos.y < Screen.height * 3f / 4f)
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
        else if (mousePos.y < Screen.height / 4f && mousePos.x > Screen.width / 3f && mousePos.x < Screen.width * 2f / 3f)
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
        else if (mousePos.x < Screen.width / 3f && mousePos.y > Screen.height / 4f && mousePos.y < Screen.height * 3f / 4f)
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

        if (hasCompletedCircle)
        {
            Vector3 newPos = transform.position;
            newPos.z += screwPositionAmount;
            newPos.z = Mathf.Min(newPos.z, screwFinalPosition);
            transform.position = newPos;

            if (newPos.z == screwFinalPosition)
            {
                Debug.Log("Screw complete");
                timer.isTimerRunning = false;
                StartCoroutine(CompletionDelay(2f));
            }
            else
            {
                transform.Rotate(0f, 0f, screwRotationAmount);
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
