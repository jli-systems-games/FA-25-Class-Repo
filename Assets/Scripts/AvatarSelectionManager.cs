using UnityEngine;
using UnityEngine.SceneManagement;

public class AvatarSelectionManager : MonoBehaviour
{
    public static int player1Avatar = -1;
    public static int player2Avatar = -1;

    private int clickCount = 0;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void OnAvatarClicked(AvatarSelection avatar)
    {
        if (clickCount == 0)
        {
            player1Avatar = avatar.avatarID;
            avatar.Highlight();
            clickCount++;
            Debug.Log("Player 1 chose avatar: " + player1Avatar);
        }
        else if (clickCount == 1)
        {
            if (avatar.avatarID == player1Avatar)
                return;

            player2Avatar = avatar.avatarID;
            avatar.Highlight();
            clickCount++;

            Debug.Log("Player 2 chose avatar: " + player2Avatar);

            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        // Replace "MainGameSceneName" with your actual scene name
        SceneManager.LoadScene("Main");
    }
}
