using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<ItemData> items = new List<ItemData>();

    public System.Action OnInventoryChanged;   // UI에게 알리는 이벤트

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(ItemData item)
    {
        if (!items.Contains(item))
        {
            items.Add(item);
            OnInventoryChanged?.Invoke();
        }
    }

    public bool HasItem(string itemId)
    {
        return items.Exists(i => i.itemId == itemId);
    }
}
