using UnityEngine;

public class TrueValue : MonoBehaviour
{
    public string trueFromLocation;
    public string trueToLocation;

    public int year;
    public int month;
    public int day;
    public int hour;
    public int arrivalHour;

    void Awake()
    {
        trueFromLocation = "Paris";
        trueToLocation = "London";

        //Random date
        year = Random.Range(1980, 2051);

        month = Random.Range(2, 12);

        day= Random.Range(2, 28);

        //Random time
        hour = Random.Range(6, 20);

        arrivalHour = hour + 3;
    }
}
