using UnityEngine;

public static class AutoBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void EnsureGameFlow()
    {
        // 场景里已经有了就不再动
        if (GameFlow.I != null) return;

        // 包含未激活里找一次
#if UNITY_2023_1_OR_NEWER
        var existing = Object.FindFirstObjectByType<GameFlow>(FindObjectsInactive.Include);
#else
        var existing = Object.FindObjectOfType<GameFlow>(true);
#endif
        if (existing)
        {
            GameFlow.I = existing;
            Object.DontDestroyOnLoad(existing.gameObject);
            return;
        }

        // 尝试用 Resources 里的预制体
        var prefab = Resources.Load<GameObject>("GameSystems");
        if (prefab)
        {
            var inst = Object.Instantiate(prefab);
            inst.name = "GameSystems (Auto)";
            Object.DontDestroyOnLoad(inst);
            return;
        }

        // 最后兜底：动态创建一个最小化的 GameSystems（带 Debug HUD）
        var go = new GameObject("GameSystems (Runtime)");
        Object.DontDestroyOnLoad(go);
        go.AddComponent<GameFlow>();            // 会自动创建 UI
        go.AddComponent<DebugHudUI>();          // HUD：F1 显示/隐藏
    }
}
