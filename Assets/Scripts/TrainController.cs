using UnityEngine;
using System.Collections;
using System;

public class TrainController : MonoBehaviour
{
    public static Action onTrainArrival;
    //public static Action<int> onTrainLeave;
    private void Start()
    {
        StartCoroutine(TrainTimer(2f));
    }

    IEnumerator TrainTimer(float waitTime)
    {
        Debug.Log("Wait");

        yield return new WaitForSeconds(waitTime);
        onTrainArrival?.Invoke();
        StartCoroutine(TrainTimer(2f));
    }
}
