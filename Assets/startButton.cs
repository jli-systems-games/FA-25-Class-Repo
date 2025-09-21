using UnityEngine;
using UnityEngine.SceneManagement;

public class startButton : MonoBehaviour
{
    public void LoadScene01()
    {
        SceneManager.LoadScene("Scene01");
    }
}