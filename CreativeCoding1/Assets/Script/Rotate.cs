using UnityEngine;

public class Rotator : MonoBehaviour
{
    [Header("旋转轴")]
    public bool rotateX = false;
    public bool rotateY = true;
    public bool rotateZ = false;

    [Header("旋转速度")]
    public float speed = 90f; // 度/秒

    [Header("设置")]
    public Space space = Space.Self; // Self = 本地轴，World = 世界轴

    void Update()
    {
        Vector3 axis = new Vector3(
            rotateX ? 1f : 0f,
            rotateY ? 1f : 0f,
            rotateZ ? 1f : 0f
        );

        transform.Rotate(axis * speed * Time.deltaTime, space);
    }
}