using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("动画设置")]
    [SerializeField] private float hoverScale = 1.1f; // hover时放大到1.1倍
    [SerializeField] private float hoverDuration = 0.3f;
    [SerializeField] private float returnDuration = 0.2f;

    private Vector3 originalScale;
    private Vector3 targetScale;

    private float currentTime;
    private bool isHovered;

    void Awake()
    {
        originalScale = transform.localScale;
        targetScale = originalScale * hoverScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        currentTime = 0f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        currentTime = 0f;
    }

    void Update()
    {
        if (isHovered)
        {
            currentTime += Time.deltaTime;
            float t = Mathf.Clamp01(currentTime / hoverDuration);
            t = 1f - (1f - t) * (1f - t); // EaseOutQuad
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
        }
        else
        {
            currentTime += Time.deltaTime;
            float t = Mathf.Clamp01(currentTime / returnDuration);
            t = t * t; // EaseInQuad
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, t);
        }
    }
}