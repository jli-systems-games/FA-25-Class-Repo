using TMPro;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    public TMP_Text messageText;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        if (GameManager.Instance.isClipFullfilled)
        {
            messageText.text = "You've unlocked every sound I've made.\nReturn to the main page to listen to the full piece.";
        }
        else
        {
            messageText.text = "";
        }
    }
}
