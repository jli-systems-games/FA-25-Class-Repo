using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;

public class DirectP2Control : MonoBehaviour
{
    public Character _character;
    public CharacterMovement _movement;
    public CharacterRun _run;
    public CharacterHandleWeapon _weapon;

    void Start()
    {
        _character = GetComponent<Character>();
        _movement = GetComponent<CharacterMovement>();
        _run = GetComponent<CharacterRun>();
        _weapon = GetComponent<CharacterHandleWeapon>();
    }

    void Update()
    {
        if (_character == null || _movement == null)
        {
            return;
        }

        // 1. 处理死亡
        if (_character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead)
        {
            _movement.SetHorizontalMovement(0f);
            _movement.SetVerticalMovement(0f);
            return;
        }

        // 2. 处理移动（小键盘：2 下，5 上，1 左，3 右）
        float h = 0f;
        float v = 0f;

        if (Input.GetKey(KeyCode.Keypad2)) { v = -1f; }  // 下
        if (Input.GetKey(KeyCode.Keypad5)) { v = 1f; }  // 上
        if (Input.GetKey(KeyCode.Keypad1)) { h = -1f; }  // 左
        if (Input.GetKey(KeyCode.Keypad3)) { h = 1f; }  // 右

        _movement.SetHorizontalMovement(h);
        _movement.SetVerticalMovement(v);

        // 3. 跑步（保持原来的右 Shift，懒得改可以不管）
        if (_run != null)
        {
            if (Input.GetKeyDown(KeyCode.RightShift)) { _run.RunStart(); }
            if (Input.GetKeyUp(KeyCode.RightShift)) { _run.RunStop(); }
        }

        // 4. 扔石头 / 攻击（按住小键盘 Enter）
        if (_weapon != null)
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter)) { _weapon.ShootStart(); }
            if (Input.GetKeyUp(KeyCode.KeypadEnter)) { _weapon.ShootStop(); }
        }

        // 5. 交互（小键盘 6）
        if (_character.LinkedInputManager != null)
        {
            if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                _character.LinkedInputManager.InteractButton.State.ChangeState(MMInput.ButtonStates.ButtonDown);
            }
            if (Input.GetKeyUp(KeyCode.Keypad6))
            {
                _character.LinkedInputManager.InteractButton.State.ChangeState(MMInput.ButtonStates.ButtonUp);
            }
        }
    }
}
