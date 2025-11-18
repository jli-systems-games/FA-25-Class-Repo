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
    public ParticleSystem boilingEffect;
    public ParticleSystem failSmokeEffect;
    public ParticleSystem successEffect;
    public AudioSource boilingAudio;
    public Text resultText;

    [Header("국자 & 젓기 설정")]
    public LadleStirController ladle;
    public int requiredStirs = 5;

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
    }


    public void SetSelectedItems(List<ItemData> items)
    {
        _selectedItems = new List<ItemData>(items);
    }

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
            boilingEffect.Play();
        if (boilingAudio != null)
            boilingAudio.Play();
    }

    void StopBoiling()
    {
        if (boilingEffect != null)
            boilingEffect.Stop();
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

            bool success = CheckRecipe();
            ShowResult(success);

            if (selector != null)
                selector.ClearSelection();
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
        if (success)
        {
            if (successEffect != null)
                successEffect.Play();

            if (resultText != null)
            {
                resultText.gameObject.SetActive(true);
                resultText.text = "success";
            }
        }
        else
        {
            if (failSmokeEffect != null)
                failSmokeEffect.Play();

            if (resultText != null)
            {
                resultText.gameObject.SetActive(true);
                resultText.text = "fail";
            }
        }
    }
}
