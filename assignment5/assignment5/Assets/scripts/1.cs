using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLog : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.sceneLoaded += (s, m) => Debug.Log($"[Scene] loaded: {s.name}");
        SceneManager.activeSceneChanged += (a, b) => Debug.Log($"[Scene] active: {a.name} -> {b.name}");
    }
}
