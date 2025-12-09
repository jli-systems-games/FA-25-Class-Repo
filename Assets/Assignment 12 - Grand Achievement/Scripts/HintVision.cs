using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HintVision : MonoBehaviour
{
    //Materials
    public Material[] shaderEmissionChangeMaterials;
    public Material[] opacityChangeMaterials;
    public Material[] emissionChangeMaterials;

    public Image circleImage;
    public Image eyeImage;
    public Image EIconImage;

    public Camera hintCamera;
    public Camera hiddenHintCamera;
    public Camera hintSymbolCamera;
    public Volume hintVisionVolume;
    public Volume hintCameraVolume;
    public Volume hintSymbolCameraVolume;
    public float hintVisionDuration = 2f;
    public float fadeDuration = 1f;
    public float coolDownTime = 5f;

    public bool isOnCooldown = false;
    public Coroutine currentCoroutine;

    private bool wasInDisableZone = false;
    private Color OGEmissionColor = new Color(0.0f, 0.749f, 0.749f, 0f);

    void Start()
    {
        //Reset
        hintCamera.enabled = false;
        hiddenHintCamera.enabled = false;
        hintSymbolCamera.enabled = false;
        hintVisionVolume.weight = 0;
        hintCameraVolume.weight = 0;
        hintSymbolCameraVolume.weight = 0;


        foreach (Material mat in shaderEmissionChangeMaterials)
        {
            mat.SetColor("_GlowEmission", Color.black);
        }

        foreach (Material mat in opacityChangeMaterials)
        {
            Color awakeColor = mat.color;
            awakeColor.a = 0;
            mat.color = awakeColor;
        }

        foreach (Material mat in emissionChangeMaterials)
        {
            mat.SetColor("_EmissionColor", Color.black);
        }
    }

    void Update()
    {
        if (Data.inDisableZone && !wasInDisableZone)
        {
            wasInDisableZone = true;

            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);
            StopAllHintCoroutines();

            hintCamera.enabled = false;
            hiddenHintCamera.enabled = false;
            hintSymbolCamera.enabled = false;
            hintVisionVolume.weight = 0;
            hintCameraVolume.weight = 0;
            hintSymbolCameraVolume.weight = 0;

            StartCoroutine(FadeAtOnce(0f, Color.black, 0f, Color.red, 0.5f, 0));

            isOnCooldown = false;

            return;
        }

        if (!Data.inDisableZone && wasInDisableZone)
        {
            wasInDisableZone = false;

            StartCoroutine(VisionIconAppearance(Color.white, 0.1f));

            StartCoroutine(LeaveZoneCooldown());

            return;
        }

        if (Input.GetKeyDown(KeyCode.E) && !isOnCooldown && Data.hintVisionEnabled)
        {
            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);

            currentCoroutine = StartCoroutine(HintVisionSequence());
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Start Scene");
        }
    }

    private void StopAllHintCoroutines()
    {
        StopCoroutine(HintVisionSequence());
        StopCoroutine(FadeAtOnce(1f, OGEmissionColor, 1f, Color.yellow, 1f, 1));
        StopCoroutine(HintVisionFade(0f, 1));
        StopCoroutine(HintGlowEmissionColorFade(shaderEmissionChangeMaterials, "_GlowEmission", Color.black, 1));
        StopCoroutine(HintGlowEmissionColorFade(emissionChangeMaterials, "_EmissionColor", Color.black, 1));
        StopCoroutine(SpriteHintFade(0f, 1));
        StopCoroutine(VisionIconAppearance(Color.white, 1f));
    }

    private IEnumerator LeaveZoneCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(coolDownTime);
        isOnCooldown = false;

        StartCoroutine(VisionIconAppearance(Color.white, 1f));
    }

    private IEnumerator HintVisionSequence()
    {
        hintCamera.enabled = true;
        hiddenHintCamera.enabled = true;
        hintSymbolCamera.enabled = true;

        yield return StartCoroutine(FadeAtOnce(1f, OGEmissionColor, 1f, Color.yellow, 1f, 1));

        yield return new WaitForSeconds(hintVisionDuration);

        yield return StartCoroutine(FadeAtOnce(0f, Color.black, 0f, Color.white, 0.1f, 1));

        hintCamera.enabled = false;
        hiddenHintCamera.enabled = false;
        hintSymbolCamera.enabled = false;

        StartCoroutine(LeaveZoneCooldown());
    }

    public IEnumerator FadeAtOnce(float weightTarget, Color emissionTarget, float alphaTarget, Color targetColor, float opacityTarget, int isFadeInt)
    {
        Coroutine c = StartCoroutine(HintVisionFade(weightTarget, isFadeInt));
        Coroutine c1 = StartCoroutine(HintGlowEmissionColorFade(shaderEmissionChangeMaterials, "_GlowEmission", emissionTarget, isFadeInt));
        Coroutine c2 = StartCoroutine(HintGlowEmissionColorFade(emissionChangeMaterials, "_EmissionColor", emissionTarget * 30, isFadeInt));
        Coroutine c3 = StartCoroutine(SpriteHintFade(alphaTarget, isFadeInt));
        Coroutine c4 = StartCoroutine(VisionIconAppearance(targetColor, opacityTarget));

        yield return c;
        yield return c1;
        yield return c2;
        yield return c3;
        yield return c4;
    }

    public IEnumerator VisionIconAppearance(Color targetColor, float opacityTarget)
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

    public IEnumerator HintVisionFade(float weightTarget, int isFadeInt)
    {
        float startWeight = hintVisionVolume.weight;
        float t = 0f;

        if (isFadeInt == 1)
        {
            while (t < fadeDuration)
            {
                if (Data.inDisableZone)
                {
                    hintVisionVolume.weight = 0;
                    hintCameraVolume.weight = 0;
                    hintSymbolCameraVolume.weight = 0;
                    yield break;
                }

                t += Time.deltaTime;
                hintVisionVolume.weight = Mathf.Lerp(startWeight, weightTarget, t / fadeDuration);
                hintCameraVolume.weight = Mathf.Lerp(startWeight, weightTarget, t / fadeDuration);
                hintSymbolCameraVolume.weight = Mathf.Lerp(startWeight, weightTarget, t / fadeDuration);
                yield return null;
            }
        }
        else
        {
            hintVisionVolume.weight = weightTarget;
            hintCameraVolume.weight = weightTarget;
            hintSymbolCameraVolume.weight = weightTarget;
        }

        hintVisionVolume.weight = weightTarget;
        hintCameraVolume.weight = weightTarget;
        hintSymbolCameraVolume.weight = weightTarget;
    }

    public IEnumerator HintGlowEmissionColorFade(Material[] materialArray, string emissionVariableName, Color emissionTarget, int isFadeInt)
    {
        foreach (Material mat in materialArray)
        {
            Color startEmission = mat.GetColor(emissionVariableName);
            float t = 0f;

            if (isFadeInt == 1)
            {
                while (t < fadeDuration)
                {
                    t += Time.deltaTime;
                    mat.SetColor(emissionVariableName, Color.Lerp(startEmission, emissionTarget, t / fadeDuration));
                    yield return null;
                }
            }
            else
            {
                mat.SetColor(emissionVariableName, emissionTarget);
            }

                mat.SetColor(emissionVariableName, emissionTarget);
        }
    }

    public IEnumerator SpriteHintFade(float alphaTarget, int isFadeInt)
    {
        foreach (Material mat in opacityChangeMaterials)
        {
            Color startColor = mat.color;
            float startAlpha = startColor.a;
            float t = 0f;

            if (isFadeInt == 1)
            {
                while (t < fadeDuration)
                {
                    t += Time.deltaTime;
                    startColor.a = Mathf.Lerp(startAlpha, alphaTarget, t / fadeDuration);
                    mat.color = startColor;
                    yield return null;
                }
            }
            else
            {
                startColor.a = alphaTarget;
                mat.color = startColor;
            }

            startColor.a = alphaTarget;
            mat.color = startColor;
        }
    }
}