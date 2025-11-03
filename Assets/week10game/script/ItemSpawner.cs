using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] clutterPrefabs;     // 그냥 채우는 물건
    public GameObject[] targetablePrefabs;  // 목표가 될 수 있는 물건

    [Header("Counts")]
    public int minClutterPerPoint = 0;
    public int maxClutterPerPoint = 2;

    List<InteractableItem> spawnedItems = new List<InteractableItem>();

    public List<InteractableItem> SpawnItems(List<Transform> spawnPoints)
    {
        ClearItems(); // 혹시 남아있을까봐

        foreach (var point in spawnPoints)
        {
            // 1) 우선 아무 상관없는 물건들 몇 개
            int clutterCount = Random.Range(minClutterPerPoint, maxClutterPerPoint + 1);
            for (int i = 0; i < clutterCount; i++)
            {
                var clutter = InstantiateRandom(clutterPrefabs, point.position);
                var itm = clutter.GetComponent<InteractableItem>();
                if (itm != null) spawnedItems.Add(itm);
            }

            // 2) 일정 확률로 "목표 후보"가 될 수 있는 물건도 하나 올려둠
            if (Random.value < 0.7f) // 70% 정도로
            {
                var targetable = InstantiateRandom(targetablePrefabs, point.position + new Vector3(0, 0.2f, 0));
                var itm = targetable.GetComponent<InteractableItem>();
                if (itm != null) spawnedItems.Add(itm);
            }
        }

        return spawnedItems;
    }

    GameObject InstantiateRandom(GameObject[] prefabs, Vector3 pos)
    {
        if (prefabs == null || prefabs.Length == 0) return null;
        int idx = Random.Range(0, prefabs.Length);
        return Instantiate(prefabs[idx], pos, Quaternion.identity);
    }

    public List<InteractableItem> ChooseTargets(List<InteractableItem> itemPool, int count)
    {
        List<InteractableItem> result = new List<InteractableItem>();

        // 목표가 될 수 있는 아이템만 골라서 뽑아도 되고,
        // 여기선 그냥 전체 중에서 뽑도록 할게요
        List<InteractableItem> temp = new List<InteractableItem>(itemPool);

        for (int i = 0; i < count; i++)
        {
            if (temp.Count == 0) break;
            int idx = Random.Range(0, temp.Count);
            result.Add(temp[idx]);
            temp.RemoveAt(idx);

            // 여기서 "너는 이번 타깃이야" 플래그를 켜줄 수도 있음
            result[^1].SetAsTarget(true);
        }

        return result;
    }

    public void ClearItems()
    {
        foreach (var item in spawnedItems)
        {
            if (item != null)
                Destroy(item.gameObject);
        }
        spawnedItems.Clear();
    }
}
