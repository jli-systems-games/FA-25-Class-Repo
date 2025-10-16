using UnityEngine;
using PixelCrushers.DialogueSystem;

public class GameStarter : MonoBehaviour
{
    public string startConversation = "Main Conversation";
    public bool autoStart = true;
    
    void Start()
    {
        CoreData.Initialize();
        
        if (autoStart)
        {
            Invoke("StartGame", 0.1f);
        }
    }
    
    public void StartGame()
    {
        DialogueManager.StartConversation(startConversation);
    }
}