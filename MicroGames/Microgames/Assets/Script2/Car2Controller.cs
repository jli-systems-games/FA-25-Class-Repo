using UnityEngine;

public class Car2Controller : MonoBehaviour
{
    [Header("Move")]
    public float speed = 12f;
    public float destroyX = 15f;

    [Header("Final Flag")]
    public bool isFinalCar = false;   // 右侧车设为 true

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        float x = transform.position.x;
        if (x > destroyX || x < -destroyX)
        {
            if (isFinalCar)
            {
                GameController.I?.Win();   // 最终车离开 → 胜利
            }
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameController.I?.Fail();      // 撞到人 → 失败
            Destroy(gameObject);
        }
    }
}
