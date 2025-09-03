using UnityEngine;

[DisallowMultipleComponent]
public class PaddlePaint : MonoBehaviour
{
    [Tooltip("0 = Day（日），1 = Night（月）")]
    public int teamId = 0;

    [Header("可选：视觉颜色")]
    public Color teamBallColor = new(1f, 0.85f, 0.5f);
    public Color teamTrailColor = new(1f, 0.85f, 0.5f);

    // 被球调用：把球切到本阵营，并更新外观/尾巴（可选）
    public void PaintBall(GameObject ballGO)
    {
        // 地垫法：同步阵营给轨迹邮差
        var trail = ballGO.GetComponent<BallTrailPainter3D>();
        if (trail) trail.SetTeam(teamId);

        // 若你的球脚本里有 owner 字段，可以同步一下（可选）
        var pong = ballGO.GetComponent<BallPong3D>();
        if (pong) { /* 如果你有 pong.owner = teamId; 就在这里设 */ }

        // 可选：更新球颜色
        var r = ballGO.GetComponentInChildren<Renderer>();
        if (r)
        {
            var mpb = new MaterialPropertyBlock();
            r.GetPropertyBlock(mpb);
            if (r.sharedMaterial && r.sharedMaterial.HasProperty("_BaseColor"))
                mpb.SetColor("_BaseColor", teamBallColor);
            else mpb.SetColor("_Color", teamBallColor);
            r.SetPropertyBlock(mpb);
        }

        var skin = ballGO.GetComponent<BallTeamSkin>();
        if (skin)
        {
            skin.SetTeam(teamId);
        }
    }
}