using System.Collections;             
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LabMixingManager : MonoBehaviour
{
    [Header("선택된 아이템들 (Selector에서 세팅)")]
    List<ItemData> _selectedItems = new List<ItemData>();

    [Header("정답 레시피 (아이템 ID 5개)")]
    public string[] correctRecipeIds = new string[5];  
    [Header("책상 위 스폰 위치들 (5개)")]
    public Transform[] spawnPoints;

    [Header("스폰 간격(초)")]
    public float spawnInterval = 0.3f;

    [Header("이펙트 & 오디오")]
    public ParticleSystem boilingEffect;   
    public ParticleSystem failSmokeEffect; 
    public ParticleSystem successEffect;    
    public AudioSource boilingAudio;     
    public Text resultText;               

    [Header("국자 & 젓기 설정")]
    public LadleStirController ladle;
    public int requiredStirs = 5;

    [Header("국자 움직임 파티클")]
    public ParticleSystem ladleMoveEffect;  
    [Header("선택 초기화(옵션)")]
    public LabInventorySelector selector;

    int _currentStirCount = 0;
    bool _mixingInProgress = false;
    List<GameObject> _spawnedWorldItems = new List<GameObject>();

    void Start()
    {

        if (resultText != null)
            resultText.gameObject.SetActive(false);


        if (ladle != null)
        {
            if (ladle.onStirOnce == null)
                ladle.onStirOnce = new UnityEngine.Events.UnityEvent();

            ladle.onStirOnce.AddListener(OnStirOnce);
        }

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

    public void SetSelectedItems(List<ItemData> items)
    {
        _selectedItems = new List<ItemData>(items);
    }


    public void OnClickMixButton()
    {
        
        // 이제는 "아무 것도 없을 때만 막고",
        // 1~5개면 섞게 둠 (나중에 CheckRecipe에서 5개만 성공 처리)
        if (_selectedItems == null || _selectedItems.Count == 0)
        {
            Debug.LogWarning("적어도 1개 이상의 아이템이 선택되어야 섞을 수 있습니다.");
            return;
        }

        ClearSpawnedItems();

        // 한 번에 다 스폰 대신, 코루틴으로 순서대로 스폰
        StartCoroutine(SpawnItemsOnTableCoroutine());

        StartBoiling();

        _mixingInProgress = true;
        _currentStirCount = 0;

        if (resultText != null)
            resultText.gameObject.SetActive(false);
    }

   
    System.Collections.IEnumerator SpawnItemsOnTableCoroutine()
    {
        int count = Mathf.Min(spawnPoints.Length, _selectedItems.Count);

        for (int i = 0; i < count; i++)
        {
            var data = _selectedItems[i];
            if (data != null && data.worldPrefab != null)
            {
                Transform point = spawnPoints[i];
                GameObject spawned = Instantiate(data.worldPrefab, point.position, point.rotation);
                _spawnedWorldItems.Add(spawned);
            }

      
            if (spawnInterval > 0f && i < count - 1)
                yield return new WaitForSeconds(spawnInterval);
        }
    }


    void ClearSpawnedItems()
    {
        foreach (var go in _spawnedWorldItems)
        {
            if (go != null) Destroy(go);
        }
        _spawnedWorldItems.Clear();
    }

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
    void OnStirOnce()
    {
        if (!_mixingInProgress) return;

        _currentStirCount++;

        if (_currentStirCount >= requiredStirs)
        {
            _mixingInProgress = false;
            StopBoiling();

            if (ladleMoveEffect != null)
            {
                ladleMoveEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                ladleMoveEffect.gameObject.SetActive(false);
            }

            bool success = CheckRecipe();
            ShowResult(success);

            // 🟡 결과가 나온 뒤, 테이블 위에 스폰된 음식들 제거
            ClearSpawnedItems();

            // 🟥 선택/버튼 색 초기화 (인벤토리에는 아이템 그대로 남겨둠)
            if (selector != null)
                selector.ClearSelection();
        }
    }


    void HandleLadleMoveEffect()
    {
        if (ladleMoveEffect == null || ladle == null)
            return;

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

    void ShowResult(bool success)
    {
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

    IEnumerator PlayOneShotEffect(ParticleSystem ps)
    {
        ps.gameObject.SetActive(true);
        ps.Clear();
        ps.Play();

        var main = ps.main;
        float duration = main.duration + main.startLifetime.constantMax;
        yield return new WaitForSeconds(duration);

        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.gameObject.SetActive(false);
    }
}
