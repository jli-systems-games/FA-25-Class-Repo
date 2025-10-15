using UnityEngine;
#if UNITY_EDITOR
using UnityEditor; // 에디터 선택/포커스
#endif

public class RaceSpawnLoader : MonoBehaviour
{
    [Header("스폰 위치(필수)")]
    public Transform spawnPoint;

    [Header("차 프리팹 배열 (커스터마이즈 씬과 동일 순서)")]
    public GameObject[] carPrefabs;   // SO 없을 때 사용

    [Header("SO 배열 (있으면 SO 우선 사용)")]
    public CarSpec[] carsSO;

    [Header("따라다니는 카메라(비우면 MainCamera 탐색)")]
    public Transform cameraToFollow;  // FollowCam 등

    [Header("스폰 후: 부모/포커스")]
    public Transform parentForSpawned; // ★ 부모(컨트롤러가 붙는 Anchor)
    public bool focusInEditor = true;

    void Start()
    {
        // === 1) 선택값 읽기 ===
        // ★ CENTRALDATA: 중앙 데이터에서 직접 읽기 (과제 요건 충족: 상속 없는 중앙 클래스)
        //    필요하면 직전 세션 저장분을 불러오도록 활성화 가능
        // CentralData.LoadFromPrefsIfAny(); // ← 세션 보존이 필요할 때만 사용

        int index = CentralData.Selected.index;   // ★ CENTRALDATA
        Color savedColor = CentralData.Selected.color; // ★ CENTRALDATA

        // (선택) PlayerPrefs 백업 경로를 남기고 싶다면, 중앙 데이터 값이 초기값일 때만 보조적으로 읽는 로직을 둘 수 있음
        // if (index == 0 && Mathf.Approximately(savedColor.a, 0f)) { /* PlayerPrefs 보조 로드 */ }

        // === 2) 스폰 위치/회전 ===
        Vector3 pos = spawnPoint ? spawnPoint.position : Vector3.zero;
        Quaternion rot = spawnPoint ? spawnPoint.rotation : Quaternion.identity;

        // === 3) 프리팹 선택 및 스폰 (SO 우선) ===
        GameObject carGO = null;
        CarSpec usedSpec = null;

        // SO가 있고 인덱스가 범위 내면 SO 우선
        if (carsSO != null && carsSO.Length > 0)
        {
            index = Mathf.Clamp(index, 0, carsSO.Length - 1);
            usedSpec = carsSO[index];
            if (usedSpec != null && usedSpec.prefab != null)
            {
                carGO = Instantiate(usedSpec.prefab, pos, rot);
                carGO.transform.Rotate(usedSpec.rotationOffset, Space.Self);
            }
            else
            {
                Debug.LogWarning("[RaceSpawnLoader] CarSpec 또는 prefab이 비어 있습니다. carPrefabs로 대체 시도.");
            }
        }

        // SO가 없거나 실패했으면 carPrefabs 사용
        if (carGO == null)
        {
            if (carPrefabs == null || carPrefabs.Length == 0)
            {
                Debug.LogError("[RaceSpawnLoader] 스폰할 프리팹이 없습니다.");
                return;
            }
            index = Mathf.Clamp(index, 0, carPrefabs.Length - 1);
            if (carPrefabs[index] == null)
            {
                Debug.LogError($"[RaceSpawnLoader] carPrefabs[{index}] 가 null입니다.");
                return;
            }
            carGO = Instantiate(carPrefabs[index], pos, rot);
        }

        // === 3.5) 부모로 붙여서 함께 움직이게 (컨트롤러는 부모에 붙어 있음) ===
        if (parentForSpawned)
        {
            carGO.transform.SetParent(parentForSpawned, worldPositionStays: false);
            carGO.transform.localPosition = Vector3.zero;
            carGO.transform.localRotation = Quaternion.identity;
            carGO.transform.localScale = Vector3.one;
        }

        // === 4) 색 적용 (프리팹의 바디에 CarColor가 있어야 함) ===
        var carColor = carGO.GetComponentInChildren<CarColor>(true);
        if (carColor) carColor.ApplyColor(savedColor);
        else
        {
            // CarColor가 없으면 Renderer 직접 칠하기(보급형)
            var rends = carGO.GetComponentsInChildren<Renderer>(true);
            foreach (var r in rends) r.material.color = savedColor;
        }

        // === 5) ★ 부모 컨트롤러에 SO 주입 (있을 경우) ===
        var controller = parentForSpawned ? parentForSpawned.GetComponent<ArcadeCarController>() : null;
        if (controller)
        {
            if (usedSpec != null)
            {
                controller.ApplySpec(usedSpec); // ← SO → 컨트롤러 연동
            }
            else
            {
                Debug.LogWarning("[RaceSpawnLoader] SO가 없어 프리팹만 스폰했습니다. 컨트롤러 값은 프리팹/인스펙터 기본값 사용.");
            }

            controller.enabled = true; // 혹시 꺼져 있었다면 켜주기
        }
        else
        {
            Debug.LogWarning("[RaceSpawnLoader] parentForSpawned에 ArcadeCarController가 없습니다.");
        }

        // === 6) 카메라 타겟 ===
        if (!cameraToFollow)
        {
            var cam = Camera.main;
            if (cam) cameraToFollow = cam.transform;
        }

        // (필요 시 여기서 카메라 팔로우 스크립트에 carGO/parentForSpawned를 타겟으로 지정)

#if UNITY_EDITOR
        if (focusInEditor && carGO)
        {
            Selection.activeGameObject = carGO;
            EditorGUIUtility.PingObject(carGO);
            if (SceneView.lastActiveSceneView != null)
            {
                var sv = SceneView.lastActiveSceneView;
                var b = new Bounds(carGO.transform.position, Vector3.one * 6f);
                sv.Frame(b, instant: false);
            }
        }
#endif
    }
}
