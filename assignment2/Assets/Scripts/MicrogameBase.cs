using UnityEngine;
using System;
using System.Collections;

public abstract class MicrogameBase : MonoBehaviour
{
    public event Action<bool> OnFinished;
    protected float timeLimit;
    protected bool running;

    public bool IsRunning => running; // <= add this

    public virtual void Begin(float timeLimit)
    {
        this.timeLimit = timeLimit;
        running = true;
        StartCoroutine(Countdown());
        OnBegin();
    }

    protected abstract void OnBegin();
    protected abstract bool CheckAutoComplete();

    IEnumerator Countdown()
    {
        float t = timeLimit;
        while (running && t > 0f)
        {
            t -= Time.deltaTime;
            OnTick?.Invoke(Mathf.Max(0f, t));
            if (CheckAutoComplete())
            {
                Finish(true);
                yield break;
            }
            yield return null;
        }
        if (running) Finish(false);
    }

    public event Action<float> OnTick;

    protected void Finish(bool success)
    {
        if (!running) return;
        running = false;
        OnFinished?.Invoke(success);
    }
}
