using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ReputationUI : MonoBehaviour
{
    public static ReputationUI Instance;

    [Header("Root / Alpha Control")]
    public CanvasGroup canvasGroup;

    [Header("Fear Bar")]
    public Image fearFill;
    [Header("Respect Bar")]
    public Image respectFill;

    [Header("Value Settings")]
    public float maxFear = 40f;
    public float maxRespect = 40f;

    [Header("Alpha Settings")]
    public float idleAlpha = 0.15f;
    public float highlightAlpha = 0.9f;
    public float stayBrightTime = 1f;
    public float fadeDuration = 0.7f;

    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = idleAlpha;
        }
    }

    public void UpdateReputation(int fear, int respect)
    {
        fear = Mathf.Max(0, fear);
        respect = Mathf.Max(0, respect);

        if (fearFill != null)
        {
            float f = maxFear <= 0 ? 0 : Mathf.Clamp01(fear / maxFear);
            fearFill.fillAmount = f;
        }

        if (respectFill != null)
        {
            float r = maxRespect <= 0 ? 0 : Mathf.Clamp01(respect / maxRespect);
            respectFill.fillAmount = r;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = highlightAlpha;
        }

        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }
        fadeRoutine = StartCoroutine(FadeBackToIdle());
    }

    IEnumerator FadeBackToIdle()
    {
        yield return new WaitForSeconds(stayBrightTime);

        if (canvasGroup == null)
        {
            yield break;
        }

        float startAlpha = canvasGroup.alpha;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float lerp = t / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, idleAlpha, lerp);
            yield return null;
        }

        canvasGroup.alpha = idleAlpha;
        fadeRoutine = null;
    }
}
