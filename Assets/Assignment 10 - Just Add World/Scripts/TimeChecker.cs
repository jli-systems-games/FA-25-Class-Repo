using UnityEngine;

public class TimeChecker : MonoBehaviour
{
    private float startTime;

    void Start()
    {
        Data.currentTime = 0;
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        Data.currentTime = Time.time - startTime;

        if (Data.currentTime > Data.highScore)
        {
            Data.highScore = Data.currentTime;
        }
    }
}
