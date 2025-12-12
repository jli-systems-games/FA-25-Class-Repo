using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 卡牌选择系统 - 独立按钮版本
/// 卡牌仅用于展示，三个独立按钮控制选择
/// </summary>
public class CardSelector : MonoBehaviour
{
    [Header("卡牌预制体列表")]
    public List<GameObject> allCardPrefabs = new List<GameObject>();

    [Header("生成位置")]
    public Transform[] spawnPositions = new Transform[3];

    [Header("独立选择按钮")]
    public Button selectButton1;  // 选择位置1的按钮
    public Button selectButton2;  // 选择位置2的按钮
    public Button selectButton3;  // 选择位置3的按钮

    [Header("场景设置")]
    public string nextSceneName = "Level2";
    public float sceneTransitionDelay = 1.2f;

    [Header("动画设置")]
    public Animator animator1;
    public Animator animator2;

    [Header("调试")]
    public bool showDebugInfo = true;

    private List<GameObject> spawnedCards = new List<GameObject>();
    private string[] cardNamesAtPositions = new string[3];  // 记录每个位置的卡牌名称

    void Start()
    {
        GenerateCards();
        SetupButtons();
    }

    void SetupButtons()
    {
        if (selectButton1 != null)
        {
            selectButton1.onClick.RemoveAllListeners();
            selectButton1.onClick.AddListener(() => OnPositionSelected(0));
        }

        if (selectButton2 != null)
        {
            selectButton2.onClick.RemoveAllListeners();
            selectButton2.onClick.AddListener(() => OnPositionSelected(1));
        }

        if (selectButton3 != null)
        {
            selectButton3.onClick.RemoveAllListeners();
            selectButton3.onClick.AddListener(() => OnPositionSelected(2));
        }

        if (showDebugInfo)
        {
            Debug.Log("独立选择按钮已设置完成");
        }
    }

    void GenerateCards()
    {
        ClearCards();

        List<GameObject> availableCards = GetAvailableCards();

        if (availableCards.Count < 3)
        {
            Debug.LogWarning($"可用卡牌不足3张！当前可用: {availableCards.Count}");
        }

        List<GameObject> selectedCards = availableCards.OrderBy(x => Random.value).Take(3).ToList();

        for (int i = 0; i < selectedCards.Count && i < spawnPositions.Length; i++)
        {
            if (spawnPositions[i] == null || selectedCards[i] == null)
                continue;

            GameObject card = Instantiate(selectedCards[i], spawnPositions[i].position, spawnPositions[i].rotation, spawnPositions[i]);

            // 记录这个位置的卡牌名称
            cardNamesAtPositions[i] = selectedCards[i].name;

            // 移除卡牌上的Button组件（如果有的话），因为现在不需要卡牌自己响应点击
            Button cardButton = card.GetComponent<Button>();
            if (cardButton != null)
            {
                cardButton.interactable = false;  // 或者直接 Destroy(cardButton);
            }

            spawnedCards.Add(card);

            if (showDebugInfo)
            {
                Debug.Log($"位置 {i} 生成卡牌: {selectedCards[i].name}");
            }
        }
    }

    List<GameObject> GetAvailableCards()
    {
        List<GameObject> available = new List<GameObject>();

        foreach (GameObject card in allCardPrefabs)
        {
            if (card == null) continue;

            string cardName = card.name.ToLower();

            if (IsCardAvailable(cardName))
            {
                available.Add(card);
            }
        }

        return available;
    }

    bool IsCardAvailable(string cardName)
    {
        if (cardName.Contains("redqueen1"))
        {
            return false;
        }

        if (cardName.Contains("redqueen2"))
        {
            return PlayerAbilityManager.Instance.redQueenLevel >= 1;
        }

        if (cardName.Contains("alice2"))
        {
            return PlayerAbilityManager.Instance.aliceLevel >= 1;
        }

        if (cardName.Contains("alice1"))
        {
            return PlayerAbilityManager.Instance.aliceLevel == 0;
        }

        if (cardName.Contains("shootspeed") || cardName.Contains("flyspeed") || cardName.Contains("size"))
        {
            return true;
        }

        return false;
    }

    // 当某个位置的按钮被点击
    void OnPositionSelected(int positionIndex)
    {
        if (positionIndex < 0 || positionIndex >= cardNamesAtPositions.Length)
        {
            Debug.LogWarning($"位置索引无效: {positionIndex}");
            return;
        }

        string cardName = cardNamesAtPositions[positionIndex];

        if (string.IsNullOrEmpty(cardName))
        {
            Debug.LogWarning($"位置 {positionIndex} 没有卡牌");
            return;
        }

        if (showDebugInfo)
        {
            Debug.Log($"========== 选择位置 {positionIndex + 1} ==========");
            Debug.Log($"📝 该位置的卡牌: {cardName}");
        }

        ActivateAbility(cardName);
        TriggerAnimations();
        StartCoroutine(DelayedSceneTransition());
    }

    void ActivateAbility(string cardName)
    {
        string lowerName = cardName.ToLower();

        if (lowerName.Contains("redqueen"))
        {
            PlayerAbilityManager.Instance.UpgradeRedQueen();
            if (showDebugInfo) Debug.Log("✅ 激活技能: 红皇后技能升级！");
        }
        else if (lowerName.Contains("alice"))
        {
            PlayerAbilityManager.Instance.UpgradeAlice();
            if (showDebugInfo) Debug.Log("✅ 激活技能: 爱丽丝技能升级！");
        }
        else if (lowerName.Contains("shootspeed"))
        {
            PlayerAbilityManager.Instance.AddFireRateStack();
            if (showDebugInfo) Debug.Log("✅ 激活技能: 发射速度提升！");
        }
        else if (lowerName.Contains("flyspeed"))
        {
            PlayerAbilityManager.Instance.AddBulletSpeedStack();
            if (showDebugInfo) Debug.Log("✅ 激活技能: 子弹飞行速度提升！");
        }
        else if (lowerName.Contains("size"))
        {
            PlayerAbilityManager.Instance.AddBulletSizeStack();
            if (showDebugInfo) Debug.Log("✅ 激活技能: 子弹大小提升！");
        }
        else
        {
            if (showDebugInfo) Debug.LogWarning($"⚠️ 未识别的卡牌: {cardName}");
        }

        if (showDebugInfo)
        {
            PlayerAbilityManager.Instance.PrintAbilityStatus();
        }
    }

    void TriggerAnimations()
    {
        if (animator1 != null)
        {
            animator1.SetTrigger("action");
            if (showDebugInfo) Debug.Log("触发 animator1 的 action trigger");
        }

        if (animator2 != null)
        {
            animator2.SetTrigger("action2");
            if (showDebugInfo) Debug.Log("触发 animator2 的 action2 trigger");
        }
    }

    System.Collections.IEnumerator DelayedSceneTransition()
    {
        yield return new WaitForSeconds(sceneTransitionDelay);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            if (showDebugInfo)
            {
                Debug.Log($"切换到场景: {nextSceneName}");
            }
            SceneManager.LoadScene(nextSceneName);
        }
    }

    void ClearCards()
    {
        foreach (GameObject card in spawnedCards)
        {
            if (card != null)
            {
                Destroy(card);
            }
        }
        spawnedCards.Clear();
        cardNamesAtPositions = new string[3];
    }
}