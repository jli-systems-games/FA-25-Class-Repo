using UnityEngine;
using UnityEngine.EventSystems;

public class UITo3DMapper : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("UI组件 (RectTransform)")]
    public RectTransform uiTarget;

    [Header("对应的3D物体")]
    public Transform object3D;

    [Header("UI的移动范围")]
    public float uiMinX = -126.4f;
    public float uiMaxX = -25f;
    public float uiMinY = -269f;
    public float uiMaxY = 30.7f;

    [Header("3D物体的移动范围")]
    public float objMinX = -15.1f;
    public float objMaxX = -5f;
    public float objMinZ = -37.6f;
    public float objMaxZ = 0f;

    [Header("Y轴保持固定")]
    public float objY = 0f;

    private Canvas parentCanvas;
    private Vector2 offset;

    void Start()
    {
        if (!uiTarget)
            uiTarget = GetComponent<RectTransform>();

        parentCanvas = uiTarget.GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            eventData.position,
            parentCanvas.worldCamera,
            out Vector2 localPoint);

        offset = uiTarget.anchoredPosition - localPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            eventData.position,
            parentCanvas.worldCamera,
            out Vector2 localPoint))
        {
            Vector2 targetPos = localPoint + offset;

            targetPos.x = Mathf.Clamp(targetPos.x, uiMinX, uiMaxX);
            targetPos.y = Mathf.Clamp(targetPos.y, uiMinY, uiMaxY);

            uiTarget.anchoredPosition = targetPos;

            Update3DPosition();
        }
    }

    public void OnEndDrag(PointerEventData eventData) { }

    void Update3DPosition()
    {
        if (!object3D) return;

        Vector2 uiPos = uiTarget.anchoredPosition;


        float tX = Mathf.InverseLerp(uiMinX, uiMaxX, uiPos.x);
        float tY = Mathf.InverseLerp(uiMinY, uiMaxY, uiPos.y);

        float objX = Mathf.Lerp(objMinX, objMaxX, tX);
        float objZ = Mathf.Lerp(objMinZ, objMaxZ, tY);

        object3D.position = new Vector3(objX, objY, objZ);
    }
}
