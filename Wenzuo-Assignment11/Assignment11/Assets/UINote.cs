using UnityEngine;
using UnityEngine.SceneManagement;

public class UINoteToggle : MonoBehaviour
{
    public GameObject noteRoot;   // ÍÏ NotePanel ½øÀ´

    void Start()
    {
        if (noteRoot != null)
            noteRoot.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (noteRoot != null)
                noteRoot.SetActive(!noteRoot.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.buildIndex);
        }
    }
}
