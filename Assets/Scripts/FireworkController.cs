using UnityEngine;
using System;
using System.Collections;

public class FireworkController : MonoBehaviour
{
    public static Action onFireHearts;
    public static Action onFireSpinner;
    public static Action onFireBurst;
    int fireworkspot = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        PlayFireworkSequence();
    }

    public void PlayFireworkSequence()
    {
        switch (fireworkspot)
        {
            case 0:
                StartCoroutine(PlayFirework(Firework.Heart, .5f));
                break;
            case 10:
                fireworkspot = 0;
                break;
            default:
                fireworkspot = 0;
                break;
        }
    }

    IEnumerator PlayFirework(Firework firework, float wait)
    {
        yield return new WaitForSeconds(wait);

        switch (firework)
        {
            case Firework.Heart:
                onFireHearts?.Invoke();
                break;
            case Firework.Spinner:
                onFireSpinner?.Invoke();
                break;
            case Firework.Burst:
                onFireBurst?.Invoke();
                break;
        }

        fireworkspot++;
        PlayFireworkSequence();
    }
}

public enum Firework
{
    Heart,
    Spinner,
    Burst
}
