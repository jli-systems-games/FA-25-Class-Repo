using UnityEngine;
using System.Collections.Generic;

public class CarCustomizerManager : MonoBehaviour
{
    [Header("교체할 차 프리팹들(각 프리팹엔 CarColor가 붙어 있어야 함)")]
    public List<GameObject> carPrefabs = new List<GameObject>();

    [Header("차 정보 UI 프리팹들(차 순서와 동일)")]
    public List<GameObject> uiCardPrefabs = new List<GameObject>();
    public Transform uiParent; // Canvas 아래 카드가 붙을 부모

    [Header("생성 위치/회전 기준")]
    public Transform spawnPoint;

    int currentIndex = 0;
    GameObject currentCar;
    GameObject currentCardUI;
    CarColor currentCarColor;

    // 현재 선택된 색
    Color savedColor = Color.red;

    void Start()
    {
        // ✅ 중앙 데이터에서 값 읽기 (과제 요구사항)
        //  - 앱 재실행까지 기억하고 싶으면 시작 전에 GameData.LoadFromPrefs() 호출 가능
        currentIndex = GameData.selectedCarIndex;
        savedColor = GameData.selectedColor;

        SpawnCar(currentIndex);
        ApplySavedColorToCurrent();
    }

    public void NextCar()
    {
        if (carPrefabs.Count == 0) return;
        currentIndex = (currentIndex + 1) % carPrefabs.Count;
        SpawnCar(currentIndex);
        ApplySavedColorToCurrent();

        // ✅ 중앙 데이터 갱신
        GameData.SetSelection(currentIndex, savedColor);
    }

    public void PrevCar()
    {
        if (carPrefabs.Count == 0) return;
        currentIndex = (currentIndex - 1 + carPrefabs.Count) % carPrefabs.Count;
        SpawnCar(currentIndex);
        ApplySavedColorToCurrent();

        // ✅ 중앙 데이터 갱신
        GameData.SetSelection(currentIndex, savedColor);
    }

    void SpawnCar(int index)
    {
        // 기존 차/카드 정리
        if (currentCar) Destroy(currentCar);
        if (currentCardUI) Destroy(currentCardUI);

        // 차 스폰
        Vector3 pos = spawnPoint ? spawnPoint.position : Vector3.zero;
        Quaternion rot = spawnPoint ? spawnPoint.rotation : Quaternion.identity;

        currentCar = Instantiate(carPrefabs[index], pos, rot);
        currentCarColor = currentCar.GetComponentInChildren<CarColor>(true);
        if (!currentCarColor)
            Debug.LogWarning("새 차 프리팹에 CarColor가 없습니다. Body Renderer 연결된 CarColor를 붙여주세요.");

        // UI 카드 스폰 (차 순서와 동일 인덱스)
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

    // 색상 버튼이 이 함수를 호출
    public void SetColor(Color c)
    {
        savedColor = c;
        if (currentCarColor) currentCarColor.ApplyColor(savedColor);

        // ✅ 중앙 데이터 갱신
        GameData.SetSelection(currentIndex, savedColor);
    }

<<<<<<< HEAD
    // (선택) PlayerPrefs를 계속 쓰고 싶다면 남겨도 되지만,
    // 과제 요구상 중앙 데이터 사용이 핵심이므로 아래 두 함수는 제거해도 무방합니다.
    void SaveIndex() { /* 사용 안 함: 중앙 데이터로 대체됨 */ }
    void SaveColor() { /* 사용 안 함: 중앙 데이터로 대체됨 */ }

    // Submit 버튼에서 호출
    public void SubmitAndGoRace(string raceSceneName)
    {
        // ✅ 중앙 데이터에 최종 확정
        GameData.SetSelection(currentIndex, savedColor);

        // (선택) 앱 재실행까지 유지하려면 한 줄 추가:
        // GameData.SaveToPrefs();

        // 다음 씬 로드
        SceneManager.LoadScene(raceSceneName);
=======
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
<<<<<<< HEAD
<<<<<<< HEAD
>>>>>>> parent of 5ec2ef95 (final racing input)
=======
>>>>>>> parent of 5ec2ef95 (final racing input)
=======
>>>>>>> parent of 5ec2ef95 (final racing input)
    }
}
