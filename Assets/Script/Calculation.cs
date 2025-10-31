using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    
    public int totalNotes = 0;       
    public int successCount = 0;   
    public int failCount = 0;    
    public UnityEvent<float> onAccuracyChanged; 
    public UnityEvent<int> onSuccessChanged;   
    public UnityEvent<int> onFailChanged;    
 

 
    public void AddSuccess()
    {
        successCount++;
        totalNotes++;
        UpdateAccuracy();
        onSuccessChanged?.Invoke(successCount);
    }

    public void AddFail()
    {
        failCount++;
        totalNotes++;
        UpdateAccuracy();
        onFailChanged?.Invoke(failCount);
    }


    public float GetAccuracy()
    {
        if (totalNotes == 0) return 0f;
        return (float)successCount / totalNotes * 100f;
    }

    
    private void UpdateAccuracy()
    {
        float acc = GetAccuracy();
        onAccuracyChanged?.Invoke(acc);
    }


    public void ResetScore()
    {
        successCount = 0;
        failCount = 0;
        totalNotes = 0;
        UpdateAccuracy();
    }
}