using UnityEngine;
using UnityEngine.SceneManagement;
using PixelCrushers.DialogueSystem;

public class GlobalRestart : MonoBehaviour
{
    public static GlobalRestart Instance;
    
    public KeyCode restartKey = KeyCode.P;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            Lua.RegisterFunction("TriggerRestart", this, typeof(GlobalRestart).GetMethod("RestartGame"));
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(restartKey))
        {
            RestartGame();
        }
    }
    
    public void RestartGame()
    {
        if (DialogueManager.isConversationActive)
        {
            DialogueManager.StopConversation();
        }
        
        CoreData.Reset();
        SceneManager.LoadScene(CoreData.setupSceneName);
    }
}