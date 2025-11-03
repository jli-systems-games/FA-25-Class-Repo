using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [Header("Base Room")]
    public GameObject roomShellPrefab; // 바닥+벽 들어있는 프리팹
    private GameObject currentRoom;

    [Header("Furniture")]
    public Transform furnitureRoot; // 생성된 가구가 들어갈 부모
    public GameObject[] furniturePrefabs;
    public Transform[] furnitureSpawnPoints;
    [Range(0, 1)] public float furnitureUseRate = 0.6f; // 자리 중 몇 %를 쓸지

    // 아이템을 올려둘 수 있는 위치들
    List<Transform> itemSpawnPoints = new List<Transform>();
    List<GameObject> generatedObjects = new List<GameObject>();

    public void GenerateRoom()
    {
        // 방 본체
        currentRoom = Instantiate(roomShellPrefab, Vector3.zero, Quaternion.identity);
        generatedObjects.Add(currentRoom);

        itemSpawnPoints.Clear();

        // 가구 자리 중 일부만 사용
        foreach (var sp in furnitureSpawnPoints)
        {
            if (Random.value <= furnitureUseRate)
            {
                GameObject furn = Instantiate(
                    furniturePrefabs[Random.Range(0, furniturePrefabs.Length)],
                    sp.position,
                    sp.rotation,
                    furnitureRoot
                );
                generatedObjects.Add(furn);

                // 가구 위에 놓을 수 있는 위치를 자식에서 찾아보기
                // (예: 가구 프리팹 안에 "ItemPoint"라는 빈 오브젝트를 넣어두면 됨)
                var points = furn.GetComponentsInChildren<ItemSpawnPoint>();
                foreach (var p in points)
                {
                    itemSpawnPoints.Add(p.transform);
                }
            }
        }

        // 만약 가구가 적어서 itemSpawnPoints가 하나도 없으면
        // 방 중앙이라도 하나 넣어둔다
        if (itemSpawnPoints.Count == 0)
        {
            GameObject dummy = new GameObject("CenterItemPoint");
            dummy.transform.position = Vector3.zero + Vector3.up * 1f;
            itemSpawnPoints.Add(dummy.transform);
        }
    }

    public void ClearRoom()
    {
        foreach (var go in generatedObjects)
        {
            if (go != null)
                Destroy(go);
        }
        generatedObjects.Clear();
        itemSpawnPoints.Clear();

        // furnitureRoot 밑에 혹시 남은 게 있으면 그것도
        foreach (Transform child in furnitureRoot)
        {
            Destroy(child.gameObject);
        }
    }

    public List<Transform> GetItemSpawnPoints()
    {
        return itemSpawnPoints;
    }
}
