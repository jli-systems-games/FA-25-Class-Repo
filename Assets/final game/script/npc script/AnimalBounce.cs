using UnityEngine;

public class AnimalBounce : MonoBehaviour
{
    [Header("바운스 설정")]
    [Tooltip("상하로 움직이는 최대 높이")]
    public float bounceHeight = 0.5f;

    [Tooltip("튀는 속도 (숫자가 높을수록 빠름)")]
    public float bounceSpeed = 4f;

    [Tooltip("멈출 때 원래 위치로 돌아가는 속도")]
    public float stopLerpSpeed = 8f;

    [Header("플레이어 근처에서 멈추기")]
    public Transform player;
    [Tooltip("이 거리 안으로 오면 바운스 멈춤")]
    public float stopDistance = 2f;

    // 내부 변수
    private Vector3 _startPos;     // NPC가 바운스할 중심 위치
    private float _timeOffset;     // NPC마다 튀는 타이밍을 다르게 하기 위한 오프셋
    private bool _isBouncing = true; // 현재 바운스 중인지 상태 관리

    void Start()
    {
        // 1. 초기 위치를 저장합니다.
        _startPos = transform.position;

        // 2. 랜덤한 시간 오프셋을 주어 동시에 튀지 않도록 합니다.
        _timeOffset = Random.Range(0f, 10f);

        // 3. 플레이어 트랜스폼을 자동으로 찾습니다.
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        bool playerIsNear = false;

        if (player != null)
        {
            float dist = Vector3.Distance(transform.position, player.position);

            // 플레이어가 가까우면 -> 멈춤 상태로 전환
            if (dist <= stopDistance)
            {
                playerIsNear = true;
            }
        }

        // 1. 상태 전환
        if (playerIsNear)
        {
            _isBouncing = false; // 멈춤
        }
        else
        {
            _isBouncing = true; // 바운스 시작
        }

        // 2. 동작 실행
        if (_isBouncing)
        {
            // 바운스 로직 (움직임)
            float timeValue = (Time.time + _timeOffset) * bounceSpeed;
            float yOffset = Mathf.Sin(timeValue) * bounceHeight;

            // X, Z는 고정하고 Y축만 바운스
            transform.position = _startPos + new Vector3(0f, yOffset, 0f);
        }
        else
        {
            // 정지 로직 (원래 위치로 부드럽게 돌아가기)
            if (transform.position != _startPos)
            {
                // Vector3.Lerp를 사용하여 현재 위치에서 _startPos로 천천히 이동
                transform.position = Vector3.Lerp(transform.position, _startPos, Time.deltaTime * stopLerpSpeed);
            }
        }
    }
}