using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelDirector : MonoBehaviour
{
    // 同时兼容两种生成器
    OnlyUpNonsenseBuilder onlyUp;
    PlanarNonsenseBuilder planar;

    public TMP_Text hintText;        // 可空
    public int baseSteps = 24;
    public int level = 1;

    Queue<GameObject> rareQueue = new Queue<GameObject>();

    void Awake()
    {
        onlyUp = FindObjectOfType<OnlyUpNonsenseBuilder>();
        planar = FindObjectOfType<PlanarNonsenseBuilder>();
        if (!onlyUp && !planar)
        {
            Debug.LogError("No world builder found. Add PlanarNonsenseBuilder or OnlyUpNonsenseBuilder to the scene.");
        }
    }

    void Start()
    {
        BuildRareQueue();
        BuildLevel();
    }

    // —— 公共能力封装 —— //
    PrefabPalette GetPalette()
    {
        if (onlyUp && onlyUp.palette) return onlyUp.palette;
        if (planar && planar.palette) return planar.palette;
        return null;
    }

    void RebuildWorld(int steps)
    {
        if (onlyUp)
        {
            onlyUp.steps = steps;
            onlyUp.Rebuild();
        }
        if (planar)
        {
            // 平铺版本没有 steps 概念，直接重建即可
            planar.Rebuild();
        }
    }

    Vector3 GetSpawnPos()
    {
        if (onlyUp) return onlyUp.FirstPlatformCenter;
        if (planar) return planar.FirstPlatformCenter;
        return Vector3.up;
    }

    void SetRarePrefab(GameObject rare)
    {
        if (onlyUp) { onlyUp.SetRarePrefab(rare); onlyUp.SpawnRareCopies(); }
        if (planar) { planar.SetRarePrefab(rare); planar.SpawnRareSingleForPlayer(); }
    }

    GameObject GetCurrentRare()
    {
        if (onlyUp) return onlyUp.currentRarePrefab;
        if (planar) return planar.currentRarePrefab;
        return null;
    }

    // —— 稀有件池 —— //
    void BuildRareQueue()
    {
        rareQueue.Clear();

        var pal = GetPalette();
        var list = new List<GameObject>();
        if (pal != null && pal.rarePrefabs != null && pal.rarePrefabs.Count > 0)
            list.AddRange(pal.rarePrefabs);
        else if (pal != null && pal.platformPrefabs != null && pal.platformPrefabs.Count > 0)
            list.AddRange(pal.platformPrefabs);

        if (list.Count == 0)
        {
            Debug.LogError("Palette has no prefabs. Ensure NonsensePalette.asset is filled or set Fallback Floor Prefab.");
            return;
        }

        // 洗牌
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
        foreach (var r in list) rareQueue.Enqueue(r);
    }

    public void CompleteLevel()
    {
        level++;
        BuildLevel();
    }

    void BuildLevel()
    {
        if (rareQueue.Count == 0) BuildRareQueue();

        // 1) 重建世界
        int steps = Mathf.RoundToInt(baseSteps + level * 3.5f);
        RebuildWorld(steps);

        // 2) 选本关稀有件
        var rare = rareQueue.Dequeue();
        SetRarePrefab(rare);

        // 3) 刷新文字
        if (hintText)
        {
            string display = rare ? rare.name : "the rare piece";
            hintText.text = $"Level {level}\nFind and photograph: {display}\nPress F to take a photo. Press P to hide/show hint.";
        }

        // 4) 一帧后把玩家放到出生点
        StartCoroutine(SpawnLater());

        if (planar)
            planar.SpawnRareSingleForPlayer();

        // 6) 更新 Hint 预览
        FindObjectOfType<RareHintUI>()?.UpdateHint();
    }


    System.Collections.IEnumerator SpawnLater()
    {
        yield return null;
        var spawner = FindObjectOfType<SimpleSpawnOnFirstPlatform>();
        if (spawner) spawner.SpawnAt(GetSpawnPos());
    }
}
