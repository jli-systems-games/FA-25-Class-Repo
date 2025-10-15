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
        // 1) 저장값 읽기
        int index = PlayerPrefs.GetInt("car_index", 0);
        Color savedColor = new Color(
            PlayerPrefs.GetFloat("car_color_r", 1f),
            PlayerPrefs.GetFloat("car_color_g", 0f),
            PlayerPrefs.GetFloat("car_color_b", 0f),
            PlayerPrefs.GetFloat("car_color_a", 1f)
        );

        // 2) 스폰 위치/회전
        Vector3 pos = spawnPoint ? spawnPoint.position : Vector3.zero;
        Quaternion rot = spawnPoint ? spawnPoint.rotation : Quaternion.identity;

        // 3) 프리팹 선택 및 스폰 (SO 우선)
        GameObject carGO = null;
        CarSpec usedSpec = null;

        if (carsSO != null && carsSO.Length > 0 && carsSO[Mathf.Clamp(index, 0, carsSO.Length - 1)] != null)
        {
            index = Mathf.Clamp(index, 0, carsSO.Length - 1);
            usedSpec = carsSO[index];
            carGO = Instantiate(usedSpec.prefab, pos, rot);
            carGO.transform.Rotate(usedSpec.rotationOffset, Space.Self);
        }
        else
        {
            index = Mathf.Clamp(index, 0, carPrefabs.Length - 1);
            carGO = Instantiate(carPrefabs[index], pos, rot);
        }

        // 3.5) 부모로 붙여서 함께 움직이게 (컨트롤러는 부모에 붙어 있음)
        if (parentForSpawned)
        {
            carGO.transform.SetParent(parentForSpawned, worldPositionStays: false);
            carGO.transform.localPosition = Vector3.zero;
            carGO.transform.localRotation = Quaternion.identity;
            carGO.transform.localScale = Vector3.one;
        }

        // 4) 색 적용 (프리팹의 바디에 CarColor가 있어야 함)
        var carColor = carGO.GetComponentInChildren<CarColor>(true);
        if (carColor) carColor.ApplyColor(savedColor);

        // 5) ★ 부모 컨트롤러에 SO 주입 (여기가 핵심!)
        var controller = parentForSpawned ? parentForSpawned.GetComponent<ArcadeCarController>() : null;
        if (controller)
        {
            if (usedSpec != null)
            {
                controller.ApplySpec(usedSpec); // ← SO → 컨트롤러 연동
            }
            else
            {
                Debug.LogWarning("[RaceSpawnLoader] SO가 없어 프리팹만 스폰했습니다. 컨트롤러 값은 프리팹/인스펙터 기본값을 사용합니다.");
            }

            controller.enabled = true; // 혹시 꺼져 있었다면 켜주기
        }
        else
        {
            Debug.LogWarning("[RaceSpawnLoader] parentForSpawned에 ArcadeCarController가 없습니다.");
        }

        // 6) 카메라 타겟
        if (!cameraToFollow)
        {
            var cam = Camera.main;
            if (cam) cameraToFollow = cam.transform;
        }
        
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
