using UnityEngine;

[ExecuteAlways]
public class UnderwaterFloat : MonoBehaviour
{
    [Header("位移漂浮")]
    public float amplitude = 0.15f;   
    public float frequency = 0.35f;  
    public Vector3 axis = new Vector3(0f, 1f, 0f);

    [Header("旋转摆动")]
    public Vector3 rotDegrees = new Vector3(3f, 0.5f, 3f);
    public float rotFrequency = 0.25f;
    [Header("细小噪声（随机抖动/水流扰动）")]
    public float noiseAmplitude = 0.03f;
    public float noiseSpeed = 0.8f; 
    public Vector3 noiseScale = new Vector3(0.6f, 0.6f, 0.6f); 

    [Header("相位随机")]
    public bool randomizeOnStart = true;

    Vector3 basePos;
    Quaternion baseRot;
    float phasePos, phaseRot;

    void Awake()
    {
        basePos = transform.localPosition;
        baseRot = transform.localRotation;

        if (randomizeOnStart)
        {
            var h = Mathf.Abs((transform.position.x * 73856093
                             + transform.position.y * 19349663
                             + transform.position.z * 83492791) % 997);
            phasePos = h * 0.013f;
            phaseRot = h * 0.017f;
        }
    }

    void Update()
    {
        float t = Time.time;

        Vector3 floatOffset = axis.normalized * (amplitude * Mathf.Sin((t + phasePos) * Mathf.PI * 2f * frequency));

        float nx = (Mathf.PerlinNoise((t * noiseSpeed) + phasePos, transform.position.x * noiseScale.x) - 0.5f);
        float ny = (Mathf.PerlinNoise((t * noiseSpeed) + phasePos, transform.position.y * noiseScale.y) - 0.5f);
        float nz = (Mathf.PerlinNoise((t * noiseSpeed) + phasePos, transform.position.z * noiseScale.z) - 0.5f);
        Vector3 noiseOffset = new Vector3(nx, ny, nz) * noiseAmplitude;

        transform.localPosition = basePos + floatOffset + noiseOffset;

        float rx = rotDegrees.x * Mathf.Sin((t + phaseRot) * Mathf.PI * 2f * rotFrequency);
        float ry = rotDegrees.y * Mathf.Sin((t + phaseRot * 1.2f) * Mathf.PI * 2f * rotFrequency * 0.9f);
        float rz = rotDegrees.z * Mathf.Sin((t + phaseRot * 0.8f) * Mathf.PI * 2f * rotFrequency * 1.1f);

        transform.localRotation = baseRot * Quaternion.Euler(rx, ry, rz);
    }
}
