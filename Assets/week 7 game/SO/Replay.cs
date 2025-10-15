using UnityEngine;
using UnityEngine.SceneManagement;

public class Replay : MonoBehaviour
{
    public void LoadCustomizeCarScene()
    {
        SceneManager.LoadScene("CustomizeCar");
    }
}