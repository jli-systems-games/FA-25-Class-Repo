using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DebugCargo : MonoBehaviour
{
    public Color gizmoColor = new Color(0.2f, 1f, 0.2f, 0.6f);
    private Collider2D col;
    private Train2DController_Pos train;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        if (!col) Debug.LogError("[DebugCargo] No Collider2D.");
        train = FindObjectOfType<Train2DController_Pos>();

        Debug.Log($"[DebugCargo] Awake on {name}. "
        + $"isTrigger={col.isTrigger}, colliderType={col.GetType().Name}, "
        + $"layer={LayerMask.LayerToName(gameObject.layer)}");
    }

    void OnEnable()
    {
        Debug.Log($"[DebugCargo] Enabled {name} at {transform.position}");
    }

    void Start()
    {
        if (!col.isTrigger)
            Debug.LogWarning($"[DebugCargo] {name} collider is NOT trigger. Set isTrigger=true.");
        if (!train)
            Debug.LogWarning($"[DebugCargo] No Train2DController_Pos found in scene.");
    }

    void Update()
    {
        if (!train) return;

        float d = Vector2.Distance(transform.position, train.transform.position);
        if (d < 0.5f) Debug.Log($"[DebugCargo] Train very close ({d:0.00}).");
        else if (d < 1.0f) Debug.Log($"[DebugCargo] Train near ({d:0.00}).");
    }

    void FixedUpdate()
    {
        if (!train) return;

        // 主动做一次物理检测，看看火车的碰撞体是否覆盖到货物
        var hits = Physics2D.OverlapCircleAll(transform.position, GuessRadius(), ~0);
        bool foundTrain = false;
        foreach (var h in hits)
        {
            if (!h) continue;
            if (h.GetComponentInParent<Train2DController_Pos>())
            {
                foundTrain = true;
                break;
            }
        }
        if (foundTrain)
            Debug.Log($"[DebugCargo] OverlapCircle sees TRAIN at {Time.time:0.00}s.");
    }

    // 触发事件直接告诉我们是否进来了
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[DebugCargo] OnTriggerEnter2D by {other.name} (layer={LayerMask.LayerToName(other.gameObject.layer)})");
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // 有些项目设置/帧率下 Enter 可能被忽略（极罕见），Stay 更保险
        if (other.GetComponentInParent<Train2DController_Pos>())
            Debug.Log($"[DebugCargo] OnTriggerStay2D with TRAIN at {Time.frameCount}.");
    }

    float GuessRadius()
    {
        // 估算当前触发范围用于 OverlapCircle 可视化
        float r = 0.3f;
        if (col is CircleCollider2D cc)
            r = cc.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
        else if (col is BoxCollider2D bc)
            r = Mathf.Max(bc.size.x, bc.size.y) * 0.5f * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
        return r;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        var c = Gizmos.color;
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, 0.02f); // center
        // 画出近似触发半径，方便看“货物是否在轨道线上”
        float r = 0.3f;
        var col = GetComponent<Collider2D>();
        if (col is CircleCollider2D cc)
            r = cc.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
        else if (col is BoxCollider2D bc)
            r = Mathf.Max(bc.size.x, bc.size.y) * 0.5f * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);

        UnityEditor.Handles.color = gizmoColor;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, r);
        Gizmos.color = c;
    }
#endif
}
