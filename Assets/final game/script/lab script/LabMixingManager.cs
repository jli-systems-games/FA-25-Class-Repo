using System.Collections;             
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Recipe
{

    [Tooltip("이 레시피를 만드는 데 필요한 5가지 재료의 Item ID 목록입니다.")]
    public string[] requiredItemIds = new string[5];

    [Tooltip("이 레시피 성공 시 얻게 되는 결과 아이템 데이터입니다.")]
    public ItemData resultItem;
}

public class LabMixingManager : MonoBehaviour
{

    //public NpcIndicator indicatorSequencer;
  

    [Header("선택된 아이템들 (Selector에서 세팅)")]
    List<ItemData> _selectedItems = new List<ItemData>();

    public Recipe[] allRecipes;

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

    public TMPro.TMP_Text pickupHintText;
    public string pickupMessage = "Click to pick up food";

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

    [Header("레시피 목록")]
    public ItemData defaultResultItem;

    public Transform resultSpawnPoint;

    IEnumerator ShowResultPickupHint()
    {
        if (pickupHintText != null)
        {
            // 텍스트 설정 및 활성화
            pickupHintText.text = pickupMessage;
            pickupHintText.gameObject.SetActive(true);

            // 2초 대기
            yield return new WaitForSeconds(2.0f);

            // 비활성화
            pickupHintText.gameObject.SetActive(false);
        }
    }

    public void MixItems()
    {
        

        // ⭐️ [추가] 믹싱 시작과 동시에 인벤토리를 닫습니다.
        var invUI = FindFirstObjectByType<InventoryUI>();
        if (invUI != null)
        {
            invUI.CloseInventory(); // 인벤토리 닫기 (이 함수가 InventoryUI.cs에 있어야 함)
        }

        _mixingInProgress = true;
        _currentStirCount = 0;
    }

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
    // LabMixingManager.cs 파일 내

    void OnStirOnce()
    {
        if (!_mixingInProgress) return;

        _currentStirCount++;

        if (_currentStirCount >= requiredStirs)
        {
            _mixingInProgress = false;
            StopBoiling();

            // ... (국자 이펙트 종료 로직 유지) ...

            // ⭐️ [수정] 레시피를 확인하고 결과 아이템을 얻습니다.
            ItemData resultItem = FindResultItem();

            // ItemData가 defaultResultItem이 아닐 때만 성공으로 간주
            bool success = (resultItem != null) && (resultItem != defaultResultItem);

            // 1. 결과 아이템 스폰 (인벤토리에 바로 추가 대신)
            if (resultItem != null)
            {
                SpawnResultItemWorldObject(resultItem);

                StartCoroutine(ShowResultPickupHint());
            }

            ShowResult(success);
            ClearSpawnedItems();

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

    ItemData FindResultItem()
    {
        if (_selectedItems == null || _selectedItems.Count != 5 || allRecipes == null)
        {
            return defaultResultItem;
        }

        var selectedIds = _selectedItems
            .Where(d => d != null)
            .Select(d => d.itemId)
            .OrderBy(id => id) // ⭐️ Linq 필요
            .ToArray();

        if (selectedIds.Length != 5)
            return defaultResultItem;

        // 2. 모든 레시피를 돌면서 일치하는지 확인
        foreach (var recipe in allRecipes)
        {
            if (recipe == null || recipe.requiredItemIds.Length != 5 || recipe.resultItem == null)
                continue;

            // ⭐️ [문제의 코드] 레시피 ID도 정렬하여 requiredIds 변수에 저장합니다.
            // requiredIds는 함수가 아닌, 정렬된 문자열 배열입니다.
            var requiredIds = recipe.requiredItemIds.OrderBy(id => id).ToArray();

            // 선택된 ID 배열과 레시피 ID 배열이 완전히 동일한지 확인
            if (selectedIds.SequenceEqual(requiredIds)) // ⭐️ Linq 필요
            {
                // 레시피 일치! 결과 아이템 반환
                return recipe.resultItem;
            }
        }

        // 3. 일치하는 레시피 없음 -> 기본 결과물 반환
        Debug.LogWarning("일치하는 레시피를 찾지 못했습니다. 기본 결과물을 반환합니다.");
        return defaultResultItem;
    }
    void SpawnResultItemWorldObject(ItemData resultItem)
    {
        if (resultItem == null || resultItem.worldPrefab == null)
        {
            Debug.LogError("결과 아이템 데이터 또는 월드 프리팹이 설정되지 않았습니다.");
            return;
        }

        if (resultSpawnPoint == null)
        {
            Debug.LogError("결과물 스폰 위치(resultSpawnPoint)가 설정되지 않았습니다.");
            return;
        }

        // ItemData에 연결된 월드 프리팹을 스폰합니다.
        GameObject spawnedObject = Instantiate(
            resultItem.worldPrefab,
            resultSpawnPoint.position,
            resultSpawnPoint.rotation);

        // WorldItemPickup 스크립트를 찾아 데이터 연결
        // 이 스크립트가 ItemData를 가지고 있어야 클릭 시 인벤토리에 추가 가능
        WorldItemPickup pickup = spawnedObject.GetComponent<WorldItemPickup>();
        if (pickup != null)
        {
            pickup.itemData = resultItem;
            // 랩에서 나온 결과물이므로 도구 검사를 건너뛰도록 설정
            pickup.requiredToolName = "";
        }
        else
        {
            Debug.LogError("결과물 월드 프리팹에 WorldItemPickup.cs 스크립트가 없습니다. 클릭해서 줍기가 불가능합니다.");
        }
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
            SceneManager.LoadScene("success");
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
