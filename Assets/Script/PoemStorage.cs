using UnityEngine;

public class PoemDataStore : MonoBehaviour
{
    public static PoemDataStore Instance { get; private set; }

    public PoemData Current; 

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // newpome
    public void StartNewPoem(int totalLines)
    {
        Current = ScriptableObject.CreateInstance<PoemData>();
        Current.totalLines = totalLines;
      
    }

    public void ReloadPoem(int totalLines)
    {
        if (Current == null) StartNewPoem(totalLines);
        else Current.totalLines = totalLines;
    }
}