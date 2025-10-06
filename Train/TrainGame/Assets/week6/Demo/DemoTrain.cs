using UnityEngine;
using System.Collections;
using System;

public class DemoTrain : MonoBehaviour
{
    public static Action onTrainArrival;
    public static Action<int>onTrainLeave;

    public void Start()
    {
  
        StartCoroutine(TrainTimer(2f));
    }
    IEnumerator TrainTimer(float waitTime)
    {
        Debug.Log("Wait");
        yield return new WaitForSeconds(2f);
     
        onTrainArrival?.Invoke();
        StartCoroutine("trainHere");
    }
}