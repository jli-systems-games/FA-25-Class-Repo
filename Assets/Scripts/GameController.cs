using System;
using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
    public static event Action onGameChange;
    public static event Action<Game> onStartGame;


    private void Start()
    {
        onGameChange?.Invoke();

    }
}

public enum Game
{
    HorseRace,
    Hopscotch
}


// GameController.onGameChange += ResetStats;