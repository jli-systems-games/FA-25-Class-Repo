using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void GoToChooseScene()
    {
        SceneManager.LoadScene("NameScene");
    }
}