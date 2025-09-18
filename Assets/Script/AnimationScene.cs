using UnityEngine;
using UnityEngine.SceneManagement; 

public class AnimationEventSceneChange : MonoBehaviour
{
  
    public void ChangeScene()
    {
        
        SceneManager.LoadScene("Game");
    }


}