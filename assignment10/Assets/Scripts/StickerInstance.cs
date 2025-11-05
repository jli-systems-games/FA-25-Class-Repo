using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StickerInstance : MonoBehaviour, IBeginDragHandler, IDragHandler, IScrollHandler, IPointerDownHandler
{
    public string stickerId;
    RectTransform rt;
    void Awake() { rt = GetComponent<RectTransform>(); }

    public void Init(Sprite sp, string id)
    {
        GetComponent<Image>().sprite = sp;
        stickerId = id;
    }

    public void OnBeginDrag(PointerEventData e) { transform.SetAsLastSibling(); }
    public void OnDrag(PointerEventData e) { rt.anchoredPosition += e.delta; }
    public void OnScroll(PointerEventData e)
    {
        float s = Mathf.Clamp(rt.localScale.x + e.scrollDelta.y * 0.05f, 0.5f, 2.5f);
        rt.localScale = Vector3.one * s;
    }
    public void OnPointerDown(PointerEventData e)
    {
        if (e.button == PointerEventData.InputButton.Right)
        {
            float angle = rt.localEulerAngles.z + 15f;
            rt.localEulerAngles = new Vector3(0, 0, angle);
        }
        else
        {
            transform.SetAsLastSibling();
        }
    }
}
