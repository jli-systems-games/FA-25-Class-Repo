using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class HintVision : MonoBehaviour
{
    public Camera hintCamera;
    public Volume hintVisionVolume;
    public float hintVisionDuration = 2f;
    public float fadeDuration = 1f;
    public float coolDownTime = 5f;

    private bool isOnCooldown = false;
    private Coroutine currentCoroutine;

    void Start()
    {
        hintCamera.enabled = false;
        hintVisionVolume.weight = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isOnCooldown)
        {
            isOnCooldown = true;

            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);

            currentCoroutine = StartCoroutine(HintVisionSequence());
        }
    }

    private IEnumerator HintVisionSequence()
    {
        yield return StartCoroutine(HintVisionFade(1f));
        hintCamera.enabled = true;

        yield return new WaitForSeconds(hintVisionDuration);

        yield return StartCoroutine(HintVisionFade(0f));
        hintCamera.enabled = false;

        yield return new WaitForSeconds(coolDownTime);
        isOnCooldown = false;
    }

    private IEnumerator HintVisionFade(float weightTarget)
    {
        float startWeight = hintVisionVolume.weight;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            hintVisionVolume.weight = Mathf.Lerp(startWeight, weightTarget, t / fadeDuration);
            yield return null;
        }

        hintVisionVolume.weight = weightTarget;
    }
}