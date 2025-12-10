using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    [Header("Default Settings")]
    public float defaultDuration = 0.25f;
    public float defaultMagnitude = 0.3f;

    private Vector3 initialLocalPos;
    private Coroutine shakeRoutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        initialLocalPos = transform.localPosition;
    }

    public void Shake(float duration = -1f, float magnitude = -1f)
    {
        if (duration <= 0f) duration = defaultDuration;
        if (magnitude <= 0f) magnitude = defaultMagnitude;

        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
        }

        shakeRoutine = StartCoroutine(DoShake(duration, magnitude));
    }

    IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            Vector2 offset2D = Random.insideUnitCircle * magnitude;
            Vector3 offset = new Vector3(offset2D.x, offset2D.y, 0f);

            transform.localPosition = initialLocalPos + offset;

            yield return null;
        }

        transform.localPosition = initialLocalPos;
        shakeRoutine = null;
    }
}
