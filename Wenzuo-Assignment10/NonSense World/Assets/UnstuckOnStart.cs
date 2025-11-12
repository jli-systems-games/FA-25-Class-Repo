using UnityEngine;

[DefaultExecutionOrder(60)]
public class UnstuckOnStart : MonoBehaviour
{
    public float duration = 1.0f;   // 开场前1秒内持续自救
    public float nudgeUp = 0.15f;   // 每帧向上微移
    public float nudgeSide = 0.05f; // 侧向微移强度

    CharacterController cc;
    float timer;

    void Awake() { cc = GetComponent<CharacterController>(); }

    void Update()
    {
        if (timer > duration) return;
        timer += Time.deltaTime;

        // 用 OverlapCapsule 判断是否和场景重叠
        if (cc && IsOverlapping(out Vector3 push))
        {
            bool reenable = false;
            if (cc.enabled) { cc.enabled = false; reenable = true; }

            // 往上+侧向轻推，直到脱离
            transform.position += Vector3.up * nudgeUp + push * nudgeSide;

            if (reenable) cc.enabled = true;
        }
    }

    bool IsOverlapping(out Vector3 outward)
    {
        outward = Vector3.zero;
        float r = cc ? cc.radius + 0.02f : 0.4f;
        float h = cc ? Mathf.Max(cc.height - 2f * cc.radius, 0.01f) : 1.2f;
        Vector3 baseP = transform.position + Vector3.up * r;
        Vector3 topP = baseP + Vector3.up * h;

        var hits = Physics.OverlapCapsule(baseP, topP, r, ~0, QueryTriggerInteraction.Ignore);
        foreach (var c in hits)
        {
            if (c.transform == transform) continue;
            // 计算一个大概的“外推方向”（从碰到的物体指向玩家的反向）
            Vector3 dir = (transform.position - c.bounds.ClosestPoint(transform.position));
            outward += dir.normalized;
        }
        if (hits.Length > 0) outward = outward.normalized;
        return hits.Length > 0;
    }
}
