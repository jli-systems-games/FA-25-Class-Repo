// Sticker.cs
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class Sticker : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Canvas canvas; // (비우면 부모 Canvas 자동 탐색)
    RectTransform rt;
    CanvasGroup cg;
    BoxCollider2D col2D;
    Rigidbody2D rb2D;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        if (canvas == null) canvas = GetComponentInParent<Canvas>();

        cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();

        // 물리 트리거 성립용 콜라이더/리짓바디 보장
        col2D = GetComponent<BoxCollider2D>();
        if (col2D == null) col2D = gameObject.AddComponent<BoxCollider2D>();

        rb2D = GetComponent<Rigidbody2D>();
        if (rb2D == null) rb2D = gameObject.AddComponent<Rigidbody2D>();
        rb2D.bodyType = RigidbodyType2D.Kinematic;
        rb2D.simulated = true;

        UpdateColliderSize();
    }

    void OnRectTransformDimensionsChange()
    {
        UpdateColliderSize();
    }

    void UpdateColliderSize()
    {
        if (col2D == null || rt == null) return;
        // World Space Canvas 기준: RectTransform의 사이즈를 월드 단위로 반영
        Vector2 size = rt.rect.size;
        // 스케일 반영
        Vector3 s = rt.lossyScale;
        col2D.size = new Vector2(size.x * Mathf.Abs(s.x), size.y * Mathf.Abs(s.y));
        col2D.offset = Vector2.zero;
    }

    public void OnBeginDrag(PointerEventData e)
    {
        cg.blocksRaycasts = false;     // 드래그 중 드롭/트리거가 UI 이벤트 받도록
        transform.SetAsLastSibling();  // Z-order 맨 앞으로
    }

    public void OnDrag(PointerEventData e)
    {
        if (canvas == null) return;
        Vector2 local;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform, e.position, canvas.worldCamera, out local);
        rt.anchoredPosition = local;   // 놓은 위치에 그대로 남음 (자유 배치)
        // 콜라이더는 RectTransform에 붙어 있어 자동으로 따라옴
    }

    public void OnEndDrag(PointerEventData e)
    {
        cg.blocksRaycasts = true;
    }
}
