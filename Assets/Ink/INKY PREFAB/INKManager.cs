using UnityEngine;
using UnityEngine.Events; // 🔥 新增
using TMPro;
using Ink.Runtime;
using System.Collections.Generic;

public class InkManager : MonoBehaviour
{
    [Header("Ink文件")]
    public TextAsset inkJSON;

    [Header("UI")]
    public TMP_Text nameText;
    public TMP_Text dialogueText;

    [Header("角色立绘系统")]
    public CharacterManager characterManager;

    [Header("场景物体控制")]
    public List<GameObjectLink> gameObjects;

    [Header("故事结束事件")]
    public UnityEvent onStoryEnd; // 🔥 核心新增

    private Story story;

    void Start()
    {
        story = new Story(inkJSON.text);
        ShowNext();
    }

    void Update()
    {
        if (Input.anyKeyDown)
            ShowNext();
    }

    void ShowNext()
    {
        if (!story.canContinue)
        {
            // 🔥 故事播放完毕，触发事件
            onStoryEnd?.Invoke();
            return;
        }

        string line = story.Continue().Trim();

        // 显示对话
        if (line.Contains(":"))
        {
            string[] parts = line.Split(':');
            nameText.text = parts[0].Trim();
            dialogueText.text = parts[1].Trim();
        }
        else
        {
            nameText.text = "";
            dialogueText.text = line;
        }

        // 🔥 处理Tag
        foreach (string tag in story.currentTags)
        {
            HandleTag(tag);
        }
    }

    void HandleTag(string tag)
    {
        string[] parts = tag.Split(' ');
        string command = parts[0];

        // 🎯 立绘切换
        if (command == "sprite" && parts.Length >= 3)
        {
            string characterName = parts[1];
            string spriteTag = parts[2];
            characterManager.ChangeSprite(characterName, spriteTag);
        }

        // 🎯 显示物体
        else if (command == "show" && parts.Length >= 2)
        {
            string objectName = parts[1];
            GameObjectLink link = gameObjects.Find(x => x.name == objectName);
            if (link != null)
                link.obj.SetActive(true);
        }

        // 🎯 隐藏物体
        else if (command == "hide" && parts.Length >= 2)
        {
            string objectName = parts[1];
            GameObjectLink link = gameObjects.Find(x => x.name == objectName);
            if (link != null)
                link.obj.SetActive(false);
        }
    }
}

[System.Serializable]
public class GameObjectLink
{
    public string name;
    public GameObject obj;
}