using UnityEngine;

public class PongBasic : MonoBehaviour

    //bilibili���Ĵ��룬�Բ�������д��
    //һ��ע�ͽ�����Ϊ���Ժ����زĸ��õ�ʱ���ܿ����Ǻ�
{
    [Header("Scene Refs")]
    public Transform leftPaddle;     // �󵲰�
    public Transform rightPaddle;    // �ҵ���
    public Transform ball;           // ��

    [Header("Paddle")]
    public float paddleSpeed = 10f;      // �󵲰��ƶ��ٶ�
    public float paddleLimitPadding = 0.3f;

    [Header("Ball")]
    public float ballSpeed = 8f;         // ����֮�ٶ�
    public float ballSpeedUpOnHit = 0.5f;// ���к����
    public float maxBallSpeed = 16f;     // �������ޡ��б�Ҫ�ӣ��о����Ǻܣ������ǲ���ɾ��������
    [Range(0f, 1f)]
    public float paddleInfluence = 0.35f;// ���ȥ��Ӱ��Ƕ�

    [Header("Control")]
    public KeyCode leftUp = KeyCode.W;
    public KeyCode leftDown = KeyCode.S;
    public KeyCode rightUp = KeyCode.UpArrow;
    public KeyCode rightDown = KeyCode.DownArrow;

    private Vector2 ballDir;
    private float camTop, camBottom, camLeft, camRight;

    void Start()
    {
        CacheCameraBounds();
        ResetBall(Random.value > 0.5f);
    }

    void Update()
    {
        CacheCameraBounds();

        // ����
        MovePaddle(leftPaddle, (Input.GetKey(leftUp) ? 1f : 0f) + (Input.GetKey(leftDown) ? -1f : 0f));
        MovePaddle(rightPaddle, (Input.GetKey(rightUp) ? 1f : 0f) + (Input.GetKey(rightDown) ? -1f : 0f));

        // �ƶ���
        ball.position += (Vector3)(ballDir * ballSpeed * Time.deltaTime);

        var ballBounds = GetBounds(ball);

        // ���·���
        if (ballBounds.max.y >= camTop || ballBounds.min.y <= camBottom)
        {
            ballDir.y = -ballDir.y;
            ball.position = new Vector3(
                ball.position.x,
                Mathf.Clamp(ball.position.y, camBottom + ballBounds.extents.y, camTop - ballBounds.extents.y),
                ball.position.z
            );
        }

        // ײ����
        TryPaddleHit(leftPaddle, true);
        TryPaddleHit(rightPaddle, false);

        // ����ɳ���Ļ���ң�������������߼�������λ�ƻ�ȥ������ɾ������������
        if (ballBounds.min.x > camRight || ballBounds.max.x < camLeft)
        {
            ResetBall(startToRight: Random.value > 0.5f);
        }
    }

    void MovePaddle(Transform paddle, float input)
    {
        if (Mathf.Approximately(input, 0f)) return;

        var p = paddle.position;
        p.y += input * paddleSpeed * Time.deltaTime;

        var pb = GetBounds(paddle);
        float minY = camBottom + pb.extents.y + paddleLimitPadding;
        float maxY = camTop - pb.extents.y - paddleLimitPadding;
        p.y = Mathf.Clamp(p.y, minY, maxY);

        paddle.position = p;
    }

    void TryPaddleHit(Transform paddle, bool isLeft)
    {
        var pb = GetBounds(paddle);
        var bb = GetBounds(ball);

        if (!pb.Intersects(bb)) return;

        bool validSide = isLeft ? (bb.min.x <= pb.max.x && ballDir.x < 0f)
                                : (bb.max.x >= pb.min.x && ballDir.x > 0f);
        if (!validSide) return;

        ballDir.x = -ballDir.x;

        float offset = Mathf.InverseLerp(paddle.position.y - pb.extents.y, paddle.position.y + pb.extents.y, ball.position.y);
        float centeredOffset = (offset - 0.5f) * 2f;
        ballDir.y += centeredOffset * paddleInfluence;

        ballDir = ballDir.normalized;

        float push = isLeft ? (pb.max.x - bb.min.x) : (bb.max.x - pb.min.x);
        ball.position += new Vector3(isLeft ? push : -push, 0f, 0f) * 1.05f;

        ballSpeed = Mathf.Min(ballSpeed + ballSpeedUpOnHit, maxBallSpeed);
    }

    void ResetBall(bool startToRight)
    {
        ball.position = Vector3.zero;
        float angle = Random.Range(-0.35f, 0.35f);
        ballDir = new Vector2(startToRight ? 1f : -1f, angle).normalized;
        ballSpeed = 8f;
    }

    void CacheCameraBounds()
    {
        var cam = Camera.main;
        float top = cam.orthographicSize;
        float bottom = -top;
        float right = top * cam.aspect;
        float left = -right;
        camTop = top; camBottom = bottom; camLeft = left; camRight = right;
    }

    Bounds GetBounds(Transform t)
    {
        var col = t.GetComponent<Collider2D>();
        if (col != null) return col.bounds;
        var sr = t.GetComponent<SpriteRenderer>();
        if (sr != null) return sr.bounds;
        Vector3 size = Vector3.one;
        size.Scale(t.localScale);
        return new Bounds(t.position, size);
    }
}
