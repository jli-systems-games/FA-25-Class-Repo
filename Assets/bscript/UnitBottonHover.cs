using UnityEngine;
using UnityEngine.EventSystems;

public class UnitButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnitStats unitStats; // assign in inspector
    public UnitInfoUI uiInfo;   // drag your UI manager here

    public void OnPointerEnter(PointerEventData eventData)
    {
        uiInfo.ShowInfo(unitStats);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        uiInfo.HideInfo();
    }
}
