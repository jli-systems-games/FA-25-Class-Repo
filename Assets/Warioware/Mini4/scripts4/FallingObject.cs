using UnityEngine;

public class FallingObject : MonoBehaviour
{
    public float fallSpeed = 6f;     // 중력을 안 쓰면 이 값으로 이동
    public float killY = -10f;       // 이 Y 밑으로 내려가면 제거
    public CollectManager manager;   // 수집 보고용

    void Update()
    {
        // 중력 대신 수동 이동(원하면 Rigidbody2D 중력 사용해도 됨)
        transform.Translate(Vector2.down * (fallSpeed * Time.deltaTime));

        if (transform.position.y <= killY)
            Destroy(gameObject);
    }

    // 플레이어와만 트리거 충돌
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        manager?.ReportCollected();  // 수집 보고
        Destroy(gameObject);
    }
}