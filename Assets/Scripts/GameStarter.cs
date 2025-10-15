using UnityEngine;
using PixelCrushers.DialogueSystem;

public class GameStarter : MonoBehaviour
{
    [Header("Settings")]
    public string startConversation = "Main Conversation";
    public bool autoStart = true;
    
    void Start()
    {

        if (DialogueManager.isConversationActive)
        {
            DialogueManager.StopConversation();
        }
        
        if (autoStart)
        {

            Invoke("StartGame", 0.1f);
        }
    }
    
    public void StartGame()
    {

        if (DialogueManager.isConversationActive)
        {
            Debug.LogWarning("对话仍在运行，停止后重新开始");
            DialogueManager.StopConversation();
        }
        
        DialogueManager.StartConversation(startConversation);
    }
}