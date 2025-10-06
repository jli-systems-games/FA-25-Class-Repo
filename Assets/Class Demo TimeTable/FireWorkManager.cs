using System;
using System.Collections;
using UnityEngine;

public class FireWorkManager : MonoBehaviour
{
    public static Action startHeart;
    public static Action startBig;
    public static Action startRed;
    public static Action startSpiral;
    public static Action end;

    void Start()
    {
        StartCoroutine(DelayBeforeRed(1f));
    }

    IEnumerator DelayBeforeRed(float delay)
    {
        yield return new WaitForSeconds(delay);
        startRed?.Invoke();

        StartCoroutine(DelayBeforeSpiral(5f));
        end?.Invoke();
    }

    IEnumerator DelayBeforeSpiral(float delay)
    {
        yield return new WaitForSeconds(delay);
        startSpiral?.Invoke();

        StartCoroutine(DelayBeforeBig(5f));
        end?.Invoke();
    }

    IEnumerator DelayBeforeBig(float delay)
    {
        yield return new WaitForSeconds(delay);
        startBig?.Invoke();

        StartCoroutine(DelayBeforeHeart(5f));
        end?.Invoke();
    }

    IEnumerator DelayBeforeHeart(float delay)
    {
        yield return new WaitForSeconds(delay);
        startHeart?.Invoke();

        StartCoroutine(DelayBeforeEnd(5f));
        end?.Invoke();
    }

    IEnumerator DelayBeforeEnd(float delay)
    {
        yield return new WaitForSeconds(delay);
        end?.Invoke();
    }
}
