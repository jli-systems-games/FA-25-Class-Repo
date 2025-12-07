using System.Collections.Generic;
using UnityEngine;

public class SideQuestManager : MonoBehaviour
{
    public static SideQuestManager Instance;

    [System.Serializable]
    public class SideQuest
    {
        public string id;
        public string title;
        public bool completed;
    }

    [Header("Quest Data")]
    public List<SideQuest> quests = new List<SideQuest>();

    [Header("UI References")]
    public Transform questListParent;
    public GameObject questItemPrefab;

    private Dictionary<string, SideQuest> questById = new Dictionary<string, SideQuest>();
    private Dictionary<string, QuestItemUI> questUIById = new Dictionary<string, QuestItemUI>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        questById.Clear();
        foreach (var q in quests)
        {
            if (!string.IsNullOrEmpty(q.id) && !questById.ContainsKey(q.id))
            {
                questById.Add(q.id, q);
            }
        }
    }

    private void Start()
    {
        BuildQuestListUI();
    }

    void BuildQuestListUI()
    {
        if (questListParent == null || questItemPrefab == null) return;

        foreach (Transform child in questListParent)
        {
            if (child.gameObject != questItemPrefab)
            {
                Destroy(child.gameObject);
            }
        }

        questUIById.Clear();

        foreach (var q in quests)
        {
            GameObject itemGO = Instantiate(questItemPrefab, questListParent);
            itemGO.SetActive(true);

            QuestItemUI itemUI = itemGO.GetComponent<QuestItemUI>();
            if (itemUI != null)
            {
                itemUI.Setup(q.title, q.completed);
                questUIById[q.id] = itemUI;
            }
        }

        questItemPrefab.SetActive(false);
    }

    public void CompleteQuest(string questId)
    {
        if (string.IsNullOrEmpty(questId)) return;

        if (!questById.TryGetValue(questId, out SideQuest quest))
        {
            Debug.LogWarning("SideQuestManager: no quest with id " + questId);
            return;
        }

        if (quest.completed) return;

        quest.completed = true;
        Debug.Log("SideQuest completed: " + quest.title);

        if (questUIById.TryGetValue(questId, out QuestItemUI ui))
        {
            ui.SetCompleted(true);
        }
    }
}
