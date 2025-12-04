using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class HintVision : MonoBehaviour
{
    //Materials
    public Material mossyRockHintMaterial;
    public Material spriteHintMaterial;

    public Image circleImage;
    public Image eyeImage;
    public Image EIconImage;

    public Camera hintCamera;
    public Volume hintVisionVolume;
    public Volume hintCameraVolume;
    public Volume hintSymbolCameraVolume;
    public float hintVisionDuration = 2f;
    public float fadeDuration = 1f;
    public float coolDownTime = 5f;

    private bool isOnCooldown = false;
    private Coroutine currentCoroutine;

    private Color OGEmissionColor = new Color(0.0f, 0.749f, 0.749f, 0f);

    void Start()
    {
        //Reset
        hintCamera.enabled = false;
        hintVisionVolume.weight = 0;
        hintCameraVolume.weight = 0;
        hintSymbolCameraVolume.weight = 0;

        mossyRockHintMaterial.SetColor("_GlowEmission", Color.black);

        Color awakeColor = spriteHintMaterial.color;
        awakeColor.a = 0;
        spriteHintMaterial.color = awakeColor;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isOnCooldown && Data.hintVisionEnabled)
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
        yield return StartCoroutine(FadeAtOnce(1f,mossyRockHintMaterial, OGEmissionColor, 1f, Color.yellow, 1f));

        yield return new WaitForSeconds(hintVisionDuration);

        yield return StartCoroutine(FadeAtOnce(0f, mossyRockHintMaterial, Color.black, 0f, Color.white, 0.1f));
        hintCamera.enabled = false;

        yield return new WaitForSeconds(coolDownTime);
        isOnCooldown = false;

        StartCoroutine(VisionIconDisable(Color.white, 1f));
    }

    private IEnumerator FadeAtOnce(float weightTarget, Material material, Color emissionTarget, float alphaTarget, Color targetColor, float opacityTarget)
    {
        Coroutine c = StartCoroutine(HintVisionFade(weightTarget));
        Coroutine c1 = StartCoroutine(HintGlowEmissionColorFade(material, emissionTarget));
        Coroutine c2 = StartCoroutine(SpriteHintFade(alphaTarget));
        Coroutine c3 = StartCoroutine(VisionIconDisable(targetColor, opacityTarget));

        yield return c;
        yield return c1;
        yield return c2;
        yield return c3;
    }

    private IEnumerator VisionIconDisable(Color targetColor, float opacityTarget)
    {
        Color circCol = circleImage.color;
        Color eyeCol = eyeImage.color;
        Color eCol = EIconImage.color;

        circCol = targetColor;
        eyeCol = targetColor;
        eCol = targetColor;
        circCol.a = opacityTarget;
        eyeCol.a = opacityTarget;
        eCol.a = opacityTarget;

        circleImage.color = circCol;
        eyeImage.color = eyeCol;
        EIconImage.color = eCol;

        yield return null;
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
            hintSymbolCameraVolume.weight = Mathf.Lerp(startWeight, weightTarget, t / fadeDuration);
            yield return null;
        }

        hintVisionVolume.weight = weightTarget;
        hintCameraVolume.weight = weightTarget;
        hintSymbolCameraVolume.weight = weightTarget;
    }

    private IEnumerator HintGlowEmissionColorFade(Material material, Color emissionTarget)
    {
        Color startEmission = material.GetColor("_GlowEmission");
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            material.SetColor("_GlowEmission", Color.Lerp(startEmission, emissionTarget, t / fadeDuration));
            yield return null;
        }

        material.SetColor("_GlowEmission", emissionTarget);
    }

    private IEnumerator SpriteHintFade(float alphaTarget)
    {
        Color startColor = spriteHintMaterial.color;
        float startAlpha = startColor.a;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            startColor.a = Mathf.Lerp(startAlpha, alphaTarget, t / fadeDuration);
            spriteHintMaterial.color = startColor;
            yield return null;
        }

        startColor.a = alphaTarget;
        spriteHintMaterial.color = startColor;
    }
}