using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Tuning")]
    public float moveSpeed = 5f; // 基础移动速度
    public float inertia = 0.98f; // 惯性
    public float drunkDriftSpeed = 5f; // 漂移速度
    public float moveLimit = 10f; // 移动范围限制
    private float velocityX; // X 轴速度
    private float driftDirection; // 随机漂移方向

    void Start()
    {
        driftDirection = Random.value > 0.5f ? 1f : -1f;
    }

    void Update()
    {
        float input = 0f;
        if (Input.GetKey(KeyCode.A)) input = -1f; // 左移
        if (Input.GetKey(KeyCode.D)) input = 1f; // 右移

        if (input == 0f)
        {
            velocityX += driftDirection * drunkDriftSpeed * Time.deltaTime;
        }
        else
        {
            velocityX = Mathf.Lerp(velocityX, input * moveSpeed, 1f - inertia);
            driftDirection = -input;
        }

        Vector3 newPosition = transform.position + new Vector3(velocityX * Time.deltaTime, 0f, 0f);

        if (moveLimit > 0)
        {
            newPosition.x = Mathf.Clamp(newPosition.x, -moveLimit, moveLimit);
        }

        transform.position = newPosition;

        if (Random.value < 0.01f)
        {
            driftDirection = Random.value > 0.5f ? 1f : -1f;
        }
    }
}