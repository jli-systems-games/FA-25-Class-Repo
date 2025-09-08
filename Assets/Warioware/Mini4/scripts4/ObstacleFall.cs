using UnityEngine;

public class ObstacleFall : MonoBehaviour
{ 
    public float fallSpeed = 6f;
    public float killY = -10f;
    public LifeManager life;   // 인스펙터에서 LifeManager 드래그

    void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;              // 트리거 충돌로 처리
    }

    void Update()
    {
        transform.Translate(Vector2.down * (fallSpeed * Time.deltaTime));
        if (transform.position.y < killY) Destroy(gameObject); // 화면 밖 정리
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (life != null) life.LoseLife(); // 맞으면 목숨 -1
        Destroy(gameObject);
    }
}