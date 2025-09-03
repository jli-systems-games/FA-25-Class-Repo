using DG.Tweening.Core.Easing;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    void Start()
    {
        GameManager.gameScore = 5;
        Debug.Log(GameManager.gameScore);

        //GameManager.onNewDay += ResetStats; //you can also subscribe multiple functions to the same event
        GameManager.onNewDay += ChangeOutfit;

        GameManager.onWeatherChange += ResetStats;
    }

    void ResetStats(Weather newWeather)
    {
        Debug.Log("Reset stats");

        if(newWeather == Weather.Clear)
        {
            //do something
        }
    }

    void ChangeOutfit()
    {

    }

}
