using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpinnerController : MonoBehaviour
{
    [Header("Spin")]
    public float tapTorque = 1.2f;      // 스페이스 1번당 토크
    public bool allowInput = true;      // Stop 후 false

    [Header("Damping on Stop")]
    public float normalAngularDrag = 0.05f; // 평소 감속감
    public float stopAngularDrag = 0.6f;  // Stop 후 서서히 감속(더 크게)
    public float dragLerpSpeed = 5f;    // drag 보간 속도

    Rigidbody _rb;
    bool _queued;
    bool _stopping;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.angularDamping = normalAngularDrag;
        // 권장 Constraints: Position X/Y/Z = Freeze, Rotation X/Z = Freeze (Y만 자유)
    }

    void Update()
    {
        // 임시 대체 입력: S 키로 Stop (UI 버튼 없을 때용)
        if (Input.GetKeyDown(KeyCode.S)) StopInput();

        if (!allowInput) return;
        if (Input.GetKeyDown(KeyCode.Space)) _queued = true; // 물리 프레임에서 토크 적용
    }

    void FixedUpdate()
    {
        if (_queued && allowInput)
        {
            _rb.AddTorque(Vector3.up * tapTorque, ForceMode.Impulse);
            _queued = false;
        }
        else _queued = false;

        // Stop 이후 서서히 감속: angularDrag를 보간해서 키움(관성은 유지)
        if (_stopping)
        {
            _rb.angularDamping = Mathf.Lerp(_rb.angularDamping, stopAngularDrag, dragLerpSpeed * Time.fixedDeltaTime);
        }
    }

    // === 외부/UI에서 호출할 Stop ===
    public void StopInput()
    {
        allowInput = false;    // 이후 스페이스바 무시
        _queued = false;
        _stopping = true;      // 서서히 감속 시작(회전 자체는 물리적으로 계속)
        // angularVelocity는 건드리지 않음
    }
    public void ResetMotor(bool resetRotation = true)
    {
        allowInput = true;     // 입력 다시 허용
        _stopping = false;     // 감속 모드 해제
        _queued = false;

        _rb.angularDamping = normalAngularDrag;
        _rb.angularVelocity = Vector3.zero;
        _rb.linearVelocity = Vector3.zero;

        if (resetRotation)
            transform.localRotation = Quaternion.identity;
    }
}