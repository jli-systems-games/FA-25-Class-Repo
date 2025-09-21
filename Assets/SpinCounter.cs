using UnityEngine;
using TMPro;

public class SpinCounter : MonoBehaviour
{
    [Header("Refs")]
    public Transform spinner;     // 회전 체크 대상(없으면 this)
    public Rigidbody rb;          // 정지 판정용(없으면 GetComponent)

    [Header("UI")]
    public TMP_Text spinLabel;    // "Spins: x/50"
    public TMP_Text resultLabel;  // "SUCCESS"/"FAIL"

    [Header("Goal")]
    public int targetSpins = 50;

    [Header("Stop Detection")]
    public float stopAngularVel = 0.1f; // 이 값 이하이면 거의 정지
    public float stopHoldTime = 0.5f;  // 위 상태가 이 시간 지속되면 정지 확정

    float _prevAngle;   // 이전 Yaw
    float _accum;       // 누적 각도(절댓값)
    int _spins;       // 1회전 단위 카운트
    float _still;       // 정지 유지 타이머
    bool _finished;

    void Awake()
    {
        if (!spinner) spinner = transform;
        if (!rb) rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (resultLabel) resultLabel.text = "";
        _prevAngle = GetWorldSignedYaw();
        UpdateSpinUI();
    }

    void FixedUpdate()
    {
        if (_finished) return;

        CountRevolutions();

        // 멈춤 판정: 일정 시간 유지 시 최종 판정
        if (rb && rb.angularVelocity.magnitude < stopAngularVel && _spins > 0)
        {
            _still += Time.fixedDeltaTime;
            if (_still >= stopHoldTime)
            {
                _finished = true;
                Judge();
            }
        }
        else _still = 0f;
    }

    // 360° 누적 → +1회전
    void CountRevolutions()
    {
        float now = GetWorldSignedYaw();                 // -180~+180
        float delta = Mathf.DeltaAngle(_prevAngle, now); // -180~+180

        if (Mathf.Abs(delta) >= 0.05f) // 미세 진동 필터
        {
            _accum += Mathf.Abs(delta);
            while (_accum >= 360f)
            {
                _accum -= 360f;
                _spins++;
                UpdateSpinUI();
            }
        }
        _prevAngle = now;
    }

    float GetWorldSignedYaw()
    {
        Vector3 f = spinner.forward; f.y = 0f;
        if (f.sqrMagnitude < 1e-6f) return _prevAngle;
        f.Normalize();
        return Vector3.SignedAngle(Vector3.forward, f, Vector3.up);
    }

    void UpdateSpinUI()
    {
        if (spinLabel) spinLabel.text = $"Spins: {_spins}/{targetSpins}";
    }

    void Judge()
    {
        if (!resultLabel) return;

        if (_spins == targetSpins)
            resultLabel.text = "<color=#FF1493>SUCCESS</color>"; // 핑크
        else
            resultLabel.text = $"<color=#FF3B30>FAIL</color> ({_spins}/{targetSpins})";
    }



    public void ResetMission()
{
    _spins = 0;
    _accum = 0f;
    _still = 0f;
    _finished = false;

    if (resultLabel) resultLabel.text = "";
    UpdateSpinUI();

    _prevAngle = GetWorldSignedYaw(); // 기준각 갱신
}
}