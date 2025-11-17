using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;

    public int p1Index;   // Player 1 selected blade index
    public int p2Index;   // Player 2 selected blade index

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
