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
        // 1. 处理死亡
        if (_character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead)
        {
            _movement.SetHorizontalMovement(0f);
            _movement.SetVerticalMovement(0f);
            return;
        }

        // 2. 处理移动 (方向键)
        float h = 0f;
        float v = 0f;
        if (Input.GetKey(KeyCode.UpArrow)) { v = 1f; }
        if (Input.GetKey(KeyCode.DownArrow)) { v = -1f; }
        if (Input.GetKey(KeyCode.LeftArrow)) { h = -1f; }
        if (Input.GetKey(KeyCode.RightArrow)) { h = 1f; }
        _movement.SetHorizontalMovement(h);
        _movement.SetVerticalMovement(v);

        // 3. 处理跑步 (右Shift)
        if (Input.GetKeyDown(KeyCode.RightShift)) { _run.RunStart(); }
        if (Input.GetKeyUp(KeyCode.RightShift)) { _run.RunStop(); }

        // 4. 处理攻击/扔石头 (斜杠键 /)
        if (Input.GetKeyDown(KeyCode.Slash)) { _weapon.ShootStart(); }
        if (Input.GetKeyUp(KeyCode.Slash)) { _weapon.ShootStop(); }

        // 5. 处理交互 (回车键 Enter) - 使用信号注入修复报错
        if (_character.LinkedInputManager != null)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                _character.LinkedInputManager.InteractButton.State.ChangeState(MMInput.ButtonStates.ButtonDown);
            }
            if (Input.GetKeyUp(KeyCode.Return))
            {
                _character.LinkedInputManager.InteractButton.State.ChangeState(MMInput.ButtonStates.ButtonUp);
            }
        }
    }
}