using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleSceneManager : MonoBehaviour
{
    [Header("自定义设置")]
    public KeyCode switchKey = KeyCode.Space;   // 触发键
    public string targetSceneName;              // 目标场景名字

    void Update()
    {
        if (Input.GetKeyDown(switchKey))
        {
            if (!string.IsNullOrEmpty(targetSceneName))
            {
                SceneManager.LoadScene(targetSceneName);
            }
            else
            {
                Debug.LogWarning("请在 Inspector 里指定目标场景 Name 或 Index。");
            }
        }
    }
}