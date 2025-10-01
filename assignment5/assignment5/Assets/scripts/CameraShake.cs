using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    Vector3 originalPos;
    Coroutine shakeRoutine;

    void Awake()
    {
        originalPos = transform.localPosition;
    }

    public void Shake(float duration, float magnitude)
    {
        if (shakeRoutine != null) StopCoroutine(shakeRoutine);
        shakeRoutine = StartCoroutine(DoShake(duration, magnitude));
    }

    IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = originalPos + new Vector3(x, y, 0f);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        transform.localPosition = originalPos;
        shakeRoutine = null;
    }
}
