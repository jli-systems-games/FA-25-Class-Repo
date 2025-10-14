using UnityEngine;
using System.Collections.Generic;

public class CarCustomizerManager : MonoBehaviour
{
    [Header("교체할 차 프리팹들(각 프리팹엔 CarColor가 붙어 있어야 함)")]
    public List<GameObject> carPrefabs = new List<GameObject>();

    [Header("생성 위치/회전 기준")]
    public Transform spawnPoint;

    int currentIndex = 0;
    GameObject currentCar;
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
        if (currentCar) Destroy(currentCar);

        Vector3 pos = spawnPoint ? spawnPoint.position : Vector3.zero;
        Quaternion rot = spawnPoint ? spawnPoint.rotation : Quaternion.identity;

        currentCar = Instantiate(carPrefabs[index], pos, rot);
        currentCarColor = currentCar.GetComponentInChildren<CarColor>(true);
        if (!currentCarColor)
            Debug.LogWarning("새 차 프리팹에 CarColor가 없습니다. Body Renderer 연결된 CarColor를 붙여주세요.");
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
}
