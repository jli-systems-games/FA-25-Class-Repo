using UnityEngine;

public class ZAxisMover : MonoBehaviour
{
    [Header("移动速度（单位/秒）")]
    public float moveSpeed = 5f;

    [Header("Z轴最大移动距离（正负范围）")]
    public float maxZDistance = 5f;

    private float startZ;
    private bool isActive = false;  // 默认关闭移动

    void Start()
    {
        startZ = transform.position.z;
    }

    void Update()
    {
        if (!isActive) return;  // 没激活就不运行移动逻辑

        float input = Input.GetAxis("Horizontal");  // A/D 或 ←/→ 控制移动

        if (Mathf.Abs(input) > 0.01f)
        {
            float newZ = transform.position.z + input * moveSpeed * Time.deltaTime;
            newZ = Mathf.Clamp(newZ, startZ - maxZDistance, startZ + maxZDistance);

            transform.position = new Vector3(transform.position.x, transform.position.y, newZ);
        }
    }

    // 👉 给Button调用的函数
    public void EnableMovement()
    {
        isActive = true;
    }

    // （可选）关闭移动
    public void DisableMovement()
    {
        isActive = false;
    }
}