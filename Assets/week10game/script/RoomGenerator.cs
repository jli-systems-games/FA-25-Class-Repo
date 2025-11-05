using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [System.Serializable]
    public class RoomVariant
    {
        [Header("Room Shell (A/B/C 등)")]
        public string variantName = "RoomA";
        public GameObject roomShellPrefab;          // 방 A/B/C 프리팹

        [Header("Furniture Set (이 방에서만 쓰는 가구 후보)")]
        public List<GameObject> furniturePrefabs;   // 이 방 전용 가구들
        [Range(0f, 1f)] public float furnitureUseRate = 0.6f; // 스폰 포인트 중 몇 % 사용
        public bool randomizeFurnitureRotation = true;         // 90도 단위 회전 랜덤
    }

    [Header("Variants (방 프리셋 3개 등)")]
    public List<RoomVariant> variants = new();      // A/B/C를 여기에 등록
    public bool pickRandomVariant = true;
    public int fixedVariantIndex = 0;               // 디버그용: 특정 방 고정

    [Header("Parents")]
    public Transform roomRoot;                      // 방 셸 부모
    public Transform furnitureRoot;                 // 생성된 가구 부모

    [Header("Fallback (구버전 호환)")]
    // 만약 방 프리팹에 FurniturePoint를 안 심었다면, 이 배열을 쓰도록 남겨둠
    public Transform[] furnitureSpawnPoints_Fallback; // 씬에 깔아둔 포인트들(선택)

    // 내부 상태
    GameObject spawnedShell;
    RoomVariant currentVariant;
    readonly List<GameObject> generatedObjects = new(); // 생성물 추적(정리용)
    readonly List<Transform> itemSpawnPoints = new();   // 가구들 속 ItemSpawnPoint 수집

    // ─────────────────────────────────────────────────────────────────────────────
    /// 기존 호출과 호환: 파라미터 없는 GenerateRoom()
    public void GenerateRoom()
    {
        int seed = Random.Range(int.MinValue, int.MaxValue);
        GenerateRoom(seed);
    }

    /// 방/가구 생성 (시드 고정 가능)
    public void GenerateRoom(int seed)
    {
        ClearRoom();

        if (variants == null || variants.Count == 0)
        {
            Debug.LogError("[RoomGenerator] No variants set.");
            return;
        }

        Random.InitState(seed);

        // 1) 방 프리셋 선택
        int idx = pickRandomVariant ? Random.Range(0, variants.Count)
                                    : Mathf.Clamp(fixedVariantIndex, 0, variants.Count - 1);
        currentVariant = variants[idx];

        // 2) 방 셸 생성
        if (currentVariant.roomShellPrefab == null)
        {
            Debug.LogError("[RoomGenerator] Variant has no roomShellPrefab.");
            return;
        }

        spawnedShell = Instantiate(currentVariant.roomShellPrefab, roomRoot != null ? roomRoot : transform);
        generatedObjects.Add(spawnedShell);

        // 3) 가구 스폰 포인트 수집 (우선순위: FurniturePoint → Fallback 배열)
        var furniturePoints = CollectFurniturePoints(spawnedShell);
        if ((furniturePoints == null || furniturePoints.Count == 0) && furnitureSpawnPoints_Fallback != null && furnitureSpawnPoints_Fallback.Length > 0)
        {
            furniturePoints = furnitureSpawnPoints_Fallback.ToList();
        }

        // 4) 이 방 전용 가구 세트에서 랜덤 배치
        SpawnFurnitureForVariant(currentVariant, furniturePoints);

        // 5) 가구 속 ItemSpawnPoint 모으기 (아이템 스폰용)
        RebuildItemSpawnPoints();
    }

    // ─────────────────────────────────────────────────────────────────────────────
    public void ClearRoom()
    {
        // 생성했던 것들 정리
        foreach (var go in generatedObjects)
            if (go) Destroy(go);
        generatedObjects.Clear();

        // furnitureRoot 아래 잔여물도 정리
        if (furnitureRoot != null)
        {
            var toDelete = new List<GameObject>();
            foreach (Transform c in furnitureRoot) toDelete.Add(c.gameObject);
            foreach (var g in toDelete) Destroy(g);
        }

        itemSpawnPoints.Clear();
        spawnedShell = null;
        currentVariant = null;
    }

    public List<Transform> GetItemSpawnPoints()
    {
        return itemSpawnPoints;
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // 내부 유틸

    List<Transform> CollectFurniturePoints(GameObject shell)
    {
        var points = new List<Transform>();
        if (shell == null) return points;

        // 방법 1) 방 프리팹 안에 박아둔 FurniturePoint 마커들
        var markers = shell.GetComponentsInChildren<FurniturePoint>(true);
        if (markers != null && markers.Length > 0)
        {
            foreach (var m in markers) if (m) points.Add(m.transform);
        }
        else
        {
            // 방법 2) 이름으로 찾기 (옵션)
            var parent = shell.transform.Find("FurnitureSpawnPoints");
            if (parent != null)
            {
                foreach (Transform t in parent) points.Add(t);
            }
        }

        // 랜덤 사용용 셔플
        for (int i = 0; i < points.Count; i++)
        {
            int j = Random.Range(i, points.Count);
            (points[i], points[j]) = (points[j], points[i]);
        }
        return points;
    }

    void SpawnFurnitureForVariant(RoomVariant variant, List<Transform> points)
    {
        if (variant == null)
        {
            Debug.LogWarning("[RoomGenerator] Variant is null.");
            return;
        }
        if (variant.furniturePrefabs == null || variant.furniturePrefabs.Count == 0)
        {
            Debug.LogWarning($"[RoomGenerator] Variant '{variant.variantName}' has no furniturePrefabs.");
            return;
        }
        if (points == null || points.Count == 0)
        {
            Debug.LogWarning($"[RoomGenerator] No furniture points in variant '{variant.variantName}'.");
            return;
        }

        int useCount = Mathf.RoundToInt(points.Count * Mathf.Clamp01(variant.furnitureUseRate));
        useCount = Mathf.Clamp(useCount, 0, points.Count);

        for (int i = 0; i < useCount; i++)
        {
            var p = points[i];
            var prefab = variant.furniturePrefabs[Random.Range(0, variant.furniturePrefabs.Count)];
            if (prefab == null) continue;

            Quaternion rot = p.rotation;
            if (variant.randomizeFurnitureRotation)
            {
                // 90도 스냅
                rot = Quaternion.Euler(0, Random.Range(0, 4) * 90f, 0);
            }

            var parent = furnitureRoot != null ? furnitureRoot : (roomRoot != null ? roomRoot : transform);
            var go = Instantiate(prefab, p.position, rot, parent);
            generatedObjects.Add(go);
        }
    }

    void RebuildItemSpawnPoints()
    {
        itemSpawnPoints.Clear();

        Transform parent = furnitureRoot != null ? furnitureRoot : (roomRoot != null ? roomRoot : transform);

        // 가구들 속 ItemSpawnPoint를 모두 수집
        var all = parent.GetComponentsInChildren<ItemSpawnPoint>(true);
        foreach (var s in all)
            if (s) itemSpawnPoints.Add(s.transform);

        // 만약 하나도 없다면, 방 중앙에 임시 포인트 1개
        if (itemSpawnPoints.Count == 0)
        {
            var dummy = new GameObject("CenterItemPoint");
            dummy.transform.SetPositionAndRotation(Vector3.up * 1f, Quaternion.identity);
            dummy.transform.SetParent(spawnedShell != null ? spawnedShell.transform : transform);
            itemSpawnPoints.Add(dummy.transform);
            generatedObjects.Add(dummy);
        }
    }
}
