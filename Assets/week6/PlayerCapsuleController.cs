// PlayerCapsuleController.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerCapsuleController : MonoBehaviour
{
    public float moveSpeed = 5f;   // 수평 이동 속도
    public float jumpForce = 7f;   // 점프 힘

    Rigidbody rb;
    bool grounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        // 넘어짐 방지(원하면 해제 가능)
        rb.freezeRotation = true;
    }

    void Update()
    {
        // 점프(바닥에서만)
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // 수직속도 리셋
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }

    void FixedUpdate()
    {
        // WASD 이동 (아주 단순: 목표속도로 바로 세팅)
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 vel = rb.linearVelocity;
        vel.x = h * moveSpeed;
        vel.z = v * moveSpeed;
        rb.linearVelocity = vel;
    }

    // 간단 바닥 판정: 충돌 중이면 바닥으로 간주
    void OnCollisionStay(Collision c) { grounded = true; }
    void OnCollisionExit(Collision c) { grounded = false; }
}
