using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SuccessOnClick : MonoBehaviour
{
    public string nextScene = "popcornLove";
    Button btn;

    void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClicked);
    }

    void OnClicked()
    {
        SceneManager.LoadScene(nextScene);
    }
}