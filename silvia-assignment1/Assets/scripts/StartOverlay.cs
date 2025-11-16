using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class StartOverlayAuto : MonoBehaviour
{
    [Header("References")]
    public Ball ball; 
    public CanvasGroup canvasGroup; 

    [Header("Behavior")]
    public float showSeconds = 3f; 
    public bool allowSkipAnyKey = true;  
    public bool fade = true; 
    public float fadeDuration = 0.35f; 
    bool running;

    void Reset()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Awake()
    {
        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    void Start()
    {
        StartCoroutine(RunSequence());
    }

    IEnumerator RunSequence()
    {
        running = true;

        if (fade) yield return FadeTo(1f, fadeDuration);
        else canvasGroup.alpha = 1f;

        float t = 0f;
        while (t < showSeconds)
        {
            if (allowSkipAnyKey && Input.anyKeyDown) break;
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        if (fade) yield return FadeTo(0f, fadeDuration);
        else canvasGroup.alpha = 0f;

        gameObject.SetActive(false);

        if (ball) ball.LaunchRandom();

        running = false;
    }

    IEnumerator FadeTo(float target, float duration)
    {
        float start = canvasGroup.alpha;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, t / duration);
            yield return null;
        }
        canvasGroup.alpha = target;
    }

    public void ShowThenHide(float secondsOverride = -1f)
    {
        if (secondsOverride > 0f) showSeconds = secondsOverride;
        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        StopAllCoroutines();
        StartCoroutine(RunSequence());
    }
}
