using UnityEngine;
using PixelCrushers.DialogueSystem;

public class GameStarter : MonoBehaviour
{
    [Header("Settings")]
    public string startConversation = "Main Conversation";
    public bool autoStart = true;
    
    void Start()
    {
        if (autoStart)
        {
            StartGame();
        }
    }
    
    public void StartGame()
    {
        ResetGameVariables();
        
        DialogueManager.StartConversation(startConversation);
    }
    
    void ResetGameVariables()
    {
        DialogueLua.SetVariable("Courage", 1);
        DialogueLua.SetVariable("Logic", 1);
        DialogueLua.SetVariable("Empathy", 1);
        DialogueLua.SetVariable("Technical", 1);
        DialogueLua.SetVariable("PointsRemaining", 6);
        DialogueLua.SetVariable("StoryProgress", 0);
        DialogueLua.SetVariable("ChoicePath", "");
        DialogueLua.SetVariable("EndingID", "None");
    }
}