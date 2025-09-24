using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwicther : MonoBehaviour
{
    public void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void Play711(AudioSource audio)
    {
        audio.Play();
    }
}
