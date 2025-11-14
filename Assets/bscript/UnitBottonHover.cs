using UnityEngine;
using UnityEngine.EventSystems;

public class UnitButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnitStats unitStats; 
    public UnitInfoUI uiInfo;   

    public void OnPointerEnter(PointerEventData eventData)
    {
        uiInfo.ShowInfo(unitStats);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        uiInfo.HideInfo();
    }
}
