using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PeeGameController : MonoBehaviour
{
    [Header("Refs")]
    public PeeToggle pee;               
    public TMP_Text timerText;          

    [Header("Rules")]
    public float requiredSeconds = 10f; 
    public string winScene = "WinScene";
    public string failScene = "FailScene";

    bool inZone = false;    
    float acc;               
    bool ended;

    public void SetInZone(bool v) => inZone = v;

    void Update()
    {
        if (ended) return;
        if (pee == null) return;

   
        if (pee != null && pee.enabled && pee.GetType() != null && peeIsPlaying(pee))
        {
            if (inZone)
            {
                acc += Time.deltaTime;
                if (acc >= requiredSeconds)
                {
                    End(true);
                    return;
                }
            }
            else
            {
                
                End(false);
                return;
            }
        }

        if (timerText)
            timerText.text = $"Inside-Zone Pee: {acc:0.0} / {requiredSeconds:0} s";
    }

  
    bool peeIsPlaying(PeeToggle p)
    {
   
        try
        {
            var prop = p.GetType().GetProperty("IsPlaying");
            if (prop != null) return (bool)prop.GetValue(p);
        }
        catch { }

        try
        {
            var field = p.GetType().GetField("playing",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            if (field != null) return (bool)field.GetValue(p);
        }
        catch { }


        return true;
    }

    void End(bool win)
    {
        ended = true;
    
        if (pee) pee.StopPee();

    
        Invoke(nameof(LoadSceneInternal), 0.2f);
        nextSceneName = win ? winScene : failScene;
    }

    string nextSceneName;
    void LoadSceneInternal()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }
}
