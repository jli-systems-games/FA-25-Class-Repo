using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    public string sceneName;          // 场景 1
    public Image targetImage;         // 1 sprite
    public Sprite newSprite;          // 2 sprite

    public void OnButtonClick()
    {
        if (targetImage != null && newSprite != null)
        {
            targetImage.sprite = newSprite;
        }

        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}