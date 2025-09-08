using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FailOnClick : MonoBehaviour
{
    public string failSceneName = "Fail";  

    Button btn;

    void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClicked);
    }

    void OnClicked()
    {
        SceneManager.LoadScene(failSceneName);
    }
}
