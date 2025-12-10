using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;
using UnityEngine.SceneManagement;
using System.Collections;

public class LinearInkController : MonoBehaviour
{
    [Header("Ink JSON Asset")]
    public TextAsset inkJSONAsset;

    [Header("UI")]
    public TMP_Text nameText;
    public TMP_Text storyText;
    public Button continueButton;
    public CharacterManager characterManager;
    public SimpleSceneManager SceneManager;

    [Header("Typewriter Settings")]
    public float typewriterSpeed = 0.05f;
    public bool canSkipTypewriter = true;

    [Header("Tutorial System")]
    public TutorialEventManager tutorialManager; // 🟢 教程事件管理器

    private Story story;
    private bool isTyping = false;
    private Coroutine typewriterCoroutine;
    private string currentFullText = "";

    // 🟢 新增：是否正在等待教程事件完成
    private bool isWaitingForTutorialCompletion = false;

    void Start()
    {
        story = new Story(inkJSONAsset.text);
        ShowNextLine();
        continueButton.onClick.AddListener(OnContinueButtonClick);

        // 🟢 注册教程事件完成回调
        if (tutorialManager != null)
        {
            tutorialManager.OnEventCompleted += OnTutorialEventCompleted;
        }
    }

    void Update()
    {
        // 任何时候都允许按键检测
        if (Input.anyKeyDown)
        {
            OnContinueButtonClick();
        }
    }

    void OnContinueButtonClick()
    {
        // 🟢 如果正在等待教程完成，不允许继续
        if (isWaitingForTutorialCompletion)
        {
            Debug.Log("请先完成当前教程任务！");
            // 可选：播放一个提示音或者抖动UI
            return;
        }

        // 如果正在打字，点击跳过打字机效果
        if (isTyping && canSkipTypewriter)
        {
            SkipTypewriter();
            return;
        }

        // 如果没在打字，显示下一行
        if (!isTyping)
        {
            ShowNextLine();
        }
    }

    void ShowNextLine()
    {
        if (story.canContinue)
        {
            string text = story.Continue().Trim();

            string speaker = "";
            string dialogue = "";

            // 提取说话人和对白
            if (text.Contains(":"))
            {
                string[] parts = text.Split(new char[] { ':' }, 2);
                speaker = parts[0].Trim();
                dialogue = parts[1].Trim();
            }
            else
            {
                speaker = "";
                dialogue = text;
            }

            nameText.text = speaker;
            currentFullText = dialogue;

            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
            }
            typewriterCoroutine = StartCoroutine(TypewriterEffect(dialogue));

            // 读取当前行的标签
            foreach (string tag in story.currentTags)
            {
                HandleTag(tag);
            }
        }
        else
        {
            if (SceneManager != null)
            {
                SceneManager.LoadNextScene();
            }
            continueButton.interactable = false;
        }
    }

    IEnumerator TypewriterEffect(string fullText)
    {
        isTyping = true;
        storyText.text = "";

        foreach (char letter in fullText.ToCharArray())
        {
            storyText.text += letter;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        isTyping = false;
    }

    void SkipTypewriter()
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
        }

        storyText.text = currentFullText;
        isTyping = false;
    }

    void HandleTag(string tag)
    {
        Debug.Log("处理标签:" + tag);
        string[] parts = tag.Split(' ');
        string command = parts[0];

        // 1. 处理立绘切换指令: # sprite 角色名 贴图tag
        if (parts.Length == 3 && command == "sprite")
        {
            string characterName = parts[1];
            string spriteTag = parts[2];
            Debug.Log($"换立绘:{characterName} → {spriteTag}");
            characterManager.ChangeSprite(characterName, spriteTag);
        }
        // 2. 处理隐藏角色指令: # hide 角色名
        else if (parts.Length == 2 && command == "hide")
        {
            string characterName = parts[1];
            Debug.Log($"隐藏角色: {characterName}");
            characterManager.HideCharacter(characterName);
        }
        // 3. 处理显示角色指令: # show 角色名
        else if (parts.Length == 2 && command == "show")
        {
            string characterName = parts[1];
            Debug.Log($"显示角色: {characterName}");
            characterManager.ShowCharacter(characterName);
        }
        // 🟢 4. 处理教程事件: # event 事件名称
        else if (parts.Length == 2 && command == "event")
        {
            string eventName = parts[1];
            if (tutorialManager != null)
            {
                tutorialManager.TriggerEvent(eventName);
            }
            else
            {
                Debug.LogWarning("TutorialEventManager 未设置！");
            }
        }
        // 🟢 5. 等待教程事件完成: # wait_event 事件名称
        else if (parts.Length == 2 && command == "wait_event")
        {
            string eventName = parts[1];
            if (tutorialManager != null)
            {
                isWaitingForTutorialCompletion = true;
                tutorialManager.StartWaitingForEvent(eventName);
                Debug.Log($"Ink暂停，等待完成: {eventName}");
            }
            else
            {
                Debug.LogWarning("TutorialEventManager 未设置！");
            }
        }
        // 🟢 6. 快捷指令: 激活对象 # activate 对象名
        else if (parts.Length == 2 && command == "activate")
        {
            string objectName = parts[1];
            if (tutorialManager != null)
            {
                tutorialManager.ActivateObjectByName(objectName);
            }
        }
        // 🟢 6. 快捷指令: 停用对象 # deactivate 对象名
        else if (parts.Length == 2 && command == "deactivate")
        {
            string objectName = parts[1];
            if (tutorialManager != null)
            {
                tutorialManager.DeactivateObjectByName(objectName);
            }
        }
        // 🟢 7. 快捷指令: 隐藏箭头 # hide_arrow
        else if (parts.Length == 1 && command == "hide_arrow")
        {
            if (tutorialManager != null)
            {
                tutorialManager.HideArrow();
            }
        }
        // 🟢 8. 快捷指令: 停止所有高亮 # stop_highlights
        else if (parts.Length == 1 && command == "stop_highlights")
        {
            if (tutorialManager != null)
            {
                tutorialManager.StopAllHighlights();
            }
        }
    }

    /// <summary>
    /// 🟢 教程事件完成回调
    /// </summary>
    void OnTutorialEventCompleted(string eventName)
    {
        Debug.Log($"收到完成通知: {eventName}");
        isWaitingForTutorialCompletion = false;

        // 自动播放下一行
        ShowNextLine();
    }

    void OnDestroy()
    {
        // 🟢 清理回调
        if (tutorialManager != null)
        {
            tutorialManager.OnEventCompleted -= OnTutorialEventCompleted;
        }
    }
}