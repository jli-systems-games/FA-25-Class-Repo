using UnityEngine;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{

    public void GoToPlayScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
