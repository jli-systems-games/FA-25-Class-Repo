using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionToLight : MonoBehaviour
{
    private Light lightObject;
    [SerializeField] private Material lightMaterial;
    [SerializeField] private float minIntensity = 0.1f;
    [SerializeField] private float maxIntensity = 1f;
    [SerializeField] private float flickerSpeed = 10f;
    [SerializeField] private float flickerDuration = 0.2f;

    private string flickerPropertyName = "_Flicker";
    private string minPropertyName = "_MinIntensity";
    private string maxPropertyName = "_MaxIntensity";
    private int flickerID, minID, maxID;

    private void Awake()
    {
        flickerID = Shader.PropertyToID(flickerPropertyName);
        minID = Shader.PropertyToID(minPropertyName);
        maxID = Shader.PropertyToID(maxPropertyName);
    }

    private void Start()
    {
        lightObject = GetComponent<Light>();
        StartCoroutine(FlickerLight());

        lightMaterial.SetFloat(minID, minIntensity);
        lightMaterial.SetFloat(maxID, maxIntensity);
    }

    private IEnumerator FlickerLight()
    {
        while (true)
        {
            float targetIntensity = UnityEngine.Random.Range(minIntensity, maxIntensity);
            float elapsedTime = 0f;
            while (elapsedTime < flickerDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.PingPong(elapsedTime * flickerSpeed, 1f);
                lightObject.intensity = Mathf.Lerp(lightObject.intensity, targetIntensity, t);
                lightMaterial.SetFloat(flickerID, lightObject.intensity);
                yield return null;
            }
            yield return null;
        }
    }
}

