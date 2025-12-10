using UnityEngine;

public class ForceFaceMoveDirection : MonoBehaviour
{
    public float rotateSpeed = 20f;        // 转身速度
    public bool ignoreVertical = true;     // 忽略上下高度，只在地面转

    private Vector3 _lastPosition;

    void Start()
    {
        _lastPosition = transform.position;
    }

    void LateUpdate()
    {
        Vector3 delta = transform.position - _lastPosition;

        // 不看 Y，防止往上掉落也改变朝向
        if (ignoreVertical)
        {
            delta.y = 0f;
        }

        // 没有移动就不要转身
        if (delta.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(delta.normalized, Vector3.up);
            // 直接瞬间转向可以用 transform.rotation = targetRot;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
        }

        _lastPosition = transform.position;
    }
}

