using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CarCustomizerManager : MonoBehaviour
{
    [Header("교체할 차 프리팹들(각 프리팹엔 CarColor가 붙어 있어야 함)")]
    public List<GameObject> carPrefabs = new List<GameObject>();

    [Header("차 정보 UI 프리팹들(차 순서와 동일)")]
    public List<GameObject> uiCardPrefabs = new List<GameObject>();  // ★ 추가
    public Transform uiParent;                                        // ★ 추가 (Canvas/CarCardRoot 등)

    [Header("생성 위치/회전 기준")]
    public Transform spawnPoint;

    int currentIndex = 0;
    GameObject currentCar;
    GameObject currentCardUI;     // ★ 추가
    CarColor currentCarColor;

    // 현재 선택된 색 (버튼이 이 값을 바꾸고, 매니저가 현 차에 적용)
    Color savedColor = Color.red;

    void Start()
    {
        // ★ CENTRALDATA: 우선 중앙 데이터가 비어있지 않다면 그 값을 사용
        CentralData.LoadFromPrefsIfAny(); // (선택) 이전 세션 로드
        currentIndex = CentralData.Selected.index;
        savedColor = CentralData.Selected.color;

        // ↓ 기존 PlayerPrefs 기반 로드는 "백업" 개념으로 유지하고 싶다면 아래 블럭을 남겨도 되지만,
        //   과제 요구상 '단일 진실은 중앙 데이터'가 되도록 우선순위는 위 CentralData 값을 씁니다.
        /*
        currentIndex = PlayerPrefs.GetInt("car_index", currentIndex);
        if (PlayerPrefs.HasKey("car_color_r"))
        {
            float r = PlayerPrefs.GetFloat("car_color_r", savedColor.r);
            float g = PlayerPrefs.GetFloat("car_color_g", savedColor.g);
            float b = PlayerPrefs.GetFloat("car_color_b", savedColor.b);
            float a = PlayerPrefs.GetFloat("car_color_a", savedColor.a);
            savedColor = new Color(r, g, b, a);
        }
        */

        SpawnCar(currentIndex);
        ApplySavedColorToCurrent();

        // (선택) 중앙 데이터 변경 시 UI 미러링이 필요하면 구독
        // CentralData.OnChanged += OnCentralDataChanged;
    }

    void OnDestroy()
    {
        // if (CentralData.OnChanged != null) CentralData.OnChanged -= OnCentralDataChanged;
    }

    // (선택)
    // void OnCentralDataChanged() { /* UI 갱신 등 */ }

    public void NextCar()
    {
        if (carPrefabs.Count == 0) return;
        currentIndex = (currentIndex + 1) % carPrefabs.Count;
        SpawnCar(currentIndex);
        ApplySavedColorToCurrent();

        // ★ CENTRALDATA: 선택 변경을 중앙에 반영
        CentralData.SetSelected(currentIndex, savedColor);

        // (선택) 세션 보존 원하면 저장
        // CentralData.SaveToPrefs();
    }

    public void PrevCar()
    {
        if (carPrefabs.Count == 0) return;
        currentIndex = (currentIndex - 1 + carPrefabs.Count) % carPrefabs.Count;
        SpawnCar(currentIndex);
        ApplySavedColorToCurrent();

        // ★ CENTRALDATA
        CentralData.SetSelected(currentIndex, savedColor);
        // CentralData.SaveToPrefs();
    }

    void SpawnCar(int index)
    {
        // 기존 차/카드 정리
        if (currentCar) Destroy(currentCar);
        if (currentCardUI) Destroy(currentCardUI);   // ★ 추가

        // 차 스폰
        Vector3 pos = spawnPoint ? spawnPoint.position : Vector3.zero;
        Quaternion rot = spawnPoint ? spawnPoint.rotation : Quaternion.identity;

        currentCar = Instantiate(carPrefabs[index], pos, rot);
        currentCarColor = currentCar.GetComponentInChildren<CarColor>(true);
        if (!currentCarColor)
            Debug.LogWarning("새 차 프리팹에 CarColor가 없습니다. Body Renderer 연결된 CarColor를 붙여주세요.");

        // UI 카드 스폰
        if (uiParent && index < uiCardPrefabs.Count && uiCardPrefabs[index] != null)
        {
            currentCardUI = Instantiate(uiCardPrefabs[index], uiParent);
            currentCardUI.SetActive(true);
        }
        else
        {
            if (!uiParent) Debug.LogWarning("[UI] uiParent가 비었습니다. Canvas 아래 빈 오브젝트를 연결하세요.");
            if (index >= uiCardPrefabs.Count) Debug.LogWarning("[UI] uiCardPrefabs 개수가 carPrefabs보다 적습니다.");
            if (index < uiCardPrefabs.Count && uiCardPrefabs[index] == null) Debug.LogWarning("[UI] 해당 인덱스의 UI 프리팹이 null입니다.");
        }
    }

    void ApplySavedColorToCurrent()
    {
        if (currentCarColor) currentCarColor.ApplyColor(savedColor);
    }

    public void SetColor(Color c)  // 색상 버튼이 이 함수를 호출
    {
        savedColor = c;
        if (currentCarColor) currentCarColor.ApplyColor(savedColor);

        // ★ CENTRALDATA
        CentralData.SetSelected(currentIndex, savedColor);
        // CentralData.SaveToPrefs();
    }

    // ▼ 기존 PlayerPrefs 전용 저장 함수는 이제 선택(옵션)
    void SaveIndex()
    {
        PlayerPrefs.SetInt("car_index", currentIndex);
        PlayerPrefs.Save();
    }

    void SaveColor()
    {
        PlayerPrefs.SetFloat("car_color_r", savedColor.r);
        PlayerPrefs.SetFloat("car_color_g", savedColor.g);
        PlayerPrefs.SetFloat("car_color_b", savedColor.b);
        PlayerPrefs.SetFloat("car_color_a", savedColor.a);
        PlayerPrefs.Save();
    }

    public void SubmitAndGoRace(string raceSceneName)
    {
        // ★ CENTRALDATA: 씬 이동 직전에 중앙 데이터가 최신 상태인지 한 번 더 보장
        CentralData.SetSelected(currentIndex, savedColor);

        // (선택) 세션 보존이 필요하면 저장
        CentralData.SaveToPrefs();

        // 다음 씬 로드
        SceneManager.LoadScene(raceSceneName);
    }
}
