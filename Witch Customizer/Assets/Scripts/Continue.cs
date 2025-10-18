using UnityEngine;
using UnityEngine.SceneManagement;

public class Continue : MonoBehaviour
{
    public void OnContinueButtonPressed()
    {
        SceneManager.LoadScene("Customizing");
    }
}
