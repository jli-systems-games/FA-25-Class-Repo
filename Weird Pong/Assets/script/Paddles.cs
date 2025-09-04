using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class Paddles : MonoBehaviour
{
    public PlayerInput playerInput;   
    public float speed = 10f;
    public GameObject electricEffectPrefab; 

    private Vector2 moveInput;
    private bool isSlowed = false;
    void OnEnable()
    {
        var actions = playerInput.actions;

       
        if (gameObject.name.Contains("P1"))
        {
            actions["Player1"].performed += ctx => moveInput = ctx.ReadValue<Vector2>();
            actions["Player1"].canceled += ctx => moveInput = Vector2.zero;
        }
        else if (gameObject.name.Contains("P2"))
        {
            actions["Player2"].performed += ctx => moveInput = ctx.ReadValue<Vector2>();
            actions["Player2"].canceled += ctx => moveInput = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        
        Vector3 movement = new Vector3(0, moveInput.y, 0) * speed * Time.deltaTime;
        transform.Translate(movement);
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Pika") && !isSlowed)
        {
            StartCoroutine(SlowDown());
        }
    }

    IEnumerator SlowDown()
    {
        isSlowed = true;

        
        speed = speed - 4.5f;

      
        GameObject effect = null;
        if (electricEffectPrefab != null)
        {
            effect = Instantiate(electricEffectPrefab, transform.position, Quaternion.identity, transform);
        }

        
        yield return new WaitForSeconds(6f);

        
        speed = speed + 4.5f;

        
        if (effect != null) Destroy(effect);

        isSlowed = false;
    }
}
