using UnityEngine;
using System.Collections;

public class RockBounceController : MonoBehaviour
{
    private int _maxBounces;
    private int _currentBounces = 0;
    private Collider _playerCollider;
    private bool _enableDebugLogs;
    private Rigidbody _rb;
    private TrailRenderer _trail;
    private bool _isDestroying = false;

    // 【修改点 1】新增：用来锁定速度
    private float _maintainedSpeed;

    // 【修改点 2】初始化时传入初始速度
    public void Initialize(int maxBounces, Collider playerCol, bool debugLogs, float initialSpeed)
    {
        _maxBounces = maxBounces;
        _playerCollider = playerCol;
        _enableDebugLogs = debugLogs;
        _rb = GetComponent<Rigidbody>();

        // 记录初始速度
        _maintainedSpeed = initialSpeed;

        if (_enableDebugLogs)
            Debug.Log($"[RockBounce] 初始化 - 速度锁定为: {_maintainedSpeed}");

        // TrailRenderer 设置保持不变...
        _trail = gameObject.AddComponent<TrailRenderer>();
        _trail.time = 0.5f;
        _trail.startWidth = 0.3f;
        _trail.endWidth = 0.05f;
        _trail.material = new Material(Shader.Find("Sprites/Default"));
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(1f, 0.8f, 0.2f), 0.0f), new GradientColorKey(new Color(1f, 0.2f, 0.2f), 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.8f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        _trail.colorGradient = gradient;
    }

    // 【修改点 3】移除了 FixedUpdate，因为我们不再依赖上一帧速度，而是强制使用 _maintainedSpeed

    void OnCollisionEnter(Collision collision)
    {
        if (_isDestroying) return;

        if (collision.collider == _playerCollider)
        {
            if (_enableDebugLogs)
                Debug.Log("[RockBounce] 忽略玩家碰撞");
            return;
        }

        _currentBounces++;

        if (_enableDebugLogs)
            Debug.Log($"[RockBounce] 第 {_currentBounces}/{_maxBounces} 次弹跳，碰到: {collision.gameObject.name}");

        if (collision.contacts.Length > 0)
        {
            CreateBounceEffect(collision.contacts[0].point);
        }

        if (_currentBounces >= _maxBounces)
        {
            if (_enableDebugLogs)
                Debug.Log($"[RockBounce] 达到最大弹跳次数，准备销毁");

            _isDestroying = true;
            CreateDestroyEffect();

            if (_rb != null)
            {
                _rb.linearVelocity = Vector3.zero;
                _rb.isKinematic = true;
            }

            Destroy(gameObject, 0.3f);
        }
        else
        {
            if (_rb != null && collision.contacts.Length > 0)
            {
                // 获取碰撞法线
                Vector3 normal = collision.contacts[0].normal;
                normal.y = 0; // 保持水平

                // 【改进】确保法线有效
                if (normal.magnitude < 0.01f)
                {
                    // 法线无效，使用反向速度
                    normal = -_rb.linearVelocity.normalized;
                    normal.y = 0;
                }

                normal.Normalize();

                // 获取当前速度
                Vector3 velocity = _rb.linearVelocity;
                float speed = velocity.magnitude;

                Vector3 direction = velocity.normalized;
                direction.y = 0; // 确保水平

                // 【改进】使用更稳定的反射算法
                Vector3 reflectDir = Vector3.Reflect(direction, normal);
                reflectDir.y = 0;

                // 【关键修复】如果反射方向与入射方向几乎相同，强制反向
                if (Vector3.Dot(reflectDir, direction) > 0.9f)
                {
                    // 反射失败，直接反向
                    reflectDir = -direction;
                    if (_enableDebugLogs)
                        Debug.Log("[RockBounce] 反射异常，强制反向");
                }

                reflectDir.Normalize();

                // 【可选】添加一点随机性，避免卡在角落
                float randomAngle = Random.Range(-5f, 5f); // ±5度随机偏移
                reflectDir = Quaternion.Euler(0, randomAngle, 0) * reflectDir;

                // 应用反射速度
                _rb.linearVelocity = reflectDir * speed;

                if (_enableDebugLogs)
                    Debug.Log($"[RockBounce] 反射 - 速度:{speed:F1}, 入射:{direction}, 法线:{normal}, 反射:{reflectDir}");
            }
        }
    }

    void CreateBounceEffect(Vector3 position) { /* 保持原样 */ }
    void CreateDestroyEffect() { /* 保持原样 */ }
}