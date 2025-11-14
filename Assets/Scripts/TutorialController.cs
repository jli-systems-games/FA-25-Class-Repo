using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 教程控制器 - 最简单版本
/// </summary>
public class TutorialController : MonoBehaviour
{
    [Header("教程画面配置")]
    [Tooltip("所有教程画面的Panel对象（按顺序，4个Panel）")]
    public GameObject[] tutorialPages;
    
    [Header("按钮引用")]
    [Tooltip("前进/下一步按钮")]
    public Button nextButton;
    
    [Tooltip("后退/上一步按钮")]
    public Button previousButton;
    
    [Tooltip("返回主菜单按钮（显示在最后一页）")]
    public Button returnButton;
    
    [Header("场景设置")]
    [Tooltip("主菜单场景名称")]
    public string mainMenuSceneName = "MainMenu";
    
    private int currentPageIndex = 0;
    
    void Start()
    {
        // 确保所有Panel都关闭
        for (int i = 0; i < tutorialPages.Length; i++)
        {
            if (tutorialPages[i] != null)
            {
                tutorialPages[i].SetActive(false);
            }
        }
        
        // 激活第一个Panel
        if (tutorialPages.Length > 0 && tutorialPages[0] != null)
        {
            tutorialPages[0].SetActive(true);
        }
        
        // 配置按钮事件
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(NextPage);
        }
        
        if (previousButton != null)
        {
            previousButton.onClick.AddListener(PreviousPage);
        }
        
        if (returnButton != null)
        {
            returnButton.onClick.AddListener(ReturnToMainMenu);
        }
        
        // 更新按钮状态
        UpdateButtonStates();
    }
    
    /// <summary>
    /// 前进到下一页
    /// </summary>
    public void NextPage()
    {
        if (currentPageIndex >= tutorialPages.Length - 1)
            return;
        
        GoToPage(currentPageIndex + 1);
    }
    
    /// <summary>
    /// 后退到上一页
    /// </summary>
    public void PreviousPage()
    {
        if (currentPageIndex <= 0)
            return;
        
        GoToPage(currentPageIndex - 1);
    }
    
    /// <summary>
    /// 跳转到指定页面
    /// </summary>
    public void GoToPage(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= tutorialPages.Length)
            return;
        
        // 关闭当前页面
        if (tutorialPages[currentPageIndex] != null)
        {
            tutorialPages[currentPageIndex].SetActive(false);
        }
        
        // 激活目标页面
        if (tutorialPages[pageIndex] != null)
        {
            tutorialPages[pageIndex].SetActive(true);
        }
        
        // 更新当前页面索引
        currentPageIndex = pageIndex;
        
        // 更新按钮状态
        UpdateButtonStates();
    }
    
    /// <summary>
    /// 返回主菜单
    /// </summary>
    public void ReturnToMainMenu()
    {
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogWarning("主菜单场景名称未设置！");
        }
    }
    
    /// <summary>
    /// 更新按钮的显示状态
    /// </summary>
    void UpdateButtonStates()
    {
        // Previous按钮：第一页时隐藏
        if (previousButton != null)
        {
            previousButton.gameObject.SetActive(currentPageIndex > 0);
        }
        
        // Next按钮：最后一页时隐藏
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(currentPageIndex < tutorialPages.Length - 1);
        }
        
        // Return按钮：最后一页时显示
        if (returnButton != null)
        {
            returnButton.gameObject.SetActive(currentPageIndex == tutorialPages.Length - 1);
        }
    }
    
    /// <summary>
    /// 重置教程到第一页
    /// </summary>
    public void ResetTutorial()
    {
        GoToPage(0);
    }
    
    /// <summary>
    /// 获取当前页面索引
    /// </summary>
    public int GetCurrentPageIndex()
    {
        return currentPageIndex;
    }
    
    /// <summary>
    /// 获取总页面数
    /// </summary>
    public int GetTotalPages()
    {
        return tutorialPages.Length;
    }
}
