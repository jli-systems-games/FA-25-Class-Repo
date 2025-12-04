using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HintVision : MonoBehaviour
{
    //Materials
    //public Material mossyRockHintMaterial;
    //public Material spriteHintMaterial;
    //public Material hiddenBlueGlowMaterial;

    public Material[] shaderEmissionChangeMaterials;
    public Material[] opacityChangeMaterials;
    public Material[] emissionChangeMaterials;

    public Image circleImage;
    public Image eyeImage;
    public Image EIconImage;

    public Camera hintCamera;
    public Camera hiddenHintCamera;
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
        hiddenHintCamera.enabled = false;
        hintVisionVolume.weight = 0;
        hintCameraVolume.weight = 0;
        hintSymbolCameraVolume.weight = 0;


        foreach (Material mat in shaderEmissionChangeMaterials)
        {
            mat.SetColor("_GlowEmission", Color.black);
        }

        //mossyRockHintMaterial.SetColor("_GlowEmission", Color.black);
        //hiddenBlueGlowMaterial.SetColor("_GlowEmission", Color.black);

        foreach (Material mat in opacityChangeMaterials)
        {
            Color awakeColor = mat.color;
            awakeColor.a = 0;
            mat.color = awakeColor;
        }

        //Color awakeColor = spriteHintMaterial.color;
        //awakeColor.a = 0;
        //spriteHintMaterial.color = awakeColor;

        foreach (Material mat in emissionChangeMaterials)
        {
            mat.SetColor("_EmissionColor", Color.black);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isOnCooldown && Data.hintVisionEnabled && !Data.inDisableZone)
        {
            isOnCooldown = true;

            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);

            currentCoroutine = StartCoroutine(HintVisionSequence());
        }
        else if (!Data.inDisableZone)
        {
            //StartCoroutine(VisionIconDisable(Color.white, 1f));
        }
        else if (Data.inDisableZone)
        {
            StartCoroutine(FadeAtOnce(0f, Color.black, 0f, Color.white, 0.1f));
            StartCoroutine(VisionIconDisable(Color.white, 0.1f));

            isOnCooldown = false;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Start Scene");
        }
    }

    private IEnumerator HintVisionSequence()
    {
        hintCamera.enabled = true;
        hiddenHintCamera.enabled = true;
        yield return StartCoroutine(FadeAtOnce(1f, OGEmissionColor, 1f, Color.yellow, 1f));

        yield return new WaitForSeconds(hintVisionDuration);

        yield return StartCoroutine(FadeAtOnce(0f, Color.black, 0f, Color.white, 0.1f));

        hintCamera.enabled = false;
        hiddenHintCamera.enabled = false;

        yield return new WaitForSeconds(coolDownTime);
        isOnCooldown = false;

        StartCoroutine(VisionIconDisable(Color.white, 1f));
    }

    private IEnumerator FadeAtOnce(float weightTarget, Color emissionTarget, float alphaTarget, Color targetColor, float opacityTarget)
    {
        Coroutine c = StartCoroutine(HintVisionFade(weightTarget));
        Coroutine c1 = StartCoroutine(HintGlowEmissionColorFade(shaderEmissionChangeMaterials, "_GlowEmission", emissionTarget));
        Coroutine c2 = StartCoroutine(HintGlowEmissionColorFade(emissionChangeMaterials, "_EmissionColor", emissionTarget * 30));
        Coroutine c3 = StartCoroutine(SpriteHintFade(alphaTarget));
        Coroutine c4 = StartCoroutine(VisionIconDisable(targetColor, opacityTarget));

        yield return c;
        yield return c1;
        yield return c2;
        yield return c3;
        yield return c4;
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

    private IEnumerator HintGlowEmissionColorFade(Material[] materialArray, string emissionVariableName, Color emissionTarget)
    {
        foreach (Material mat in materialArray)
        {
            Color startEmission = mat.GetColor(emissionVariableName);
            float t = 0f;

            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                mat.SetColor(emissionVariableName, Color.Lerp(startEmission, emissionTarget, t / fadeDuration));
                yield return null;
            }

            mat.SetColor(emissionVariableName, emissionTarget);
        }
    }

    private IEnumerator SpriteHintFade(float alphaTarget)
    {
        foreach (Material mat in opacityChangeMaterials)
        {
            Color startColor = mat.color;
            float startAlpha = startColor.a;
            float t = 0f;

            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                startColor.a = Mathf.Lerp(startAlpha, alphaTarget, t / fadeDuration);
                mat.color = startColor;
                yield return null;
            }

            startColor.a = alphaTarget;
            mat.color = startColor;
        }
    }
}