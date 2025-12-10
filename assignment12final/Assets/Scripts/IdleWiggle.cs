using UnityEngine;

public class IdleWiggle : MonoBehaviour
{
    [Header("左右晃动幅度（世界坐标）")]
    public float wiggleAmplitude = 0.03f;

    [Header("晃动频率（每秒摆几次）")]
    public float wiggleFrequency = 2f;

    private Vector3 defaultLocalPos;
    private float offsetSeed;

    private void Awake()
    {
        defaultLocalPos = transform.localPosition;
        offsetSeed = Random.Range(0f, 100f);
    }

    private void Update()
    {
        float t = Time.time + offsetSeed;

        float offsetX = Mathf.Sin(t * wiggleFrequency) * wiggleAmplitude;

        transform.localPosition = defaultLocalPos + new Vector3(offsetX, 0f, 0f);
    }
}
