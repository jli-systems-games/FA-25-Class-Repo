using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class GlobalRestart : MonoBehaviour
{
    const string MainMenuScene = "MainMenu";

    void Awake()
    {
        if (FindObjectsByType<GlobalRestart>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.R)) return;

        if (EventSystem.current && EventSystem.current.currentSelectedGameObject &&
            EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>())
            return;

        CentralData.I.ResetAll(keepNames: false);

        SceneManager.LoadScene(MainMenuScene);
    }
}