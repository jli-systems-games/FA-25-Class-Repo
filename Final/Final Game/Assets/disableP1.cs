using UnityEngine;
using MoreMountains.TopDownEngine;

[RequireComponent(typeof(CharacterController))]
public class P1KeyboardOnlyController : MonoBehaviour
{
    [Header("移动参数")]
    public float MoveSpeed = 6f;
    public float RunMultiplier = 1.5f;

    [Tooltip("是否用 LeftShift 作为跑步键")]
    public bool UseRunKey = true;

    [Header("按键设置")]
    public KeyCode KeyUp = KeyCode.W;
    public KeyCode KeyDown = KeyCode.S;
    public KeyCode KeyLeft = KeyCode.A;
    public KeyCode KeyRight = KeyCode.D;
    public KeyCode KeyRun = KeyCode.LeftShift;

    private CharacterController _cc;
    private Character _character;
    private CharacterMovement _characterMovement;
    private TopDownController3D _topdownController;

    void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _character = GetComponent<Character>();
        _characterMovement = GetComponent<CharacterMovement>();
        _topdownController = GetComponent<TopDownController3D>();

        // 1. 禁用 TDE 自带的移动逻辑（不再读 InputManager 的摇杆/键盘）
        if (_characterMovement != null) _characterMovement.enabled = false;
        if (_topdownController != null) _topdownController.enabled = false;

        // 2. ❌ 不再修改 PlayerID，让 BuildZone / SmartScalingWall 还能找到 Player1
        // if (_character != null) _character.PlayerID = "";
    }

    void Update()
    {
        Vector3 input = ReadKeyboardDirection();

        if (input.sqrMagnitude > 1f)
        {
            input.Normalize();
        }

        float speed = MoveSpeed;
        if (UseRunKey && Input.GetKey(KeyRun))
        {
            speed *= RunMultiplier;
        }

        Vector3 motion = input * speed * Time.deltaTime;
        _cc.Move(motion);

        if (input.sqrMagnitude > 0.0001f)
        {
            transform.forward = input;
        }
    }

    Vector3 ReadKeyboardDirection()
    {
        float h = 0f;
        float v = 0f;

        if (Input.GetKey(KeyLeft)) h -= 1f;
        if (Input.GetKey(KeyRight)) h += 1f;
        if (Input.GetKey(KeyDown)) v -= 1f;
        if (Input.GetKey(KeyUp)) v += 1f;

        return new Vector3(h, 0f, v);
    }
}
