using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    // Call this method from the UI Button OnClick event
    public void OnStartButtonPressed()
    {
        // Replace "GameScene" with your actual scene name
        SceneManager.LoadScene("PickBeyblade");
    }
}