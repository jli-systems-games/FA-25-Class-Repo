using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChangerWithDelay : MonoBehaviour
{
    [SerializeField] private string sceneName = "YourSceneName"; 
    [SerializeField] private GameObject[] objectsToActivate;     
    [SerializeField] private float delayTime = 3f;               

    private bool isTriggered = false; 

    void Update()
    {
        if (!isTriggered && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            StartCoroutine(ActivateAndChangeScene());
            isTriggered = true;
        }
    }

    private IEnumerator ActivateAndChangeScene()
    {

        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
                obj.SetActive(true);
        }

    
        yield return new WaitForSeconds(delayTime);


        SceneManager.LoadScene(sceneName);
    }
}