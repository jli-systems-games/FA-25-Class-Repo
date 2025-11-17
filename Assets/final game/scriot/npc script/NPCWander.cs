using UnityEngine;

public class NPCWander : MonoBehaviour
{
    public float moveSpeed = 1.5f;     // 이동 속도
    public float changeDirTime = 2f;   // 몇 초마다 방향 바꿈
    public float moveRadius = 10f;     // 돌아다닐 반경

    Vector3 _startPos;
    Vector3 _currentDir;
    float _timer;

    void Start()
    {
        _startPos = transform.position;
        PickNewDirection();
    }

    void Update()
    {
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
