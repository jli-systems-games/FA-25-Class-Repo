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
}
