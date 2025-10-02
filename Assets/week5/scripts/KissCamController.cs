using UnityEngine;

public enum KissCamState { Moving, Stopping }

public class KissCamController : MonoBehaviour
{
    [Header("Movement Bounds (World)")]
    public float fixedZ = -108.5f;                      // z 고정
    public Vector2 xRange = new Vector2(52f, 98f);
    public Vector2 yRange = new Vector2(2f, 14f);

    [Header("Stop Checkpoint (kept as-is)")]
    public Vector2 checkpoint = new Vector2(77.6f, 4.89f);
    public float stopDuration = 3f;
    public float reachTolerance = 0.2f;

    [Header("Patrol")]
    public float moveSpeed = 7f;
    public bool randomPatrol = true;

    [Header("UI / SFX (optional)")]
    public UIController ui;
    public SFXVFXManager sfx;

    // =============== Trigger Fail 옵션 ===============
    [Header("Trigger Fail")]
    [Tooltip("허그일 때만 실패 처리")]
    public bool requireHugToFail = true;
    public PlayerStateController player;                // requireHugToFail=true면 연결

    [Tooltip("Trigger에 닿으면 정지할 시간(초)")]
    public float caughtStopDuration = 1.0f;

    [Tooltip("특정 태그의 트리거만 유효하게 하려면 설정(비워두면 모든 트리거 수용)")]
    public string triggerTag = "";                      // 예: "KissCamGate"

    KissCamState state = KissCamState.Moving;
    float stopTimer = 0f;
    Vector3 target;

    // '트리거에 걸린 상태' 플래그
    bool caughtByTrigger = false;

    void Start()
    {
        var p = transform.position;
        transform.position = new Vector3(
            Mathf.Clamp(p.x, xRange.x, xRange.y),
            Mathf.Clamp(p.y, yRange.x, yRange.y),
            fixedZ);

        PickNewTarget();
    }

    void Update()
    {
        // 트리거에 걸린 순간: 1초 정지 후 실패 처리
        if (caughtByTrigger)
        {
            stopTimer -= Time.deltaTime;
            if (stopTimer <= 0f)
            {
                GameManager.I?.GameOver();  // GameManager의 단일 실패 씬으로 전환
            }
            return; // 정지 유지
        }

        // 기존 이동/정지 로직
        if (state == KissCamState.Moving) MoveUpdate();
        else StopUpdate();
    }

    void MoveUpdate()
    {
        Vector2 now2 = new Vector2(transform.position.x, transform.position.y);
        if (Vector2.Distance(now2, checkpoint) <= reachTolerance)
        {
            state = KissCamState.Stopping;
            stopTimer = stopDuration;
            ui?.SetWarningFrame(true);
            sfx?.OnKissCamArriveCheckpoint();
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, target) < 0.05f) PickNewTarget();
    }

    void StopUpdate()
    {
        stopTimer -= Time.deltaTime;
        if (stopTimer <= 0f)
        {
            state = KissCamState.Moving;
            ui?.SetWarningFrame(false);
            sfx?.OnKissCamLeaveCheckpoint();
            PickNewTarget();
        }
    }

    void PickNewTarget()
    {
        target = new Vector3(
            Random.Range(xRange.x, xRange.y),
            Random.Range(yRange.x, yRange.y),
            fixedZ);
    }

    public KissCamState GetState() => state;

    public bool IsAtCheckpoint()
    {
        Vector2 now2 = new Vector2(transform.position.x, transform.position.y);
        return Vector2.Distance(now2, checkpoint) <= reachTolerance;
    }

    // =============== 핵심: 트리거 충돌 처리 ===============
    void OnTriggerEnter(Collider other)
    {
        // 태그 필터(원하면 사용)
        if (!string.IsNullOrEmpty(triggerTag) && !other.CompareTag(triggerTag)) return;

        // 허그 요구 시 상태 확인
        if (requireHugToFail && player != null && !player.IsHug()) return;

        // 중복 방지
        if (caughtByTrigger) return;

        // 1초 정지 + 경고표시 + SFX
        caughtByTrigger = true;
        stopTimer = Mathf.Max(0.01f, caughtStopDuration);

        ui?.SetWarningFrame(true);
        sfx?.OnCaught();

        // 움직임 완전 정지
        // (Rigidbody가 있다면 속도 0; 없다면 Update에서 return으로 고정)
        var rb = GetComponent<Rigidbody>();
        if (rb) { rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }
    }
}
