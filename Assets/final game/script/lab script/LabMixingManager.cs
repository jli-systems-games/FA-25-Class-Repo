using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public class Recipe
{
    public string[] requiredItemIds;
    public ItemData resultItem;
}

public class LabMixingManager : MonoBehaviour
{
    [Header("선택된 아이템들 (Selector에서 세팅)")]
    List<ItemData> _selectedItems = new List<ItemData>();

    public TMP_Text toastMessage;

    [Header("레시피 목록")]
    public Recipe[] allRecipes;
    public ItemData defaultResultItem;
    public ItemData successResultItem;
    public ItemData hairRecoverItem;

    [Header("책상 위 스폰 위치들")]
    public Transform[] spawnPoints;
    public float spawnInterval = 0.3f;

    [Header("이펙트 & 오디오")]
    public ParticleSystem boilingEffect;
    public ParticleSystem failSmokeEffect;
    public ParticleSystem successEffect;
    public AudioSource boilingAudio;

    public AudioSource sfxSource;
    public AudioClip successFoodSound;
    public AudioClip normalFoodSound;

    [Header("결과 텍스트(TMP)")]
    public TMP_Text resultText;

    [Header("결과 픽업 힌트 텍스트(TMP)")]
    public TMP_Text pickupHintText;
    public string pickupMessage = "Click to pick up food";

    [Header("국자 & 젓기 설정")]
    public LadleStirController ladle;
    public int requiredStirs = 5;

    [Header("국자 움직임 파티클")]
    public ParticleSystem ladleMoveEffect;

    [Header("선택 초기화(옵션)")]
    public LabInventorySelector selector;

    [Header("결과 아이템 월드 스폰")]
    public Transform resultSpawnPoint;

    int _currentStirCount = 0;
    bool _mixingInProgress = false;
    List<GameObject> _spawnedWorldItems = new List<GameObject>();
    ItemData _lastResultItem;


    bool _melonBreadUsed = false; //

    void Start()
    {
        if (resultText != null)
            resultText.gameObject.SetActive(false);

        if (pickupHintText != null)
            pickupHintText.gameObject.SetActive(false);

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
        if (items == null)
            _selectedItems = new List<ItemData>();
        else
            _selectedItems = new List<ItemData>(items);
    }

    IEnumerator ShowResultPickupHint()
    {
        if (pickupHintText != null)
        {
            pickupHintText.text = pickupMessage;
            pickupHintText.gameObject.SetActive(true);
            yield return new WaitForSeconds(2.0f);
            pickupHintText.gameObject.SetActive(false);
        }
    }

    public void MixItems()
    {
        var invUI = FindFirstObjectByType<InventoryUI>();
        if (invUI != null)
            invUI.CloseInventory();

        _mixingInProgress = true;
        _currentStirCount = 0;
    }

    public void OnClickMixButton()
    {
        MixItems();

        if (_selectedItems == null || _selectedItems.Count == 0)
        {
            Debug.LogWarning("적어도 1개 이상의 아이템이 선택되어야 섞을 수 있습니다.");
            return;
        }

        ClearSpawnedItems();
        StartCoroutine(SpawnItemsOnTableCoroutine());
        StartBoiling();
        _mixingInProgress = true;
        _currentStirCount = 0;

        if (resultText != null)
            resultText.gameObject.SetActive(false);
    }

    IEnumerator SpawnItemsOnTableCoroutine()
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
            if (go != null)
                Destroy(go);
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
        if (!_mixingInProgress)
            return;

        _currentStirCount++;

