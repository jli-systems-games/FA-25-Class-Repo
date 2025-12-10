using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;

public class P3ShootAndThrow : MonoBehaviour
{
    [Header("P3 的扔石头 + 射击键")]
    [Tooltip("PS5 手柄 X 键 = Joystick1Button1")]
    public KeyCode P3Button = KeyCode.Joystick1Button1;

    private Character _character;
    private InputManager _inputManager;

    void Start()
    {
        _character = GetComponent<Character>();
        if (_character != null)
        {
            _inputManager = _character.LinkedInputManager;
        }
    }

    void Update()
    {
        // 防止空引用 + 只作用在 Player3 身上
        if (_character == null || _inputManager == null) return;
        if (_character.PlayerID != "Player3") return;

        bool down = Input.GetKeyDown(P3Button);
        bool held = Input.GetKey(P3Button);
        bool up = Input.GetKeyUp(P3Button);

        // 如果完全没按，就直接关掉
        if (!down && !held && !up)
        {
            _inputManager.ShootButton.State.ChangeState(MMInput.ButtonStates.Off);
            return;
        }

        if (down)
        {
            _inputManager.ShootButton.State.ChangeState(MMInput.ButtonStates.ButtonDown);
        }
        else if (up)
        {
            _inputManager.ShootButton.State.ChangeState(MMInput.ButtonStates.ButtonUp);
        }
        else if (held)
        {
            _inputManager.ShootButton.State.ChangeState(MMInput.ButtonStates.ButtonPressed);
        }
    }
}
