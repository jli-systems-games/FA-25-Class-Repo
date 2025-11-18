using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class HintVision : MonoBehaviour
{
    public Camera hintCamera;
    public Volume hintVisionVolume;
    public Volume hintCameraVolume;
    public float hintVisionDuration = 2f;
    public float fadeDuration = 1f;
    public float coolDownTime = 5f;

    private bool isOnCooldown = false;
    private Coroutine currentCoroutine;

    private Color OGEmissionColor = new Color(0.0f, 0.749f, 0.749f, 0f);

    //Materials
    public Material mossyRockHintMaterial;

    void Start()
    {
        hintCamera.enabled = false;
        hintVisionVolume.weight = 0;
        hintCameraVolume.weight = 0;
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
        hintCamera.enabled = true;
        yield return StartCoroutine(HintVisionFade(1f));
        yield return StartCoroutine(HintGlowEmissionColorFade(mossyRockHintMaterial, OGEmissionColor));

        yield return new WaitForSeconds(hintVisionDuration);

        yield return StartCoroutine(HintGlowEmissionColorFade(mossyRockHintMaterial, Color.black));
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
            hintCameraVolume.weight = Mathf.Lerp(startWeight, weightTarget, t / fadeDuration);
            yield return null;
        }

        hintVisionVolume.weight = weightTarget;
        hintCameraVolume.weight = weightTarget;
    }

    private IEnumerator HintGlowEmissionColorFade(Material material, Color emissionColor)
    {
        Color startEmission = material.GetColor("_GlowEmission");
        Color emissionTarget = emissionColor;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            material.SetColor("_GlowEmission", Color.Lerp(startEmission, emissionTarget, t / fadeDuration));
            yield return null;
        }

        material.SetColor("_GlowEmission", emissionTarget);
    }
}