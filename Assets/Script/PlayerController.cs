using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputActionAsset inputActions;

    private InputAction _action_move;
    private InputAction _action_jump;

    private void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
    }
        
    private void OnDisable()
    {
        inputActions.FindActionMap("Player").Disable();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _action_move = InputSystem.actions.FindAction("Move");
        _action_jump = InputSystem.actions.FindAction("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Movement:" + _action_move.ReadValue<Vector2>());
        Vector2 move = _action_move.ReadValue<Vector2>();

        if(_action_jump.IsPressed())
            Debug.Log("Jump!");
        
    }
}
