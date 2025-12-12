using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 超简单卡牌生成器
/// 只做三件事：1.随机选3张 2.生成在指定位置 3.确保符合规则
/// </summary>
public class SimpleCardSpawner : MonoBehaviour
{
    [Header("所有卡牌Prefab")]
    public List<GameObject> allCards = new List<GameObject>();

    [Header("生成位置")]
    public Transform pos1;
    public Transform pos2;
    public Transform pos3;

    void Start()
    {
        SpawnCards();
    }

    void SpawnCards()
    {
        // 筛选可用的卡
        List<GameObject> availableCards = new List<GameObject>();

        foreach (GameObject card in allCards)
        {
            if (card == null) continue;

            SimpleCard sc = card.GetComponent<SimpleCard>();
            if (sc != null && sc.CanUse())
            {
                availableCards.Add(card);
            }
        }

        Debug.Log($"可用卡牌: {availableCards.Count}张");

        // 随机打乱
        availableCards = availableCards.OrderBy(x => Random.value).ToList();

        // 生成3张
        if (availableCards.Count > 0 && pos1 != null)
        {
            Instantiate(availableCards[0], pos1.position, pos1.rotation, pos1);
            Debug.Log($"位置1: {availableCards[0].name}");
        }

        if (availableCards.Count > 1 && pos2 != null)
        {
            Instantiate(availableCards[1], pos2.position, pos2.rotation, pos2);
            Debug.Log($"位置2: {availableCards[1].name}");
        }

        if (availableCards.Count > 2 && pos3 != null)
        {
            Instantiate(availableCards[2], pos3.position, pos3.rotation, pos3);
            Debug.Log($"位置3: {availableCards[2].name}");
        }
    }
}