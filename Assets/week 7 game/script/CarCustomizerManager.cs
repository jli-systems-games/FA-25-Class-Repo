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
        // 저장 불러오기(선택)
        currentIndex = PlayerPrefs.GetInt("car_index", 0);
        if (PlayerPrefs.HasKey("car_color_r"))
        {
            float r = PlayerPrefs.GetFloat("car_color_r", 1f);
            float g = PlayerPrefs.GetFloat("car_color_g", 0f);
            float b = PlayerPrefs.GetFloat("car_color_b", 0f);
            float a = PlayerPrefs.GetFloat("car_color_a", 1f);
            savedColor = new Color(r, g, b, a);
        }

        SpawnCar(currentIndex);
        ApplySavedColorToCurrent();
    }

    public void NextCar()
    {
        if (carPrefabs.Count == 0) return;
        currentIndex = (currentIndex + 1) % carPrefabs.Count;
        SpawnCar(currentIndex);
        ApplySavedColorToCurrent();
        SaveIndex();
    }

    public void PrevCar()
    {
        if (carPrefabs.Count == 0) return;
        currentIndex = (currentIndex - 1 + carPrefabs.Count) % carPrefabs.Count;
        SpawnCar(currentIndex);
        ApplySavedColorToCurrent();
        SaveIndex();
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

        // UI 카드 스폰 (차 순서와 동일한 인덱스의 UI 프리팹 사용) ★ 추가
        if (uiParent && index < uiCardPrefabs.Count && uiCardPrefabs[index] != null)
        {
            currentCardUI = Instantiate(uiCardPrefabs[index], uiParent);
            currentCardUI.SetActive(true);
        }
        else
        {
            // 문제가 있으면 한 번만 로그로 확인
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
        SaveColor();
    }

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
        // 선택 인덱스/색 저장
        PlayerPrefs.SetInt("car_index", currentIndex);
        PlayerPrefs.SetFloat("car_color_r", savedColor.r);
        PlayerPrefs.SetFloat("car_color_g", savedColor.g);
        PlayerPrefs.SetFloat("car_color_b", savedColor.b);
        PlayerPrefs.SetFloat("car_color_a", savedColor.a);
        PlayerPrefs.Save();

        // 다음 씬 로드
        SceneManager.LoadScene(raceSceneName);
    }
}
