using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class NpcIndicator : MonoBehaviour
{
    [Header("카메라 설정 (필수 연결)")]
    public Camera playerCamera;

    [Header("UI 프리팹 및 캔버스")]
    // ⭐️ Canvas 아래에 배치할 화살표 UI 프리팹 (하나만 필요)
    public GameObject indicatorUIPrefab;
    public Transform indicatorContainer;

    [Header("순서 추적 설정")]
    // ⭐️ 추적할 NPC의 순서 목록 (Inspector에서 순서대로 연결)
    public List<Transform> sequentialNpcs;
    // ⭐️ 현재 추적 중인 NPC의 인덱스
    private int _currentIndex = 0;

    // ⭐️ 현재 활성화된 화살표 UI 요소
    private RectTransform _currentIndicatorRect;


    [Header("거리 및 크기 설정")]
    public float minDistanceToHide = 10f;
    public float indicatorScale = 0.05f;

    private const float EdgeBuffer = 50f;

    void Start()
    {
        if (playerCamera == null)
        {
            Debug.LogError("Player Camera 필드에 카메라를 연결해야 합니다.");
            enabled = false;
            return;
        }
        if (indicatorUIPrefab == null)
        {
            Debug.LogError("Indicator UIPrefab을 연결해야 합니다.");
            enabled = false;
            return;
        }

        // ⭐️ 화살표 UI를 미리 하나만 생성합니다.
        InitializeIndicator();


        SetCurrentTarget(_currentIndex);

        // ⭐️ 첫 번째 NPC 추적을 시작합니다.
        GoToNextNpc();
    }
    private void SetCurrentTarget(int index)
    {
        if (index < 0 || index >= sequentialNpcs.Count)
        {
            // 목록 끝에 도달했을 경우 추적 종료
            if (_currentIndicatorRect != null)
                _currentIndicatorRect.gameObject.SetActive(false);
            enabled = false;
            Debug.Log("모든 NPC 추적이 완료되었습니다!");
            return;
        }

        Transform targetNpc = sequentialNpcs[index];
        if (targetNpc == null)
        {
            // ⭐️ [Null 체크]: 만약 리스트의 다음 NPC가 비어있다면, 그 다음 NPC를 찾기 위해 GoToNextNpc()를 재귀 호출합니다.
            Debug.LogError($"인덱스 {index}의 NPC가 리스트에 연결되어 있지 않거나, 파괴되었습니다. 다음 대상으로 즉시 넘어갑니다.");
            GoToNextNpc(); // 유효하지 않으면 바로 다음 타겟을 시도합니다.
            return;
        }

        // 유효한 NPC가 있을 경우 인덱스를 업데이트하고 추적을 시작합니다.
        _currentIndex = index;
        Debug.Log("현재 추적 대상: " + targetNpc.name + " (인덱스: " + _currentIndex + ")");

        if (_currentIndicatorRect != null)
            _currentIndicatorRect.gameObject.SetActive(true);

        UpdateCurrentIndicatorDisplay(); // 즉시 업데이트하여 화살표가 바로 보이도록 합니다.
    }

    void InitializeIndicator()
    {
        GameObject indicatorGO = Instantiate(indicatorUIPrefab, indicatorContainer);
        _currentIndicatorRect = indicatorGO.GetComponent<RectTransform>();
        if (_currentIndicatorRect != null)
        {
            _currentIndicatorRect.gameObject.SetActive(false); // 시작 시 숨김
        }
    }

    // NpcIndicator.cs 파일 내

    // ⭐️ [수정할 함수]: 다음 NPC를 찾는 로직을 강화합니다.
    public void GoToNextNpc()
    {
        // 인덱스를 1 증가시킵니다.
        _currentIndex++;

        // 목록의 끝에 도달했는지 확인합니다.
        if (_currentIndex >= sequentialNpcs.Count)
        {
            Debug.Log("모든 NPC 추적이 완료되었습니다!");
            if (_currentIndicatorRect != null)
                _currentIndicatorRect.gameObject.SetActive(false);
            enabled = false;
            return;
        }

        Transform nextNpc = sequentialNpcs[_currentIndex];

        // ⭐️ [핵심 추가]: 현재 인덱스가 Null인지 확인하고, Null이면 즉시 다음 대상을 시도합니다.
        if (nextNpc == null)
        {
            Debug.LogWarning($"인덱스 {_currentIndex}의 NPC가 비어있습니다. 다음 NPC를 시도합니다.");
            // 재귀 호출: 유효하지 않은 항목을 건너뛰고 다음 대상을 찾기 위해 함수를 다시 호출합니다.
            GoToNextNpc();
            return;
        }

        // 유효한 NPC를 찾았을 경우
        Debug.Log("다음 추적 대상: " + nextNpc.name + " (인덱스: " + _currentIndex + ")");

        if (_currentIndicatorRect != null)
            _currentIndicatorRect.gameObject.SetActive(true);

        // ⭐️ (UpdateCurrentIndicatorDisplay() 함수가 정의되어 있다고 가정합니다.)
        UpdateCurrentIndicatorDisplay();
    }

    // ⭐️ [새로운 함수 추가]: 현재 NPC의 위치를 기반으로 화살표를 즉시 업데이트합니다.
    private void UpdateCurrentIndicatorDisplay()
    {
        // 현재 추적 대상이 없거나, 인디케이터가 없으면 업데이트 중단
        if (_currentIndex < 0 || _currentIndex >= sequentialNpcs.Count || _currentIndicatorRect == null || playerCamera == null) return;

        Transform targetNpc = sequentialNpcs[_currentIndex];
        if (targetNpc == null) return;

        // --- (이하, 기존 Update() 함수에 있던 모든 로직을 여기에 붙여넣습니다) ---
        Vector3 playerPosition = playerCamera.transform.position;
        Vector3 targetPosition = targetNpc.position;
        float distance = Vector3.Distance(targetPosition, playerPosition);

        // 1. [아주 가까이 오면 안 보이도록] 거리 제어
        if (distance <= minDistanceToHide)
        {
            if (_currentIndicatorRect.gameObject.activeSelf)
                _currentIndicatorRect.gameObject.SetActive(false);
            return;
        }

        // 2. 월드 좌표를 화면 좌표로 변환합니다.
        Vector3 screenPoint = playerCamera.WorldToScreenPoint(targetPosition);
        bool isBehind = screenPoint.z < 0;

        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Vector3 direction = screenPoint - screenCenter;

        // 3. [화살표 표시 로직]
        if (isBehind ||
            screenPoint.x < EdgeBuffer || screenPoint.x > Screen.width - EdgeBuffer ||
            screenPoint.y < EdgeBuffer || screenPoint.y > Screen.height - EdgeBuffer)
        {
            _currentIndicatorRect.gameObject.SetActive(true); // 이미 _currentIndicatorRect로 변경되었다고 가정

            // 4. [화면 가장자리에 고정] 
            if (isBehind) direction *= -1;

            float angle = Mathf.Atan2(direction.y, direction.x);
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            float maxClipDistance = Mathf.Min(
                (Screen.width / 2f - EdgeBuffer) / Mathf.Abs(cos),
                (Screen.height / 2f - EdgeBuffer) / Mathf.Abs(sin)
            );

            Vector3 clippedPosition = screenCenter + new Vector3(cos, sin, 0) * maxClipDistance;
            _currentIndicatorRect.position = clippedPosition;

            // 5. [방향 설정 및 회전]
            float rotation = angle * Mathf.Rad2Deg;
            _currentIndicatorRect.localRotation = Quaternion.Euler(0, 0, rotation - 90);

            // 6. [크기 유지 보정] 
            float currentScale = distance * indicatorScale;
            currentScale = Mathf.Clamp(currentScale, 0.5f, 5f);
            _currentIndicatorRect.localScale = new Vector3(currentScale, currentScale, 1f);
        }
        else
        {
            // NPC가 화면 안에 있다면 숨깁니다.
            _currentIndicatorRect.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (playerCamera == null) return;

        // ⭐️ [수정된 부분]: 코어 로직을 별도 함수로 호출
        UpdateCurrentIndicatorDisplay();
    }
}