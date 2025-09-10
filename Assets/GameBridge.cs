using System;
using UnityEngine;

public class GameBridge : MonoBehaviour
{
    public static event Action<bool> OnGameEnd;

    public static void EndGame(bool success)
    {
        OnGameEnd?.Invoke(success);
    }
}
