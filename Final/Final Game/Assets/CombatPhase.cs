using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;

public class CombatPhaseController : MonoBehaviour
{
    [Header("绑定 (自动获取)")]
    public Character _character;
    public CharacterHandleWeapon _handleWeapon;

    [Header("配置")]
    public KeyCode InteractKey; // P1填F, P2填Return
    // public bool IsHuntingPhase = false; // [修改] 移除此开关限制，让逻辑一直运行

    // 内部变量
    private Vector3 _lastMoveDirection = Vector3.forward;

    void Start()
    {
        _character = GetComponent<Character>();
        _handleWeapon = GetComponent<CharacterHandleWeapon>();
    }

    void Update()
    {
        // [修改] 移除了 !IsHuntingPhase 的判断，确保一直运行
        if (_character == null || _character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead) return;

        // 1. 读取移动输入 (恢复了原本的手动读取逻辑)
        RecordMovementDirection();

        // 2. 处理射击
        HandleShooting();
    }

    // 【核心】恢复 LateUpdate 覆盖所有引擎自带的旋转逻辑
    void LateUpdate()
    {
        // [修改] 移除了 !IsHuntingPhase 的判断
        if (_character == null || _character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead) return;

        // 3. 强制锁定：人转哪，枪就转哪
        ForceLockWeaponToBody();
    }

    void RecordMovementDirection()
    {
        float h = 0f;
        float v = 0f;

        // [恢复] 直接根据 PlayerID 读取按键，比 InputManager 更直接
        if (_character.PlayerID == "Player1")
        {
            if (Input.GetKey(KeyCode.W)) v = 1f;
            if (Input.GetKey(KeyCode.S)) v = -1f;
            if (Input.GetKey(KeyCode.A)) h = -1f;
            if (Input.GetKey(KeyCode.D)) h = 1f;
        }
        else // Player2
        {
            if (Input.GetKey(KeyCode.UpArrow)) v = 1f;
            if (Input.GetKey(KeyCode.DownArrow)) v = -1f;
            if (Input.GetKey(KeyCode.LeftArrow)) h = -1f;
            if (Input.GetKey(KeyCode.RightArrow)) h = 1f;
        }

        // 只有当有输入时才更新方向
        if (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f)
        {
            _lastMoveDirection = new Vector3(h, 0f, v).normalized;

            // 顺便帮角色模型也转一下 (防止角色不动)
            if (_character.CharacterModel != null)
            {
                _character.CharacterModel.transform.forward = _lastMoveDirection;
            }
        }
    }

    void ForceLockWeaponToBody()
    {
        if (_handleWeapon != null && _handleWeapon.CurrentWeapon != null)
        {
            // --- 暴力手段 1：禁用 WeaponAim 的自动计算 ---
            var weaponAim = _handleWeapon.CurrentWeapon.GetComponent<WeaponAim>();
            if (weaponAim != null)
            {
                // 把旋转速度设为无限大，防止延迟
                weaponAim.WeaponRotationSpeed = 99999f;
                // 告诉它我们要瞄准哪里
                weaponAim.SetCurrentAim(_lastMoveDirection);
            }

            // --- 暴力手段 2：直接修改 Transform (终极保险) ---
            // 如果上面的方法还不行，这一行代码会直接把枪“按”在方向上
            _handleWeapon.CurrentWeapon.transform.rotation = Quaternion.LookRotation(_lastMoveDirection);

            // --- 暴力手段 3：消除散布 ---
            // 这里为了通用性，使用了 GetComponent 检查
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

        // [恢复] 使用 GetKeyDown/Up 逻辑，适合全自动或半自动武器
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