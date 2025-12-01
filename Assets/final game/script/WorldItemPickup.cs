using UnityEngine;

public class WorldItemPickup : MonoBehaviour
{
    public ItemData itemData;   // Inspector에서 어떤 아이템인지 지정

    // 플레이어가 집을 때 호출할 함수
    public void Pickup()
    {
        if (itemData != null)
        {
            InventoryManager.Instance.AddItem(itemData);
            // 월드에서 사라지게
            Destroy(gameObject);
        }
    }
}
