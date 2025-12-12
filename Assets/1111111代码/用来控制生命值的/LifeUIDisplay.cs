using UnityEngine;

// 场景中的生命值UI显示器
public class LifeUIDisplay : MonoBehaviour
{
    [Header("UI设置")]
    [Tooltip("7个生命UI对象，按顺序排列")]
    public GameObject[] lifeUIObjects = new GameObject[7];
    
    [Header("调试设置")]
    public bool showDebugInfo = true;
    
    void Start()
    {
        // 根据当前生命值更新UI显示
        UpdateLifeDisplay();
    }
    
    // 更新UI显示
    public void UpdateLifeDisplay()
    {
        int currentLives = LifeManager.Instance.GetCurrentLives();
        
        if (showDebugInfo)
        {
            Debug.Log($"更新生命UI显示，当前生命: {currentLives}");
        }
        
        // 根据剩余生命值显示对应数量的UI
        for (int i = 0; i < lifeUIObjects.Length; i++)
        {
            if (lifeUIObjects[i] != null)
            {
                // 如果索引小于当前生命值，显示UI；否则隐藏
                bool shouldBeActive = i < currentLives;
                lifeUIObjects[i].SetActive(shouldBeActive);
                
                if (showDebugInfo)
                {
                    Debug.Log($"UI[{i}]: {lifeUIObjects[i].name} = {(shouldBeActive ? "显示" : "隐藏")}");
                }
            }
            else
            {
                Debug.LogWarning($"生命UI对象 [{i}] 为空！");
            }
        }
    }
}
