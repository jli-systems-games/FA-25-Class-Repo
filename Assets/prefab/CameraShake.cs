using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("抖动参数")]
    public float amplitude = 0.05f;  // 抖动de幅度
    public float frequency = 10f;    // 抖动de频率

    private Vector3 originalPos;

    void Start()
    {
        originalPos = transform.localPosition; // 这是初始位置就是 最开始啥样定一下
    }

    void Update()
    {
       //wtf这个好高级如果以后有人问就说一下不是自己写的因为这个涉及到cos sin了反正就是用这个去让这摄像头抖一下 就是嗯 很高级
        float offsetX = Mathf.Sin(Time.time * frequency) * amplitude;
        float offsetY = Mathf.Abs(Mathf.Cos(Time.time * frequency * 0.5f)) * amplitude;

        transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0f);
    }
}
