using UnityEngine;

public class FakeWalkBob : MonoBehaviour
{
    [Header("bobAmplitude")]
    public float bobAmplitude = 0.05f;

    [Header("bobFrequency")]
    public float bobFrequency = 8f;

    [Header("moveThreshold")]
    public float moveThreshold = 0.05f;

    private Rigidbody2D rb;
    private Vector3 defaultLocalPos;
    private float bobTimer = 0f;

    private void Awake()
    {
        defaultLocalPos = transform.localPosition;

        rb = GetComponentInParent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogWarning("FakeWalkBob: Rigidbody2D not found");
        }
    }

    private void Update()
    {
        bool isMoving = false;

        if (rb != null)
        {
            isMoving = rb.linearVelocity.sqrMagnitude > (moveThreshold * moveThreshold);
        }

        if (isMoving)
        {
            bobTimer += Time.deltaTime * bobFrequency;

            float offsetX = Mathf.Sin(bobTimer) * bobAmplitude;

            float offsetY = 0f;

            transform.localPosition = defaultLocalPos + new Vector3(offsetX, offsetY, 0f);
        }
        else
        {
            bobTimer = 0f;
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                defaultLocalPos,
                Time.deltaTime * 10f
            );
        }
    }
}
