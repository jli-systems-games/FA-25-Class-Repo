using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIButtonSceneLoader : MonoBehaviour
{
    [Header("UI 按钮")]
    public Button buttonA;  // 在 Inspector 拖拽 UI Button

    private void Start()
    {
        if (buttonA != null)
        {
            buttonA.onClick.AddListener(OnButtonAClick);
        }
    }

    void OnButtonAClick()
    {
        StartCoroutine(LoadSceneWithDelay());
    }

    IEnumerator LoadSceneWithDelay()
    {
        yield return new WaitForSeconds(1f); // 等 1 秒钟
        SceneManager.LoadScene("0");         // 切换到名为 "0" 的场景
    }
}
