using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadWithSpace : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("game");
        }
    }
}
