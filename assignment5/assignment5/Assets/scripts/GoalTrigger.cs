using UnityEngine;
#if UNITY_2020_1_OR_NEWER
using System.Linq;
#endif

public class GoalTrigger : MonoBehaviour
{
    [Header("")]
    public GameFlow flow; 
    public LayerMask allowedLayers;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (allowedLayers.value != 0 && (allowedLayers.value & (1 << other.gameObject.layer)) == 0) return;

        var f = flow;

        if (!f)
        {
            GameBootstrap.EnsureGameFlow();
            f = GameFlow.I;
        }

#if UNITY_2020_1_OR_NEWER
        if (!f) f = Object.FindObjectOfType<GameFlow>(true);
#else
        if (!f) f = Resources.FindObjectsOfTypeAll<GameFlow>()
                             .FirstOrDefault(g => g != null && g.gameObject.scene.IsValid());
#endif
        if (!f) { Debug.LogError("[GoalTrigger] GameFlow missing."); return; }

        f.Win();
    }
}
