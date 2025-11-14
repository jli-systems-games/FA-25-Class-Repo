using UnityEngine;
using TMPro;

public class AutoDisableAfterTime : MonoBehaviour
{
    public float disableDelay = 2f;
    public bool restartOnEnable = true;

    private float timer = 0f;
    private bool counting = false;

    void OnEnable()
    {
        if (restartOnEnable)
        {
            timer = 0f;
            counting = true;
        }
    }

    void Update()
    {
        if (!counting) return;

        timer += Time.deltaTime;

        if (timer >= disableDelay)
        {
            gameObject.SetActive(false);
        }
    }

    public void StartCountdown(float time)
    {
        disableDelay = time;
        timer = 0f;
        counting = true;
    }
}