using UnityEngine;

/// 把球在 XZ 平面的轨迹喂给 PaintManager3D（仅当被染色时）。
public class BallTrailPainter3D : MonoBehaviour
{
    [Header("引用")]
    public PaintManager3D paint;                 // 地板上的 PaintManager3D
    [Tooltip("找不到 paint 时，按这个 Tag 在场景里自动寻找")]
    public string paintManagerTag = "Ground";    // 可改成你地板物体的 Tag；留空则不自动找

    [Header("画笔")]
    public float brushRadiusWorld = 0.25f;       // 画笔半径（世界单位）
    [Tooltip("从球的 SphereCollider 推导画笔半径（启动时运行一次）")]
    public bool autoRadiusFromSphere = true;

    [Header("阵营/调试")]
    [Tooltip("0=日, 1=月, -1=未染。为 -1 时不画。")]
    public int teamId = -1;
    [Tooltip("调试用：忽略 teamId，始终按 0（日）去画，便于验收可视化")]
    public bool alwaysPaintForDebug = false;

    Vector3 lastPos;

    void Awake()
    {
        // 自动找 PaintManager（可选）
        if (!paint && !string.IsNullOrEmpty(paintManagerTag))
        {
            var go = GameObject.FindGameObjectWithTag(paintManagerTag);
            if (go) paint = go.GetComponent<PaintManager3D>();
        }
    }

    void Start()
    {
        lastPos = Flatten(transform.position);

        // 运行一次：用球半径推导画笔更省心
        if (autoRadiusFromSphere)
        {
            var sphere = GetComponent<SphereCollider>();
            if (sphere)
            {
                // 半径 = 球半径 * 最大轴向缩放
                float scaledRadius = sphere.radius * Mathf.Max(
                    Mathf.Abs(transform.lossyScale.x),
                    Mathf.Abs(transform.lossyScale.y),
                    Mathf.Abs(transform.lossyScale.z)
                );
                // 画笔稍微比球大一点点，避免出现断点
                brushRadiusWorld = Mathf.Max(brushRadiusWorld, scaledRadius * 0.9f);
            }
        }
    }

    void FixedUpdate()
    {
        Vector3 now = Flatten(transform.position);

        // 选择要用的阵营
        int teamToPaint = teamId;
        if (alwaysPaintForDebug) teamToPaint = 0; // 调试强制为日

        if (paint != null && teamToPaint >= 0)
        {
            paint.StampLineXZ(lastPos, now, brushRadiusWorld, teamToPaint);
        }

        lastPos = now;
    }

    static Vector3 Flatten(Vector3 p) { p.y = 0f; return p; }

    // 提供给外部调用（比如球拍命中时切换阵营）
    public void SetTeam(int team) { teamId = team; }
}