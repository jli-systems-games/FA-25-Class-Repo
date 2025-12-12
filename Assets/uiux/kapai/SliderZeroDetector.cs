using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Slider归零检测器
/// 当Slider值归零时，等待指定时间后跳转到新场景
/// </summary>
[RequireComponent(typeof(Slider))]
public class SliderZeroDetector : MonoBehaviour
{
    [Header("场景设置")]
    [Tooltip("要跳转的场景名称")]
    public string targetSceneName = "GameOver";

    [Header("时间设置")]
    [Tooltip("归零后等待多少秒跳转场景")]
    public float waitTime = 3f;

    [Tooltip("等待期间是否暂停游戏")]
    public bool pauseGameWhileWaiting = true;

    [Header("物体激活设置")]
    [Tooltip("归零时要启用的物体")]
    public GameObject objectToEnable;

    [Tooltip("是否在归零时启用物体")]
    public bool enableObjectOnZero = true;

    [Header("检测设置")]
    [Tooltip("多小的值算作归零（防止浮点数误差）")]
    public float zeroThreshold = 0.01f;

    [Header("调试")]
    [Tooltip("显示调试信息")]
    public bool showDebugInfo = true;

    private Slider slider;
    private bool hasTriggered = false;
    private bool isWaiting = false;

    void Start()
    {
        slider = GetComponent<Slider>();

        if (slider == null)
        {
            Debug.LogError("未找到Slider组件！");
            return;
        }

        // 监听Slider值变化
        slider.onValueChanged.AddListener(OnSliderValueChanged);

        if (showDebugInfo)
        {
            Debug.Log($"SliderZeroDetector 已初始化，当前值: {slider.value}");
        }
    }

    void Update()
    {
        // 持续检测Slider值（防止没有触发onValueChanged事件）
        if (!hasTriggered && !isWaiting && slider != null)
        {
            CheckIfZero();
        }
    }

    void OnSliderValueChanged(float value)
    {
        if (showDebugInfo)
        {
            Debug.Log($"Slider值变化: {value}");
        }

        CheckIfZero();
    }

    void CheckIfZero()
    {
        if (slider.value <= zeroThreshold && !hasTriggered && !isWaiting)
        {
            if (showDebugInfo)
            {
                Debug.Log($"⚠️ Slider归零！当前值: {slider.value}");
            }

            hasTriggered = true;

            // 启用指定的物体
            if (enableObjectOnZero && objectToEnable != null)
            {
                objectToEnable.SetActive(true);
                if (showDebugInfo)
                {
                    Debug.Log($"✅ 已启用物体: {objectToEnable.name}");
                }
            }
            else if (enableObjectOnZero && objectToEnable == null && showDebugInfo)
            {
                Debug.LogWarning("未设置要启用的物体！");
            }

            StartCoroutine(WaitAndLoadScene());
        }
    }

    IEnumerator WaitAndLoadScene()
    {
        isWaiting = true;

        if (showDebugInfo)
        {
            Debug.Log($"⏳ 等待 {waitTime} 秒后跳转到场景: {targetSceneName}");
        }

        // 如果需要暂停游戏
        if (pauseGameWhileWaiting)
        {
            Time.timeScale = 0f;
            if (showDebugInfo)
            {
                Debug.Log("⏸️ 游戏已暂停");
            }

            // 使用 WaitForSecondsRealtime 来等待（不受 timeScale 影响）
            yield return new WaitForSecondsRealtime(waitTime);

            // 恢复游戏（虽然马上要跳转场景了，但保险起见还是恢复）
            Time.timeScale = 1f;
            if (showDebugInfo)
            {
                Debug.Log("▶️ 游戏已恢复");
            }
        }
        else
        {
            // 不暂停游戏，正常等待
            yield return new WaitForSeconds(waitTime);
        }

        if (showDebugInfo)
        {
            Debug.Log($"🎬 开始加载场景: {targetSceneName}");
        }

        // 检查场景是否存在
        if (Application.CanStreamedLevelBeLoaded(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError($"场景 '{targetSceneName}' 不存在或未添加到Build Settings中！");
        }
    }

    void OnDestroy()
    {
        // 清理监听器
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }

    // 公开方法：手动触发跳转
    public void TriggerSceneTransition()
    {
        if (!hasTriggered && !isWaiting)
        {
            hasTriggered = true;
            StartCoroutine(WaitAndLoadScene());
        }
    }

    // 公开方法：重置检测器
    public void ResetDetector()
    {
        hasTriggered = false;
        isWaiting = false;

        if (showDebugInfo)
        {
            Debug.Log("检测器已重置");
        }
    }
}