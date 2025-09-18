using UnityEngine;

public class MouseForceFieldController : MonoBehaviour
{
    public Camera mainCamera;
    public ParticleSystemForceField forceField;

    [Header("Radius")]
    public float scrollSpeed = 2f;

    [Header("Gravity Strengths")]
    public float defaultStrength = 0f;   // 平时的强度
    public float attractStrength = -10f; // 左键按下：吸引
    public float repelStrength = 10f;  // 右键按下：排斥

    void Update()
    {
        // 鼠标 -> 世界坐标（锁到 z=0）
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; // 任意正值即可
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f;
        transform.position = worldPos;

        // 滚轮调半径
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.0001f)
            forceField.endRange = Mathf.Max(0.1f, forceField.endRange + scroll * scrollSpeed);

        // 左键：吸引
        if (Input.GetMouseButtonDown(0))
            forceField.gravity = attractStrength;

        // 右键：排斥
        if (Input.GetMouseButtonDown(1))
            forceField.gravity = repelStrength;

        // 松开任意键：恢复默认
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            forceField.gravity = defaultStrength;
    }
}