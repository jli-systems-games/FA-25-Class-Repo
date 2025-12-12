using UnityEngine;

public class SimpleBulletMovement : MonoBehaviour
{
    [HideInInspector]
    public Vector3 direction;

    [HideInInspector]
    public float speed;

    [Header("生命周期设置")]
    [Tooltip("子弹存活时间（秒），0表示永久存在")]
    public float lifetime = 10f;

    [Header("碰撞设置")]
    [Tooltip("碰撞到Building标签的物体时销毁")]
    public bool destroyOnBuilding = true;

    private float timer = 0f;

    void Update()
    {
        // 移动子弹
        transform.position += direction * speed * Time.deltaTime;

        // 生命周期计时
        if (lifetime > 0)
        {
            timer += Time.deltaTime;
            if (timer >= lifetime)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"碰撞到: {collision.gameObject.name}, Tag: {collision.gameObject.tag}");

        // 检测碰撞物体是否带有 Building 标签
        if (destroyOnBuilding && collision.gameObject.CompareTag("Building"))
        {
            Debug.Log("检测到Building，销毁子弹！");
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"触发: {other.gameObject.name}, Tag: {other.gameObject.tag}");

        // 检测触发物体是否带有 Building 标签
        if (destroyOnBuilding && other.CompareTag("Building"))
        {
            Debug.Log("检测到Building触发器，销毁子弹！");
            Destroy(gameObject);
        }
    }
}