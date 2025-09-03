using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    //Managers
    public int startingScore;
    
    public static int gameScore = 0;
    public static Weather globalWeather = Weather.Clear;

    public UnityEvent onWorldGenerated;
    //event_whatever

    public static event Action onNewDay;//actions have fairly clean and beginner friendly syntax 
    public static event Action<Weather> onWeatherChange; //and also have the benefit of letting you add required arguements to them


    private void Start()
    {
        gameScore = startingScore;

        //do stufff
        onWorldGenerated?.Invoke();
        onNewDay?.Invoke();
    }

    
    //GameManager.gameScore
}

public enum Weather
{
    Clear,
    Cloudy,
    Rain,
    Snow
}
