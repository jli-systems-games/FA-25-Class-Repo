using System.Collections;                     // 👈 코루틴용
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LabMixingManager : MonoBehaviour
{
    [Header("선택된 아이템들 (Selector에서 세팅)")]
    List<ItemData> _selectedItems = new List<ItemData>();

    [Header("정답 레시피 (아이템 ID 5개)")]
    public string[] correctRecipeIds = new string[5];  // "CrocNoseHair", "SnailGoo" 이런 문자열

    [Header("책상 위 스폰 위치들 (5개)")]
    public Transform[] spawnPoints;

    [Header("이펙트 & 오디오")]
    public ParticleSystem boilingEffect;    // 끓는 이펙트 (Loop)
    public ParticleSystem failSmokeEffect;  // 실패 이펙트 (One-shot)
    public ParticleSystem successEffect;    // 성공 이펙트 (One-shot)
    public AudioSource boilingAudio;        // 끓는 소리
    public Text resultText;                 // "success" / "fail" 텍스트

    [Header("국자 & 젓기 설정")]
    public LadleStirController ladle;
    public int requiredStirs = 5;

    [Header("국자 움직임 파티클")]
    public ParticleSystem ladleMoveEffect;  // 국자를 움직이는 동안만 나올 파티클 (Loop)

    [Header("선택 초기화(옵션)")]
    public LabInventorySelector selector;

    int _currentStirCount = 0;
    bool _mixingInProgress = false;
    List<GameObject> _spawnedWorldItems = new List<GameObject>();

    void Start()
    {
        // 결과 텍스트 숨기기
        if (resultText != null)
            resultText.gameObject.SetActive(false);

        // 국자 1회 젓기 이벤트 연결
        if (ladle != null)
        {
            if (ladle.onStirOnce == null)
                ladle.onStirOnce = new UnityEngine.Events.UnityEvent();

            ladle.onStirOnce.AddListener(OnStirOnce);
        }

        // 시작할 때는 파티클 오브젝트들만 비활성화 (삭제 X)
        if (boilingEffect != null)
            boilingEffect.gameObject.SetActive(false);
        if (failSmokeEffect != null)
            failSmokeEffect.gameObject.SetActive(false);
        if (successEffect != null)
            successEffect.gameObject.SetActive(false);
        if (ladleMoveEffect != null)
            ladleMoveEffect.gameObject.SetActive(false);
    }

    void Update()
    {
        HandleLadleMoveEffect();
    }

    // LabInventorySelector에서 선택된 아이템 리스트를 전달해 줌
    public void SetSelectedItems(List<ItemData> items)
    {
        _selectedItems = new List<ItemData>(items);
    }

    // mix 버튼 눌렀을 때
    public void OnClickMixButton()
    {
        if (_selectedItems == null || _selectedItems.Count != 5)
        {
            Debug.LogWarning("5개의 아이템이 선택되어야 섞을 수 있습니다.");
            return;
        }

        ClearSpawnedItems();
        SpawnItemsOnTable();
        StartBoiling();

        _mixingInProgress = true;
        _currentStirCount = 0;

        if (resultText != null)
            resultText.gameObject.SetActive(false);
    }

    // 선택된 아이템들을 책상 위 위치에 스폰
    void SpawnItemsOnTable()
    {
        int count = Mathf.Min(spawnPoints.Length, _selectedItems.Count);

        for (int i = 0; i < count; i++)
        {
            var data = _selectedItems[i];
            if (data == null || data.worldPrefab == null) continue;

            Transform point = spawnPoints[i];
            GameObject spawned = Instantiate(data.worldPrefab, point.position, point.rotation);
            _spawnedWorldItems.Add(spawned);
        }
    }

    // 이전에 스폰된 아이템들 정리 (이건 Destroy 써도 괜찮음 – 사과/재료만)
    void ClearSpawnedItems()
    {
        foreach (var go in _spawnedWorldItems)
        {
            if (go != null) Destroy(go);
        }
        _spawnedWorldItems.Clear();
    }

    // 끓는 이펙트 + 소리 시작 (활성화)
    void StartBoiling()
    {
        if (boilingEffect != null)
        {
            boilingEffect.gameObject.SetActive(true);
            boilingEffect.Clear();
            boilingEffect.Play();
        }

        if (boilingAudio != null)
        {
            boilingAudio.Stop();
            boilingAudio.Play();
        }
    }

    // 끓는 이펙트 + 소리 정지 (비활성화)
    void StopBoiling()
    {
        if (boilingEffect != null)
        {
            boilingEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            boilingEffect.gameObject.SetActive(false);
        }

        if (boilingAudio != null)
            boilingAudio.Stop();
    }

    // 국자 한 번 저을 때마다 호출
    void OnStirOnce()
    {
        if (!_mixingInProgress) return;

        _currentStirCount++;

        if (_currentStirCount >= requiredStirs)
        {
            _mixingInProgress = false;
            StopBoiling();

            // 국자 움직임 파티클도 완전히 끄기
            if (ladleMoveEffect != null)
            {
                ladleMoveEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                ladleMoveEffect.gameObject.SetActive(false);
            }

            bool success = CheckRecipe();
            ShowResult(success);

            if (selector != null)
                selector.ClearSelection();
        }
    }

    // 국자 움직이는 동안에만 나오는 파티클 제어
    void HandleLadleMoveEffect()
    {
        if (ladleMoveEffect == null || ladle == null)
            return;

        // mix 버튼 누른 뒤 + 드래그 중일 때만 파티클 켜기
        bool shouldPlay = _mixingInProgress && ladle.IsDragging;
        bool isActive = ladleMoveEffect.gameObject.activeSelf;

        if (shouldPlay && !isActive)
        {
            ladleMoveEffect.gameObject.SetActive(true);
            ladleMoveEffect.Clear();
            ladleMoveEffect.Play();
        }
        else if (!shouldPlay && isActive)
        {
            ladleMoveEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ladleMoveEffect.gameObject.SetActive(false);
        }
    }

    // 선택된 아이템들이 정답 레시피와 일치하는지 체크
    bool CheckRecipe()
    {
        if (_selectedItems == null || _selectedItems.Count != 5)
            return false;
        if (correctRecipeIds == null || correctRecipeIds.Length != 5)
            return false;

        var selectedIds = _selectedItems
            .Where(d => d != null)
            .Select(d => d.itemId)
            .ToList();

        foreach (var id in correctRecipeIds)
        {
            if (!selectedIds.Contains(id))
                return false;
        }

        return true;
    }

    // 성공/실패 이펙트 + 텍스트 표시 (잠깐 활성화했다가 다시 꺼줌)
    void ShowResult(bool success)
    {
        // 둘 다 일단 꺼두고 시작
        if (successEffect != null)
        {
            successEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            successEffect.gameObject.SetActive(false);
        }
        if (failSmokeEffect != null)
        {
            failSmokeEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            failSmokeEffect.gameObject.SetActive(false);
        }

        if (success)
        {
            if (successEffect != null)
            {
                StartCoroutine(PlayOneShotEffect(successEffect));
            }

            if (resultText != null)
            {
                resultText.gameObject.SetActive(true);
                resultText.text = "success";
            }
        }
        else
        {
            if (failSmokeEffect != null)
            {
                StartCoroutine(PlayOneShotEffect(failSmokeEffect));
            }

            if (resultText != null)
            {
                resultText.gameObject.SetActive(true);
                resultText.text = "fail";
            }
        }
    }

    // 한 번만 켰다가 자동으로 비활성화하는 코루틴
    IEnumerator PlayOneShotEffect(ParticleSystem ps)
    {
        ps.gameObject.SetActive(true);
        ps.Clear();
        ps.Play();

        // 대략적인 지속 시간만큼 기다렸다가
        var main = ps.main;
        float duration = main.duration + main.startLifetime.constantMax;
        yield return new WaitForSeconds(duration);

        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.gameObject.SetActive(false);
    }
}
