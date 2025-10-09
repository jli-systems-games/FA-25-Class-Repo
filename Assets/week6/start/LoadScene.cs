using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public string sceneName = "train";   
    public KeyCode key = KeyCode.Space;  

    bool loading;

    void Update()
    {
        if (loading) return;
        if (Input.GetKeyDown(key))
        {
            loading = true;

            // 혹시 이전에 멈춰놨다면 풀기
            if (Time.timeScale != 1f) Time.timeScale = 1f;
            AudioListener.pause = false;

            SceneManager.LoadScene(sceneName);
        }
    }
}