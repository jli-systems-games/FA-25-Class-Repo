using UnityEngine;
using TMPro; // TMP 사용

public class PlayerPaddle : Paddle
{
    private Vector2 _direction;

    [Header("Reverse (랜덤 뒤집힘)")]
    [Tooltip("시작 후 일정 시간마다 조작이 랜덤으로 뒤집힙니다.")]
    [SerializeField] private bool enableRandomReverse = true;

    [Tooltip("다음 뒤집힘까지 최소 대기 시간(초)")]
    [SerializeField] private float minInterval = 5f;

    [Tooltip("다음 뒤집힘까지 최대 대기 시간(초)")]
    [SerializeField] private float maxInterval = 10f;

    [Tooltip("뒤집힌 상태가 유지되는 시간(초)")]
    [SerializeField] private float invertedDuration = 5.0f; // ★ 3초 유지

    [Tooltip("게임 시작 시 뒤집힌 상태로 시작")]
    [SerializeField] private bool startInverted = false;

    [Header("UI")]
    [Tooltip("뒤집힘 상태/카운트다운을 표시할 TMP 텍스트")]
    [SerializeField] private TMP_Text invertIndicator;
    [SerializeField] private string normalText = "";                 // 평상시
    [SerializeField] private string invertedPrefixText = "REVERSED"; // 리버스 텍스트 앞부분
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color invertedColor = Color.red;

    private bool _isInverted;
    private float _timer;              // 다음 상태 전환까지 남은 시간
    private bool _inInvertedWindow;    // 현재 리버스 유지 중인지
    private bool _prevInverted;        // UI 상태 변경 감지용
    private float _invertRemain;       // 리버스 남은 시간(카운트다운 표시용)

    private void Start()
    {
        _isInverted = startInverted;
        _inInvertedWindow = _isInverted;

        if (_inInvertedWindow)
        {
            _invertRemain = invertedDuration;
        }
        else if (enableRandomReverse)
        {
            ScheduleNext();
        }

        UpdateInvertUI(force: true);
    }

    private void Update()
    {
        // ==== 입력 처리 ====
        bool upPressed = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool downPressed = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

        if (!_isInverted)
        {
            if (upPressed) _direction = Vector2.up;
            else if (downPressed) _direction = Vector2.down;
            else _direction = Vector2.zero;
        }
        else
        {
            if (upPressed) _direction = Vector2.down;
            else if (downPressed) _direction = Vector2.up;
            else _direction = Vector2.zero;
        }

        // ==== 랜덤 뒤집힘 타이머 ====
        if (enableRandomReverse)
        {
            if (_inInvertedWindow)
            {
                // 리버스 유지 시간 카운트다운
                _invertRemain -= Time.deltaTime;
                _timer -= Time.deltaTime;

                // UI 텍스트를 프레임마다 업데이트 (3,2,1 정수로)
                UpdateInvertUI();

                if (_timer <= 0f)
                {
                    // 리버스 종료 → 정상화
                    _isInverted = false;
                    _inInvertedWindow = false;
                    _invertRemain = 0f;
                    ScheduleNext();
                    UpdateInvertUI();
                }
            }
            else
            {
                // 다음 리버스까지 대기
                _timer -= Time.deltaTime;
                if (_timer <= 0f)
                {
                    // 리버스 시작
                    _isInverted = true;
                    _inInvertedWindow = true;
                    _invertRemain = invertedDuration;
                    _timer = invertedDuration; // 정확히 invertedDuration만큼 유지
                    UpdateInvertUI();
                    // 여기서 SFX/플래시 트리거 해도 좋음
                }
            }
        }
        else
        {
            // 랜덤 기능 비활성화 시에도 UI는 상태 변화 시만 갱신
            if (_prevInverted != _isInverted) UpdateInvertUI();
        }
    }

    private void FixedUpdate()
    {
        if (_direction.sqrMagnitude != 0f)
        {
            _rigidbody.AddForce(_direction * this.speed);
        }
    }

    private void ScheduleNext()
    {
        _timer = Random.Range(minInterval, maxInterval);
    }

    // === UI 갱신: "REVERSED 3", "REVERSED 2", "REVERSED 1" ===
    private void UpdateInvertUI(bool force = false)
    {
        _prevInverted = _isInverted;

        if (!invertIndicator) return;

        if (_isInverted)
        {
            // 남은 시간을 1,2,3 정수로 보이게 (0.0~1.0 구간도 1로 보이게)
            int count = Mathf.Max(1, Mathf.CeilToInt(_invertRemain));
            invertIndicator.text = $"{invertedPrefixText}  {count}";
            invertIndicator.color = invertedColor;
        }
        else
        {
            invertIndicator.text = normalText;
            invertIndicator.color = normalColor;
        }

        if (force) invertIndicator.ForceMeshUpdate();
    }

    public bool IsInverted => _isInverted;
}
