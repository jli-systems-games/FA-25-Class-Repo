using UnityEngine;

public class PlayerBalance : MonoBehaviour
{
    [Header("Auto Sway")]
    public float swayFreq = 0.6f;     // 自动左右摇摆频率
    public float swayPower = 2.5f;    // 自动左右摇摆“速度/力度”

    [Header("Player Counter Input")]
    public float inputPower = 6f;     // A/D 抵消的力度

    [Header("Horizontal Bounds (optional)")]
    public bool clampX = true;
    public float minX = -8f;
    public float maxX = 8f;

    float t;

    void Update()
    {
        t += Time.deltaTime;

        // 自动摇摆产生“速度”
        float autoVel = Mathf.Sin(t * Mathf.PI * 2f * swayFreq) * swayPower;

        // 玩家抵消
        float input = 0f;
        if (Input.GetKey(KeyCode.A)) input -= 1f;
        if (Input.GetKey(KeyCode.D)) input += 1f;
        float inputVel = input * inputPower;

        // 合成位移
        float dx = (autoVel + inputVel) * Time.deltaTime;
        Vector3 p = transform.position;
        p.x += dx;

        if (clampX) p.x = Mathf.Clamp(p.x, minX, maxX);
        transform.position = p;
    }
}
