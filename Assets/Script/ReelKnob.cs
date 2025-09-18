using UnityEngine;
using UnityEngine.EventSystems;

public class ReelKnobSimple : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Objects")]
    public RectTransform reelRoot;
    public RectTransform handleRoot;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip clockwiseSound;
    public AudioClip counterClockwiseSound;
    public float stepAngle = 10f;

    private bool dragging;
    private Vector2 screenCenter;
    private float startAngleOffset;
    private float lastStepIndex;

    void Reset()
    {
        handleRoot = GetComponent<RectTransform>();
        reelRoot = transform.parent as RectTransform;
    }

    void Start()
    {
        if (handleRoot == null) handleRoot = GetComponent<RectTransform>();
        if (reelRoot == null) reelRoot = handleRoot;
        UpdateScreenCenter();
        lastStepIndex = Mathf.Round(GetCurrentAngle() / stepAngle);
    }

    void UpdateScreenCenter()
    {
        RectTransform root = reelRoot != null ? reelRoot : handleRoot;
        screenCenter = RectTransformUtility.WorldToScreenPoint(null, root.position);
    }

    float GetMouseAngleDeg()
    {
        Vector2 mouse = Input.mousePosition;
        Vector2 dir = mouse - screenCenter;
        float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    return ang - 90f;
    }

    float GetCurrentAngle()
    {
        return handleRoot.eulerAngles.z;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
        UpdateScreenCenter();

        float current = GetCurrentAngle();
        float mouse = GetMouseAngleDeg();
        startAngleOffset = Mathf.DeltaAngle(mouse, current);
        lastStepIndex = Mathf.Round(current / stepAngle);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging) return;

        float mouse = GetMouseAngleDeg();
        float target = mouse + startAngleOffset;

        handleRoot.rotation = Quaternion.Euler(0, 0, target);

        
        float currentIndex = Mathf.Round(target / stepAngle);
        if (Mathf.Abs(currentIndex - lastStepIndex) >= 1f)
        {
            if (currentIndex > lastStepIndex)
                PlaySound(clockwiseSound);
            else
                PlaySound(counterClockwiseSound);

            lastStepIndex = currentIndex;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}