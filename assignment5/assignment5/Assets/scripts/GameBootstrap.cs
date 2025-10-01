using UnityEngine;

[DefaultExecutionOrder(-10000)]  
public class GameBootstrap : MonoBehaviour
{
    public GameFlow gameFlowPrefab;

    void Awake()
    {
        EnsureGameFlow();
    }

    public static void EnsureGameFlow()
    {
        if (GameFlow.I != null) return;

#if UNITY_2020_1_OR_NEWER
        var existing = Object.FindObjectOfType<GameFlow>(true);
#else
        var all = Resources.FindObjectsOfTypeAll<GameFlow>();
        GameFlow existing = null;
        foreach (var g in all) { if (g && g.gameObject.scene.IsValid()) { existing = g; break; } }
#endif
        if (existing)
        {
            GameFlow.I = existing;
            return;
        }

        var bootstrap = FindObjectOfType<GameBootstrap>();
        if (!bootstrap || !bootstrap.gameFlowPrefab)
        {
            Debug.LogError("[GameBootstrap] 请在场景里放一个带 GameBootstrap 的物体，并把 GameFlow 预制体拖进去。");
            return;
        }

        var inst = Instantiate(bootstrap.gameFlowPrefab);
        inst.gameObject.name = "GameSystems (Auto)";
        DontDestroyOnLoad(inst.gameObject);
        GameFlow.I = inst;
        Debug.Log("[GameBootstrap] Spawned GameFlow: " + inst.name);
    }
}
