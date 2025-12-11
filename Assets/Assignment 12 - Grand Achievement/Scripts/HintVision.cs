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

    private bool wasInDisableZone = false;
    private Color OGEmissionColor = Color.white;

    private bool hintAvailable = true;

    //Coroutines
    public Coroutine currentCR;
    private Coroutine fadeAtOnceCR;
    private Coroutine hintVisionCR;
    private Coroutine hintFadeCR;
    private Coroutine shaderGlowCR;
    private Coroutine emissionGlowCR;
    private Coroutine spriteFadeCR;
    private Coroutine iconCR;
    private Coroutine cooldownCR;

    void Start()
    {
        ResetAll();

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
        UpdateHintIcon();

        if (Data.inDisableZone && !wasInDisableZone)
        {
            wasInDisableZone = true;

            if (currentCR != null)
                StopCoroutine(currentCR);

            StopAllHintCoroutines();

            ResetAll();

            hintAvailable = false;
            isOnCooldown = false;

            return;
        }

        if (!Data.inDisableZone && wasInDisableZone)
        {
            wasInDisableZone = false;

            cooldownCR = StartCoroutine(LeaveZoneCooldown());

            return;
        }

        if (Input.GetKeyDown(KeyCode.E) && !isOnCooldown && Data.hintVisionEnabled && !Data.inDisableZone && hintAvailable)
        {
            if (currentCR != null)
                StopCoroutine(currentCR);

            currentCR = StartCoroutine(HintVisionSequence());
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene("Start Scene");
        }
    }

    private void StopAllHintCoroutines()
    {
        if (currentCR != null) StopCoroutine(currentCR);
        if (hintVisionCR != null) StopCoroutine(hintVisionCR);
        if (fadeAtOnceCR != null) StopCoroutine(fadeAtOnceCR);
        if (hintFadeCR != null) StopCoroutine(hintFadeCR);
        if (shaderGlowCR != null) StopCoroutine(shaderGlowCR);
        if (emissionGlowCR != null) StopCoroutine(emissionGlowCR);
        if (spriteFadeCR != null) StopCoroutine(spriteFadeCR);
        if (iconCR != null) StopCoroutine(iconCR);
        if (cooldownCR != null) StopCoroutine(cooldownCR);
    }

    private void ResetAll()
    {
        hintCamera.enabled = false;
        hiddenHintCamera.enabled = false;
        hintSymbolCamera.enabled = false;
        hintVisionVolume.weight = 0;
        hintCameraVolume.weight = 0;
        hintSymbolCameraVolume.weight = 0;
    }

    private IEnumerator LeaveZoneCooldown()
    {
        hintAvailable = false;
        isOnCooldown = true;

        yield return new WaitForSeconds(coolDownTime);
        isOnCooldown = false;
        hintAvailable = true;

        iconCR = StartCoroutine(VisionIconAppearance(Color.white, 1f));
    }

    private IEnumerator HintVisionSequence()
    {
        hintCamera.enabled = true;
        hiddenHintCamera.enabled = true;
        hintSymbolCamera.enabled = true;

        yield return fadeAtOnceCR = StartCoroutine(FadeAtOnce(1f, OGEmissionColor, 1f, Color.yellow, 1f, 1));

        yield return new WaitForSeconds(hintVisionDuration);

        yield return fadeAtOnceCR = StartCoroutine(FadeAtOnce(0f, Color.black, 0f, Color.white, 0.1f, 1));

        hintCamera.enabled = false;
        hiddenHintCamera.enabled = false;
        hintSymbolCamera.enabled = false;

        cooldownCR = StartCoroutine(LeaveZoneCooldown());
    }

    public IEnumerator FadeAtOnce(float weightTarget, Color emissionTarget, float alphaTarget, Color targetColor, float opacityTarget, int isFadeInt)
    {
        hintVisionCR = StartCoroutine(HintVisionFade(weightTarget, isFadeInt));
        shaderGlowCR = StartCoroutine(HintGlowEmissionColorFade(shaderEmissionChangeMaterials, "_GlowEmission", emissionTarget, isFadeInt));
        emissionGlowCR = StartCoroutine(HintGlowEmissionColorFade(emissionChangeMaterials, "_EmissionColor", emissionTarget * 30, isFadeInt));
        hintFadeCR = StartCoroutine(SpriteHintFade(alphaTarget, isFadeInt));
        iconCR = StartCoroutine(VisionIconAppearance(targetColor, opacityTarget));

        yield return hintVisionCR;
        yield return shaderGlowCR;
        yield return emissionGlowCR;
        yield return hintFadeCR;
        yield return iconCR;
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

    private void UpdateHintIcon()
    {
        if (Data.inDisableZone)
        {
            circleImage.color = new Color(1f, 0f, 0f, 0.5f);
            eyeImage.color = new Color(1f, 0f, 0f, 0.5f);
            EIconImage.color = new Color(1f, 0f, 0f, 0.5f);
        }
        else if (isOnCooldown || !hintAvailable)
        {
            circleImage.color = new Color(1f, 1f, 1f, 0.1f);
            eyeImage.color = new Color(1f, 1f, 1f, 0.1f);
            EIconImage.color = new Color(1f, 1f, 1f, 0.1f);
        }
        else
        {
            circleImage.color = new Color(1f, 1f, 1f, 1f);
            eyeImage.color = new Color(1f, 1f, 1f, 1f);
            EIconImage.color = new Color(1f, 1f, 1f, 1f);
        }
    }
}