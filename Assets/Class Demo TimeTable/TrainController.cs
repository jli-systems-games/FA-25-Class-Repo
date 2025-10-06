using System;
using System.Collections;
using UnityEngine;

public class TrainController : MonoBehaviour
{
    public static Action onTrainArrival;
    public static Action<int> onTrainLeave;

    private void Start()
    {
        StartCoroutine(TrainTimer(6f));
    }

    private IEnumerator TrainTimer(float delay)
    {
        Debug.Log("Train coming.");
        yield return new WaitForSeconds(delay);
        Debug.Log("Train here!");
        onTrainArrival?.Invoke();
    }
}
