using UnityEngine;
using System.Collections.Generic;

public class RuntimePlayersRegistry : MonoBehaviour
{
    public static RuntimePlayersRegistry Instance { get; private set; }
    public List<PlayerData> players = new();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}