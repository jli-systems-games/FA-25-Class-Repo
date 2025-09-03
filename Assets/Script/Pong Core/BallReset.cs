using UnityEngine;

/// 挂在“球”上：若离开指定范围 Collider（XZ），传送回 (0,0,0)
[RequireComponent(typeof(Rigidbody))]
public class BallResetZone : MonoBehaviour
{
    [Tooltip("限定范围的 Collider（推荐 BoxCollider 或 SphereCollider），必须勾 isTrigger")]
    public Collider boundary;

    [Tooltip("超出范围时重置到的坐标（默认为原点）")]
    public Vector3 resetPosition = Vector3.zero;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (boundary && !boundary.isTrigger)
        {
            Debug.LogWarning("[BallResetZone] boundary 必须勾选 isTrigger。已自动设置。");
            boundary.isTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // 如果是自己离开 boundary，就重置位置
        if (other == boundary)
        {
            ResetBall();
        }
    }

    void ResetBall()
    {
        // 位置 & 速度
        rb.position = resetPosition;
        rb.linearVelocity = Vector3.zero; // 注意 Rigidbody 用 velocity，不是 linearVelocity

        // 清空颜色 / 阵营
        var skin = GetComponent<BallTeamSkin>();
        if (skin)
        {
            skin.SetTeam(-1); // -1 = 未染色，会把材质槽还原成默认
        }

        var trailPainter = GetComponent<BallTrailPainter3D>();
        if (trailPainter)
        {
            trailPainter.SetTeam(-1); // 不再往地面涂色
        }

        // 重新发球
        var ball = GetComponent<BallPong3D>();
        if (ball)
        {
            ball.Serve(Random.value < 0.5f ? -1 : 1);
        }
    }
}