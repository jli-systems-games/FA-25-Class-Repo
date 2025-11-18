using UnityEngine;

public class NPCWander : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 1.5f;     // 이동 속도
    public float changeDirTime = 2f;   // 몇 초마다 방향 바꿈
    public float moveRadius = 10f;     // 돌아다닐 반경

    [Header("플레이어 근처에서 멈추기")]
    public Transform player;           // 플레이어 Transform 넣어주기
    public float stopDistance = 2f;    // 이 거리 안으로 오면 멈춤

    Vector3 _startPos;
    Vector3 _currentDir;
    float _timer;

    void Start()
    {
        _startPos = transform.position;
        PickNewDirection();

        // 만약 Inspector에서 안 넣었다면, Tag로 찾아보기 (옵션)
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        // 1) 플레이어와 거리 체크
        if (player != null)
        {
            float dist = Vector3.Distance(transform.position, player.position);

            // 플레이어가 가까우면 → 멈추기 (이동/방향 업데이트 안 함)
            if (dist <= stopDistance)
            {
                // 여기서 나중에 '플레이어가 E 눌렀을 때 인터랙트' 같은 거 연결하면 됨
                return;
            }
        }

        // 2) 평소처럼 방황 로직
        _timer += Time.deltaTime;

        if (_timer >= changeDirTime)
        {
            PickNewDirection();
            _timer = 0f;
        }

        transform.position += _currentDir * moveSpeed * Time.deltaTime;

        Vector3 offset = transform.position - _startPos;
        if (offset.magnitude > moveRadius)
        {
            _currentDir = (-offset).normalized;
        }
    }

    void PickNewDirection()
    {
        float angle = Random.Range(0f, 360f);
        _currentDir = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)).normalized;
    }
}
