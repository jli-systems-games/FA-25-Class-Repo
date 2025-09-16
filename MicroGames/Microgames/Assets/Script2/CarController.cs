using System.Collections;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Move")]
    public float speed = 8f;          // 左车为正，右车为负
    public float destroyX = 15f;      // 超出此范围视为离开镜头

    [Header("Who am I")]
    public bool isFinalCar = false;   // 右车=true；左车=false

    [Header("Spawn next (only on the LEFT car)")]
    public bool spawnRightAfterLeave = false;   // 只有左车勾选
    public GameObject rightCarPrefab;          // 右车 Prefab
    public float rightSpawnDelay = 5f;         // 左车离开后多久出右车
    public Vector2 rightSpawnPos = new Vector2(12f, -3.4f); // 右车出生点(按你的场景改)
    public float rightCarSpeed = -12f;         // 右车速度 (负数→往左)

    [Header("SFX on spawn (optional)")]
    public AudioSource audioSource;            // 车体上的 2D AudioSource
    public AudioClip spawnSfx;                 // 进场音效

    void Start()
    {
        if (audioSource && spawnSfx) audioSource.PlayOneShot(spawnSfx);
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        float x = transform.position.x;
        if (x > destroyX || x < -destroyX)
        {
            // 我离开屏幕了
            if (spawnRightAfterLeave && rightCarPrefab)
                StartCoroutine(SpawnRightCarAfterDelay());

            if (isFinalCar)
                GameController.I?.Win();   // 右车离开→胜利

            Destroy(gameObject);
        }
    }

    IEnumerator SpawnRightCarAfterDelay()
    {
        // 防止重复触发
        spawnRightAfterLeave = false;
        yield return new WaitForSeconds(rightSpawnDelay);

        var car = Instantiate(rightCarPrefab, rightSpawnPos, Quaternion.identity);

        // 设置右车移动方向与速度
        var ctrl = car.GetComponent<CarController>();
        if (ctrl)
        {
            ctrl.speed = rightCarSpeed;   // -12 之类
            ctrl.isFinalCar = true;       // 这是最终判定车
            ctrl.spawnRightAfterLeave = false;
        }

        // 右车外观翻转（如果你的右车 sprite 需要）
        Vector3 s = car.transform.localScale;
        s.x = -Mathf.Abs(s.x);
        car.transform.localScale = s;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameController.I?.Fail();
            Destroy(gameObject);
        }
    }
}
