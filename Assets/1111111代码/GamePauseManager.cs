using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GamePauseManager : MonoBehaviour 
{
    [Header("暂停设置")]
    [SerializeField] private float delayTime = 4f;
    
    [Header("UI设置")]
    [SerializeField] private Button resumeButton;

    void Start() 
    {
        // 启动延迟暂停
        StartCoroutine(PauseAfterDelay());
        
        // 绑定按钮事件
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
        }
        else
        {
            Debug.LogWarning("Resume Button未分配！请在Inspector中分配按钮。");
        }
    }

    IEnumerator PauseAfterDelay() 
    {
        yield return new WaitForSeconds(delayTime);
        Time.timeScale = 0f;
        Debug.Log("游戏已暂停");
    }

    public void ResumeGame() 
    {
        Time.timeScale = 1f;
        Debug.Log("游戏已恢复");
    }

    void OnDestroy()
    {
        // 清理按钮事件监听
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveListener(ResumeGame);
        }
    }
}
