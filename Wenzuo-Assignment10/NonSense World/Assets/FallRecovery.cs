using UnityEngine;

/// <summary>
/// 玩家跌落到阈值以下时，瞬移回最近的可走板中心
/// </summary>
public class FallRecovery : MonoBehaviour
{
    public Transform cityRoot;         // 指向有 WalkablePlates 的根（WorldRoot）
    public float fallY = -5f;          // 跌落阈值
    public float eyeLift = 1.2f;       // 回城时抬高一点

    void Update()
    {
        if (transform.position.y > fallY) return;
        if (!cityRoot) cityRoot = GameObject.Find("WalkablePlates")?.transform ?? GameObject.Find("WorldRoot")?.transform;
        if (!cityRoot) return;

        Transform best = null;
        float bestD = float.MaxValue;
        foreach (Transform t in cityRoot.GetComponentsInChildren<Transform>())
        {
            if (!t.name.StartsWith("Plate_")) continue;
            float d = (t.position - transform.position).sqrMagnitude;
            if (d < bestD) { bestD = d; best = t; }
        }
        if (best)
        {
            var pos = best.position + Vector3.up * eyeLift;
            var cc = GetComponent<CharacterController>();
            bool re = false; if (cc && cc.enabled) { cc.enabled = false; re = true; }
            transform.position = pos;
            if (re) cc.enabled = true;
        }
    }
}