        if (_currentStirCount >= requiredStirs)
        {
            _mixingInProgress = false;
            StopBoiling();

            ItemData resultItem = FindResultItem();
            _lastResultItem = resultItem;

            bool success = (resultItem != null && resultItem == successResultItem);

            if (resultItem != null)
            {
                SpawnResultItemWorldObject(resultItem);
                StartCoroutine(ShowResultPickupHint());

                PlayFoodSound(success);
            }

            if (resultItem != null && resultItem == hairRecoverItem)
            {
                if (!_melonBreadUsed)
                {
                    _melonBreadUsed = true;

                    HealAllNPCsOneStep();
                    StartCoroutine(ShowToast("+ Baldness condition has improved"));
                }
                else
                {
                }
            }


            ShowResult(success, resultItem);
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
        if (_selectedItems == null || _selectedItems.Count == 0 || allRecipes == null)
            return defaultResultItem;

        var selectedIds = _selectedItems
            .Where(d => d != null)
            .Select(d => d.itemId)
            .OrderBy(id => id)
            .ToArray();

        if (selectedIds.Length == 0)
            return defaultResultItem;

        foreach (var recipe in allRecipes)
        {
            if (recipe == null ||
                recipe.requiredItemIds == null ||
                recipe.requiredItemIds.Length == 0 ||
                recipe.resultItem == null)
                continue;

            var requiredIds = recipe.requiredItemIds
                .Where(id => !string.IsNullOrEmpty(id))
                .OrderBy(id => id)
                .ToArray();

            if (selectedIds.Length != requiredIds.Length)
                continue;

            if (selectedIds.SequenceEqual(requiredIds))
                return recipe.resultItem;
        }

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

        GameObject spawnedObject = Instantiate(
            resultItem.worldPrefab,
            resultSpawnPoint.position,
            resultSpawnPoint.rotation);

        WorldItemPickup pickup = spawnedObject.GetComponent<WorldItemPickup>();
        if (pickup != null)
        {
            pickup.itemData = resultItem;
            pickup.requiredToolName = "";
        }
        else
        {
            Debug.LogError("결과물 월드 프리팹에 WorldItemPickup.cs 스크립트가 없습니다.");
        }
    }

    void ShowResult(bool success, ItemData resultItem)
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
            StartCoroutine(SuccessSequence(resultItem));
        }
        else
        {
            if (failSmokeEffect != null)
                StartCoroutine(PlayOneShotEffect(failSmokeEffect));

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

    void HealAllNPCsOneStep()
    {
        NPCHairController[] npcs = FindObjectsOfType<NPCHairController>();
        foreach (var npc in npcs)
        {
            if (npc != null)
                npc.RecoverHair(1);
        }
    }

    IEnumerator ShowToast(string msg)
    {
        if (toastMessage == null) yield break;

        toastMessage.text = msg;
        toastMessage.gameObject.SetActive(true);

        Color c = toastMessage.color;

        toastMessage.color = new Color(c.r, c.g, c.b, 1f);

        Vector3 startPos = toastMessage.rectTransform.anchoredPosition;
        Vector3 endPos = startPos + new Vector3(0, 80f, 0);

        float t = 0f;
        float duration = 1.2f;

        while (t < duration)
        {
            t += Time.deltaTime;

            toastMessage.rectTransform.anchoredPosition =
                Vector3.Lerp(startPos, endPos, t / duration);

            float alpha = Mathf.Lerp(1f, 0f, t / duration);
            toastMessage.color = new Color(c.r, c.g, c.b, alpha);

            yield return null;
        }

        toastMessage.gameObject.SetActive(false);
        toastMessage.rectTransform.anchoredPosition = startPos;
    }
    IEnumerator SuccessSequence(ItemData resultItem)
    {
        if (successEffect != null)
        {
            successEffect.gameObject.SetActive(true);
            successEffect.Clear();
            successEffect.Play();
        }

        if (resultText != null)
        {
            resultText.gameObject.SetActive(true);
            if (resultItem != null)
                resultText.text = resultItem.displayName + " crafted!!";
            else
                resultText.text = "success";
        }

        yield return new WaitForSeconds(5f);

        if (successEffect != null)
        {
            successEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            successEffect.gameObject.SetActive(false);
        }

        SceneManager.LoadScene("success");
    }

    void PlayFoodSound(bool success)
    {
        Debug.Log($"PlayFoodSound called. success={success}, sfxSource={sfxSource}, successClip={successFoodSound}, normalClip={normalFoodSound}");



        if (sfxSource == null) return;

        AudioClip clip = success ? successFoodSound : normalFoodSound;
        if (clip == null) return;

        sfxSource.PlayOneShot(clip);
    }



}
