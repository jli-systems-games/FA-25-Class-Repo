using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneJump : MonoBehaviour
{
    [Header("目标场景")]
    public string targetScene = "NextScene";

    [Header("UI 按钮")]
    public Button jumpButton;

    void Start()
    {

        if (jumpButton != null)
        {
            jumpButton.onClick.AddListener(JumpToScene);
        }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpToScene();
        }
    }

    void JumpToScene()
    {
        SceneManager.LoadScene(targetScene);
    }
}
