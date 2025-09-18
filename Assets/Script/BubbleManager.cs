using UnityEngine;
using UnityEngine.SceneManagement;

public class BubbleManager : MonoBehaviour
{
    public static BubbleManager Instance;
    public int totalBubbles = 0;
    private int clearedBubbles = 0;
    public string nextSceneName = "Table";
    
    public GameObject bubblePanel;

    void Awake()
    {
        Instance = this;
        clearedBubbles = 0;
        Debug.Log($"[BubbleManager] Awake, totalBubbles={totalBubbles}, clearedBubbles={clearedBubbles}, nextSceneName={nextSceneName}");
    
    }

    public void RegisterBubble()
    {
    totalBubbles++;
    Debug.Log($"[BubbleManager] RegisterBubble, totalBubbles={totalBubbles}");
    }

    public void BubbleCleared()
    {
        clearedBubbles++;
        Debug.Log($"[BubbleManager] BubbleCleared, clearedBubbles={clearedBubbles}, totalBubbles={totalBubbles}");
        if (clearedBubbles >= totalBubbles)
        {
            Debug.Log($"[BubbleManager] All bubbles cleared! Switching to scene: {nextSceneName}");
            if (bubblePanel) bubblePanel.SetActive(false);
            SceneManager.LoadScene(nextSceneName);
        }
    }

    public void SwitchToScene(string sceneName)
    {
        Debug.Log($"[BubbleManager] SwitchToScene called, sceneName={sceneName}");
        SceneManager.LoadScene(sceneName);
    }
}
