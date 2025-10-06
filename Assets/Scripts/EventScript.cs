using UnityEngine;
using UnityEngine.Events;
using System;

public class EventScript : MonoBehaviour
{
    public UnityEvent onPlayerStep;
    public static Action onPlayerDie;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventScript.onPlayerDie += PlayerDeath;

        onPlayerStep?.Invoke();
        onPlayerDie?.Invoke();
    }

    public void PlayerDeath()
    {

    }
}
