using System;
using UnityEngine;

public class TrueValue : MonoBehaviour
{
    public DateTime trueDate;
    public TimeSpan trueDepartureTime;
    public TimeSpan trueArrivalTime;

    public string trueFromLocation;
    public string trueToLocation;

    public int year;
    public int month;
    public int day;
    public int hour;
    public int minute;
    public int cartNumber;
    public int arrivalHour;

    void Start()
    {
        trueFromLocation = "Paris";
        trueToLocation = "London";

        cartNumber = UnityEngine.Random.Range(1, 10);

        //Random date
        year = UnityEngine.Random.Range(1980, 2051);

        month = UnityEngine.Random.Range(2, 12);

        day= UnityEngine.Random.Range(2, 28);

        trueDate = new DateTime(year, month, day);

        //Random time
        hour = UnityEngine.Random.Range(6, 20);

        arrivalHour = hour + 3;

        minute = UnityEngine.Random.Range(0, 4) * 15;

        trueDepartureTime = new TimeSpan(hour, minute, 0);

        trueArrivalTime = new TimeSpan(hour + 3, minute, 0);
    }
}
