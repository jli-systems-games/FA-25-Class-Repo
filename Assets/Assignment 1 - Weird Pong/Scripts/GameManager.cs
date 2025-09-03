using System;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public int startingScore;
    public static int gameScore; //this variable only exists once in this script

    public static Weather globalWeather = Weather.Clear;

    public UnityEvent onWorldGenerated;

    public static event Action onNewDays;
    public static event Action<Weather> OnWeatherChange;

    private void Start()
    {
        gameScore = startingScore;

        //Do stuff
        onWorldGenerated?.Invoke(); //? means try to do this and don't tell me if it doesn't fail
    }

    public enum Weather
    {
        Clear,
        Cloudy,
        Sunny,
        Rainy
    }
}
