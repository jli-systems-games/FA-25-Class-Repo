using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform arenaCenter;   // 拖你的圆盘中心（ArenaCenter / Disk）
    public float diskRadius = 5f;   // 圆盘半径，按你实际大小调
    public int ballCount = 10;
    public float spawnHeight = 1f;
    public float initialForce = 15f; // 初始推一把

    void Start()
    {
        for (int i = 0; i < ballCount; i++)
        {
            // 在圆形范围内随机一个点
            Vector2 pos2D = Random.insideUnitCircle * diskRadius;
            Vector3 spawnPos = arenaCenter.position + new Vector3(pos2D.x, spawnHeight, pos2D.y);

            GameObject ball = Instantiate(ballPrefab, spawnPos, Quaternion.identity);

            // 随机颜色
            Renderer r = ball.GetComponent<Renderer>();
            if (r != null)
            {
                Material m = new Material(r.sharedMaterial);
                m.color = Random.ColorHSV();
                r.material = m;
            }

            // 传入圆盘中心给 BallMove
            BallMove mover = ball.GetComponent<BallMove>();
            if (mover != null)
            {
                mover.arenaCenter = arenaCenter.position;
            }

            // 初始推一把，让它们立刻动起来
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = new Vector3(
                    Random.Range(-1f, 1f),
                    0f,
                    Random.Range(-1f, 1f)
                ).normalized;

                rb.AddForce(dir * initialForce, ForceMode.Impulse);
            }
        }
    }
}
