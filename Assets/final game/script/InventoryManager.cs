using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<ItemData> items = new List<ItemData>();

    public System.Action OnInventoryChanged;

    public HashSet<string> acquiredTools = new HashSet<string>();

    public bool hasCuttingTool = false;

    public bool HasTool(string toolName)
    {
        return acquiredTools.Contains(toolName);
    }

    public void AcquireTool(string toolName)
    {
        if(!acquiredTools.Contains(toolName))
        {
            acquiredTools.Add(toolName);
        }
    }

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
