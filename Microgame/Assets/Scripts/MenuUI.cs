using UnityEngine;

public class MenuUI : MonoBehaviour
{
    
    public void OnPlayButton()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame(); 
        }
        else
        {
            Debug.LogError("GameManager instance not found in the scene!");
        }
    }
}
