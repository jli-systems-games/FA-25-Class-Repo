using UnityEngine;

public class CameraSway : MonoBehaviour
{
    public float posAmpX = 0.05f;  // 左右位移振幅（米）
    public float rotAmpZ = 1.5f;   // 旋转 Roll 振幅（度）
    public float speed = 0.8f;   // 速度（Hz）
    public float noise = 0.3f;   // 额外噪声（度）

    Vector3 startPos;
    Quaternion startRot;
    float t;

    void Start()
    {
        startPos = transform.localPosition;
        startRot = transform.localRotation;
    }

    void LateUpdate()
    {
        t += Time.deltaTime;
        float s = Mathf.Sin(2f * Mathf.PI * speed * t);
        float n = (Random.value - 0.5f) * 2f * noise;

        transform.localPosition = startPos + new Vector3(s * posAmpX, 0f, 0f);
        transform.localRotation = startRot * Quaternion.Euler(0f, 0f, s * rotAmpZ + n);
    }
}
