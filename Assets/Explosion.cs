using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float radius = 12f;           // 作用半径
    public float force = 1200f;          // 推动力（冲量）
    public float upwardModifier = 0f;    // 向上抬升（0~2常用）
    public LayerMask affectedLayers = ~0;// 影响哪些层（默认全选）
    public bool ignoreSelf = true;       // 是否忽略自己
    public KeyCode testKey = KeyCode.E;  // 测试按键（可删）

    Rigidbody selfRb;

    void Awake() { selfRb = GetComponent<Rigidbody>(); }

    void Update()
    {
        if (Input.GetKeyDown(testKey)) Explode();
    }

    public void Explode()
    {
        var cols = Physics.OverlapSphere(transform.position, radius, affectedLayers, QueryTriggerInteraction.Ignore);
        foreach (var c in cols)
        {
            var rb = c.attachedRigidbody;
            if (rb == null) continue;
            if (ignoreSelf && (rb == selfRb || rb.transform.root == transform.root)) continue;

            rb.AddExplosionForce(force, transform.position, radius, upwardModifier, ForceMode.Impulse);
        }
    }

    // 选中物体时显示范围，方便调参
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.15f);
        Gizmos.DrawSphere(transform.position, radius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}