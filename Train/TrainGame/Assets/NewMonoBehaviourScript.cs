using UnityEngine;
using System.Collections;
using System;
public class TrainController : MonoBehaviour
{
    public static Action onTrainArrival;
    public GameObject firework1;
    public GameObject firework2;
    public GameObject firework3;
    private void Start()
    {
        firework1.SetActive(false);
        firework2.SetActive(false);
        firework3.SetActive(false);
        StartCoroutine(TrainTime(15f));
        StartCoroutine(fire2(10f));
        StartCoroutine(fire3(1f));
    }

    IEnumerator TrainTime(float waitTime)
    {
        Debug.Log("Wait");
        yield return new WaitForSeconds(waitTime);
        Debug.Log("Train Here");
        onTrainArrival?.Invoke();
        firework1.SetActive(true);
    }
    IEnumerator fire2(float waitTime)
    {
        Debug.Log("Wait");
        yield return new WaitForSeconds(waitTime);
        Debug.Log("Train Here");

        firework2.SetActive(true);
    }
    IEnumerator fire3(float waitTime)
    {
        Debug.Log("Wait");
        yield return new WaitForSeconds(waitTime);
        Debug.Log("Train Here");

        firework3.SetActive(true);
    }
}