using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 3f;
    public Transform mainCamera;

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        bool isMoving = moveInput.sqrMagnitude > 0.01f;
        anim.SetBool("isHopping", isMoving);

        if (moveInput.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(moveInput.x), 1, 1);

        if(Input.GetMouseButtonDown(0))anim.SetBool("isAttacking",true );
        else if(Input.GetMouseButtonUp(0))anim.SetBool("isAttacking",false );
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * speed * Time.fixedDeltaTime);
    }
    void LateUpdate()
    {
        if (mainCamera != null)
        {
            Vector3 camPos = transform.position;
            camPos.z = -5.93f;
            mainCamera.position = camPos;
        }
    }
}
