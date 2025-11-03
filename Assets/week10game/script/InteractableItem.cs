using UnityEngine;
// 프리팹에 붙이기

public class InteractableItem : MonoBehaviour
{
    [Header("Item Info")]
    public string itemName = "Unnamed Item";

    bool isTarget = false;

    public string GetName() => itemName;

    public void SetAsTarget(bool value)
    {
        isTarget = value;
        // 디버그용으로 살짝 색을 바꿀 수도 있음
        // var r = GetComponentInChildren<Renderer>();
        // if (r != null && value) r.material.color = Color.yellow;
    }

    public bool IsTarget()
    {
        return isTarget;
    }
}