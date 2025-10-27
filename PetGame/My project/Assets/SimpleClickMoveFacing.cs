using UnityEngine;

public class SimpleClickMoveFacing : MonoBehaviour
{
    public Camera cam;
    public LayerMask groundMask;
    public float moveSpeed = 3.5f;
    public float turnSpeed = 9f;
    public float stopDistance = 0.1f;
    public Animator animator;

    Vector3 target;
    bool hasTarget = false;
    int hashSpeed = Animator.StringToHash("Speed");

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out var hit, 200f, groundMask))
            {
                target = hit.point;
                hasTarget = true;
            }
        }

        float speedParam = 0f;

        if (hasTarget)
        {
            Vector3 to = target - transform.position; to.y = 0f;
            float dist = to.magnitude;

            if (dist > stopDistance)
            {
                // 旋转
                Quaternion rot = Quaternion.LookRotation(to.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * Time.deltaTime);

                // 前进（朝角色正前）
                transform.position += transform.forward * moveSpeed * Time.deltaTime;
                speedParam = moveSpeed;
            }
            else
            {
                hasTarget = false;
            }
        }

        if (animator) animator.SetFloat(hashSpeed, speedParam);
    }
}
