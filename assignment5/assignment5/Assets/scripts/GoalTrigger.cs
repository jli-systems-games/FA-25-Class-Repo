using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    [Header("拖入 GameFlow（推荐）")]
    public GameFlow flow;

    public LayerMask allowedLayers;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (allowedLayers.value != 0 && (allowedLayers.value & (1 << other.gameObject.layer)) == 0) return;

        var f = flow ? flow : GameFlow.I;
        if (!f) f = FindObjectOfType<GameFlow>(true); // 兜底（includeInactive）

        if (f) f.Win();
    }
}
