using UnityEngine;
using UnityEngine.SceneManagement;

public class ArcadeCarController : MonoBehaviour
{
    [Header("Move (No-Physics)")]
    public float moveSpeed = 20f;   // m/s, SO로 덮어씌워짐
    public float turnSpeed = 120f;  // deg/s, SO로 덮어씌워짐
    public float damping = 5f;    // 감속, SO로 덮어씌워짐

    [Header("SO → 컨트롤러 매핑 스케일")]
    [Tooltip("topSpeed_kmh → m/s 변환 후 곱할 계수 (손맛 조정)")]
    public float speedScaleFromSpec = 0.65f; // 0.5~0.8 추천

    [Header("레이싱 씬에서만 자동 활성 (선택)")]
    public bool enableOnlyInSceneNamed = true;
    public string raceSceneName = "Race";    // 레이싱 씬 이름

    float currentSpeed;

    void Awake()
    {
        // 커스터마이징 씬에서 실수로 켜지지 않도록 가드
        if (enableOnlyInSceneNamed)
        {
            string now = SceneManager.GetActiveScene().name;
            if (!string.Equals(now, raceSceneName))
            {
                enabled = false; // 레이싱 씬이 아니면 비활성
            }
        }
    }

    void Update()
    {
        // 간단 키 입력(새 Input System 안 써도 됨)
        float steer = Input.GetAxisRaw("Horizontal"); // A/D, ←/→
        float throttle = Input.GetAxisRaw("Vertical");   // W/S, ↑/↓

        // 회전
        transform.Rotate(0f, steer * turnSpeed * Time.deltaTime, 0f);

        // 속도 업데이트
        float target = throttle * moveSpeed; // m/s
        currentSpeed = Mathf.MoveTowards(currentSpeed, target, damping * Time.deltaTime);

        // 전진 이동
        transform.position += transform.forward * currentSpeed * Time.deltaTime;
    }

    /// <summary>
    /// 커스텀 SO 스펙을 컨트롤러 파라미터로 주입
    /// </summary>
    public void ApplySpec(CarSpec spec)
    {
        if (!spec) { Debug.LogWarning("[ArcadeCarController] Spec is null"); return; }

        // 최고속도: km/h → m/s 변환 (/3.6) 후 스케일
        float ms = (spec.topSpeed_kmh / 3.6f) * Mathf.Max(0.05f, speedScaleFromSpec);
        moveSpeed = Mathf.Max(5f, ms);

        // 핸들링(1~10) → 회전속도(80~160deg/s 예시)
        float h01 = Mathf.Clamp01(spec.handling_rating / 10f);
        turnSpeed = Mathf.Lerp(80f, 160f, h01);

        // 그립(1~10) → 감속(3~8 예시): 그립 높을수록 더 빨리 속도 맞춤
        float g01 = Mathf.Clamp01(spec.grip_rating / 10f);
        damping = Mathf.Lerp(3f, 8f, g01);

        // 확인용 로그
        Debug.Log($"[ArcadeCarController] ApplySpec -> moveSpeed={moveSpeed:0.0} m/s (~{moveSpeed * 3.6f:0} km/h), turn={turnSpeed:0}, damping={damping:0.0}");
    }
}
