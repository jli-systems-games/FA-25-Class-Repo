using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameStat gameStats; 

    void Start()
    {
        gameStats.ResetStats();
    }
}
