using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;

public class CombatPhaseController : MonoBehaviour
{
    [Header("绑定 (自动获取)")]
    public Character _character;
    public CharacterHandleWeapon _handleWeapon;

    [Header("配置")]
    public KeyCode InteractKey; // P1 填 F, P2 填 KeypadEnter（扔石头用）

    // 内部变量
    private Vector3 _lastMoveDirection = Vector3.forward;
    private Rigidbody _rb;

    void Start()
    {
        _character = GetComponent<Character>();
        _handleWeapon = GetComponent<CharacterHandleWeapon>();
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (_character == null
            || _character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead
            || _character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Frozen)
        {
            return;
        }

        // 1. 记录方向（P1/P2 用键盘，P3/P4 用“面朝方向”）
        RecordMovementDirection();

        // 2. 处理射击
        HandleShooting();
    }

    void LateUpdate()
    {
        if (_character == null
            || _character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead
            || _character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Frozen)
        {
            return;
        }

        // 3. 强制：武器和子弹朝 _lastMoveDirection
        ForceLockWeaponToBody();
    }

    void RecordMovementDirection()
    {
        float h = 0f;
        float v = 0f;
        bool hasKeyboardInput = false;

        // ---------------- P1：WASD ----------------
        if (_character.PlayerID == "Player1")
        {
            if (Input.GetKey(KeyCode.W)) { v = 1f; hasKeyboardInput = true; }
            if (Input.GetKey(KeyCode.S)) { v = -1f; hasKeyboardInput = true; }
            if (Input.GetKey(KeyCode.A)) { h = -1f; hasKeyboardInput = true; }
            if (Input.GetKey(KeyCode.D)) { h = 1f; hasKeyboardInput = true; }
        }
        // ---------------- P2：小键盘 2 / 5 / 1 / 3 ----------------
        else if (_character.PlayerID == "Player2")
        {
            if (Input.GetKey(KeyCode.Keypad2)) { v = -1f; hasKeyboardInput = true; } // 下
            if (Input.GetKey(KeyCode.Keypad5)) { v = 1f; hasKeyboardInput = true; } // 上
            if (Input.GetKey(KeyCode.Keypad1)) { h = -1f; hasKeyboardInput = true; } // 左
            if (Input.GetKey(KeyCode.Keypad3)) { h = 1f; hasKeyboardInput = true; } // 右
        }
        // ---------------- P3 / P4：永远用“面朝方向” ----------------
        else if (_character.PlayerID == "Player3" || _character.PlayerID == "Player4")
        {
            // 优先用角色模型的 forward
            if (_character.CharacterModel != null)
            {
                Vector3 fwd = _character.CharacterModel.transform.forward;
                fwd.y = 0f;
                if (fwd.sqrMagnitude > 0.0001f)
                {
                    _lastMoveDirection = fwd.normalized;
                }
            }
            // 如果没有模型，才退回用刚体速度（一般用不到）
            else if (_rb != null)
            {
                Vector3 vel = _rb.linearVelocity;
                vel.y = 0f;
                if (vel.sqrMagnitude > 0.01f)
                {
                    _lastMoveDirection = vel.normalized;
                }
            }

            // P3 / P4 在这里就已经更新完方向了，直接 return
            return;
        }

        // ---------- P1 / P2：有键盘输入就更新方向 ----------
        if (hasKeyboardInput && (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f))
        {
            _lastMoveDirection = new Vector3(h, 0f, v).normalized;

            if (_character.CharacterModel != null)
            {
                _character.CharacterModel.transform.forward = _lastMoveDirection;
            }
        }
    }

    // 小工具：从角色当前朝向同步 _lastMoveDirection（给 P3/P4 用）
    void SyncDirectionWithFacing()
    {
        if ((_character.PlayerID == "Player3" || _character.PlayerID == "Player4")
            && _character.CharacterModel != null)
        {
            Vector3 fwd = _character.CharacterModel.transform.forward;
            fwd.y = 0f;
            if (fwd.sqrMagnitude > 0.0001f)
            {
                _lastMoveDirection = fwd.normalized;
            }
        }
    }

    void ForceLockWeaponToBody()
    {
        if (_handleWeapon != null && _handleWeapon.CurrentWeapon != null)
        {
            var weaponAim = _handleWeapon.CurrentWeapon.GetComponent<WeaponAim>();
            if (weaponAim != null)
            {
                weaponAim.WeaponRotationSpeed = 99999f;
                weaponAim.SetCurrentAim(_lastMoveDirection);
            }

            _handleWeapon.CurrentWeapon.transform.rotation = Quaternion.LookRotation(_lastMoveDirection);

            var projectileWeapon = _handleWeapon.CurrentWeapon.GetComponent<ProjectileWeapon>();
            if (projectileWeapon != null)
            {
                projectileWeapon.Spread = Vector3.zero;
            }
        }
    }

    void HandleShooting()
    {
        if (_handleWeapon == null) return;

        // ---------------- P3：手柄1 Button0 ----------------
        if (_character.PlayerID == "Player3")
        {
            bool down = Input.GetKeyDown(KeyCode.Joystick1Button0);
            bool up = Input.GetKeyUp(KeyCode.Joystick1Button0);

            if (down)
            {
                SyncDirectionWithFacing();    // 开枪前再对齐一次
                _handleWeapon.ShootStart();
            }
            if (up) _handleWeapon.ShootStop();
            return;
        }

        // ---------------- P4：手柄2 Button0 ----------------
        if (_character.PlayerID == "Player4")
        {
            bool down = Input.GetKeyDown(KeyCode.Joystick2Button0);
            bool up = Input.GetKeyUp(KeyCode.Joystick2Button0);

            if (down)
            {
                SyncDirectionWithFacing();    // 开枪前再对齐一次
                _handleWeapon.ShootStart();
            }
            if (up) _handleWeapon.ShootStop();
            return;
        }

        // ---------------- P1 / P2：键盘射击 ----------------
        if (Input.GetKeyDown(InteractKey))
        {
            _handleWeapon.ShootStart();
        }

        if (Input.GetKeyUp(InteractKey))
        {
            _handleWeapon.ShootStop();
        }
    }
}
