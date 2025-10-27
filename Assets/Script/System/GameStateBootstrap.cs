using UnityEngine;

public static class GameStateBootstrap
{
    static void EnsureGameState()
    {
        if (GameState.Instance != null) return;

        var prefab = Resources.Load<GameObject>("GameStateRoot");
        if (prefab == null)
        {
            return;
        }

        Object.Instantiate(prefab);
    }
}