using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SpiderController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public GameObject wall;

    private Rigidbody2D rb;
    private SpiderControls controls;
    private Vector2 moveInput;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == wall)
        {
            SceneManager.LoadScene("Main");
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controls = new SpiderControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}
