using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GoToMini2 : MonoBehaviour
{
    public string nextScene = "mini2";
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
