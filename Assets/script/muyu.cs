using UnityEngine;
using System.Collections;

public class ClickShowOnce : MonoBehaviour
{
    public GameObject objectToShow;  
    public AudioSource clickAudio;   
    public float showDuration = 1f;  

    private Coroutine showRoutine;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            
            if (clickAudio != null)
                clickAudio.Play();

            
            if (showRoutine != null)
                StopCoroutine(showRoutine);

            showRoutine = StartCoroutine(ShowObjectOnce());
        }
    }

    IEnumerator ShowObjectOnce()
    {
        objectToShow.SetActive(true);
        yield return new WaitForSeconds(showDuration);
        objectToShow.SetActive(false);
    }
}

