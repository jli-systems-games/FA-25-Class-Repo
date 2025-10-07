// PlayerCapsuleController.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerCapsuleController : MonoBehaviour
{
    [Header("Move / Jump")]
    public float moveSpeed = 5f;   // 수평 이동 속도
    public float jumpForce = 7f;   // 점프 힘

    private Rigidbody rb;
    private bool grounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        // 넘어짐 방지(원하면 꺼도 됨)
        rb.freezeRotation = true;
    }

    void Update()
    {
        // 점프(바닥에서만)
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            // 수직 속도 리셋 후 점프
            Vector3 v = rb.linearVelocity;
            v.y = 0f;
            rb.linearVelocity = v;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }

    void FixedUpdate()
    {
        // WASD 이동 (아주 단순: 목표 속도로 바로 세팅)
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
