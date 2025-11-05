using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [System.Serializable]
    public class RoomVariant
    {
        [Header("Minimums")]
        public int minFurniturePerRound = 3;

        [Header("Maximums")]
        public int maxFurniturePerRound = 5;   // ★ 추가: 방별 최대 가구 수

        [Header("Room Shell (프리팹)")]
        public string variantName = "RoomA";
        public GameObject roomShellPrefab;

        [Header("Furniture Set (이 방 전용 가구 후보)")]
        public List<GameObject> furniturePrefabs;
        [Range(0f, 1f)] public float furnitureUseRate = 0.6f;
        public bool randomizeFurnitureRotation = true;
    }
    [Header("Variants (딱 2개만 사용)")]
    public RoomVariant variantA;
    public RoomVariant variantB;

    [Header("선택 모드")]
    public bool pickRandomVariant = true;   // true면 매 라운드 A/B 중 랜덤
    [Range(0, 1)] public int fixedVariantIndex = 0; // false일 때: 0=A, 1=B

    [Header("Parents")]
    public Transform roomRoot;      // 방 셸 부모
    public Transform furnitureRoot; // 가구 부모

    [Header("Fallback (옵션)")]
    // 방 프리팹에 FurniturePoint 마커가 없을 때만 사용
    public Transform[] furnitureSpawnPoints_Fallback;

    // 내부 상태
    GameObject spawnedShell;
    RoomVariant currentVariant;
    readonly List<GameObject> generatedObjects = new();
    readonly List<Transform> itemSpawnPoints = new();

    // ─────────────────────────────────────────────────────────────

    public void GenerateRoom()
    {
        int seed = Random.Range(int.MinValue, int.MaxValue);
        GenerateRoom(seed);
    }

    public void GenerateRoom(int seed)
    {
        ClearRoom();

        // 난수 초기화 먼저
        Random.InitState(seed);

        // 유효한 후보 채우기
        var candidates = new List<RoomVariant>();
        if (variantA != null && variantA.roomShellPrefab != null) candidates.Add(variantA);
        if (variantB != null && variantB.roomShellPrefab != null) candidates.Add(variantB);

        if (candidates.Count == 0)
        {
            Debug.LogError("[RoomGenerator] Variant A/B가 비었거나 Shell이 없습니다.");
            return;
        }

        // A/B 선택 (정확한 50:50 보장; 후보가 2개일 때)
        if (pickRandomVariant)
        {
            if (candidates.Count == 2)
            {
                currentVariant = (Random.value < 0.5f) ? candidates[0] : candidates[1];
            }
            else
            {
                // 후보가 1개뿐이면 그걸 사용
                currentVariant = candidates[0];
            }
        }
        else
        {
            currentVariant = candidates[Mathf.Clamp(fixedVariantIndex, 0, candidates.Count - 1)];
        }

        // 2) 방 셸 생성
        spawnedShell = Instantiate(currentVariant.roomShellPrefab, roomRoot != null ? roomRoot : transform);
        generatedObjects.Add(spawnedShell);

        // 3) 가구 스폰 포인트 수집(FurniturePoint 우선 → Fallback)
        var furniturePoints = CollectFurniturePoints(spawnedShell);
        if ((furniturePoints == null || furniturePoints.Count == 0) &&
            furnitureSpawnPoints_Fallback != null && furnitureSpawnPoints_Fallback.Length > 0)
        {
            furniturePoints = furnitureSpawnPoints_Fallback.ToList();
        }

        // 4) 선택된 방 전용 가구 세트로 랜덤 배치
        SpawnFurnitureForVariant(currentVariant, furniturePoints);

        // 5) 가구 속 ItemSpawnPoint 모으기
        RebuildItemSpawnPoints();
    }

    public void ClearRoom()
    {
        foreach (var go in generatedObjects)
            if (go) Destroy(go);
        generatedObjects.Clear();

        if (furnitureRoot != null)
        {
            var tmp = new List<GameObject>();
            foreach (Transform c in furnitureRoot) tmp.Add(c.gameObject);
            foreach (var g in tmp) Destroy(g);
        }

        itemSpawnPoints.Clear();
        spawnedShell = null;
        currentVariant = null;
    }

    public List<Transform> GetItemSpawnPoints() => itemSpawnPoints;

    // ─────────────────────────────────────────────────────────────
    // 내부 유틸

    List<Transform> CollectFurniturePoints(GameObject shell)
    {
        var points = new List<Transform>();
        if (!shell) return points;

        // 방법 1) 프리팹 내부 FurniturePoint 마커
        var markers = shell.GetComponentsInChildren<FurniturePoint>(true);
        if (markers != null && markers.Length > 0)
        {
            foreach (var m in markers) if (m) points.Add(m.transform);
        }
        else
        {
            // 방법 2) 이름으로 찾기(선택)
            var parent = shell.transform.Find("FurnitureSpawnPoints");
            if (parent != null)
            {
                foreach (Transform t in parent) points.Add(t);
            }
        }

        // 셔플
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
            Debug.LogWarning($"[RoomGenerator] '{variant.variantName}' 가구 리스트가 비었습니다.");
            return;
        }
        if (points == null || points.Count == 0)
        {
            Debug.LogWarning($"[RoomGenerator] '{variant.variantName}' 스폰 포인트가 없습니다.");
            return;
        }

        int baseCount = Mathf.CeilToInt(points.Count * Mathf.Clamp01(variant.furnitureUseRate));
        int useCount = Mathf.Clamp(
            Mathf.Max(variant.minFurniturePerRound, baseCount),
            0,
            Mathf.Min(points.Count, Mathf.Max(1, variant.maxFurniturePerRound)) // ★ 상한 적용
        );


        for (int i = 0; i < useCount; i++)
        {
            var p = points[i];
            var prefab = variant.furniturePrefabs[Random.Range(0, variant.furniturePrefabs.Count)];
            if (!prefab) continue;

            Quaternion rot = p.rotation;
            if (variant.randomizeFurnitureRotation)
                rot = Quaternion.Euler(0, Random.Range(0, 4) * 90f, 0); // 90° 스냅

            var parent = furnitureRoot != null ? furnitureRoot : (roomRoot != null ? roomRoot : transform);
            var go = Instantiate(prefab, p.position, rot, parent);
            generatedObjects.Add(go);
        }
    }

    void RebuildItemSpawnPoints()
    {
        itemSpawnPoints.Clear();

        Transform parent = furnitureRoot != null ? furnitureRoot : (roomRoot != null ? roomRoot : transform);
        var all = parent.GetComponentsInChildren<ItemSpawnPoint>(true);
        foreach (var s in all) if (s) itemSpawnPoints.Add(s.transform);

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
