using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    [Header("SceneÕâ¸öÌø×ª")]
    public string sceneName;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonClicked);
    }

    void OnButtonClicked()
    {
        SceneManager.LoadScene(sceneName);
    }
}
