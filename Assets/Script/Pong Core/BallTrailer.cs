using UnityEngine;

/// 把球在 XZ 平面的轨迹喂给 PaintManager3D（仅当被染色时）。
public class BallTrailPainter3D : MonoBehaviour
{
    public PaintManager3D paint;
    public float brushRadiusWorld = 0.25f;

    [Tooltip("0=日, 1=月, -1=未染；若你的球脚本有 owner 字段，可在别的脚本里同步给这里")]
    public int teamId = -1;

    Vector3 lastPos;

    void Start() { lastPos = Flatten(transform.position); }

    void FixedUpdate()
    {
        Vector3 now = Flatten(transform.position);
        if (teamId >= 0 && paint != null)
            paint.StampLineXZ(lastPos, now, brushRadiusWorld, teamId);
        lastPos = now;
    }

    static Vector3 Flatten(Vector3 p) { p.y = 0f; return p; }

    // 提供给外部调用（比如球拍命中时切换阵营）
    public void SetTeam(int team) { teamId = team; }
}