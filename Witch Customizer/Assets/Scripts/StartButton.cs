using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    // Call this when the Start button is clicked
    public void StartGame()
    {
        SceneManager.LoadScene("Introduction"); // change scene name if needed
    }
}
