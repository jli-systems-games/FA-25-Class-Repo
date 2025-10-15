using UnityEngine;

public class ArcadeCarController : MonoBehaviour
{
    public float moveSpeed = 20f;     // 전진 속도
    public float turnSpeed = 120f;    // 회전 속도(도/초)
    public float damping = 5f;        // 감속(손 뗐을 때 서서히 감속)

    float currentSpeed;

    void Update()
    {
        // 입력 (간단히 기존 Horizontal/Vertical 사용)
        float steer = Input.GetAxisRaw("Horizontal"); // A/D, ←/→
        float throttle = Input.GetAxisRaw("Vertical"); // W/S, ↑/↓

        // 회전
        transform.Rotate(0f, steer * turnSpeed * Time.deltaTime, 0f);

        // 속도 업데이트
        float target = throttle * moveSpeed;
        currentSpeed = Mathf.MoveTowards(currentSpeed, target, damping * Time.deltaTime);

        // 전진 이동(중력/충돌 고려 X)
        transform.position += transform.forward * currentSpeed * Time.deltaTime;
    }
}