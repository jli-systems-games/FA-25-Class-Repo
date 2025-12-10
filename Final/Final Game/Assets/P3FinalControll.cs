using UnityEngine;
using MoreMountains.TopDownEngine;

/// <summary>
/// **只给 P3 用**：
/// - Joystick1Button0 ：开枪（Shoot）
/// - Joystick1Button1 ：扔石头（RockThrower 充能 + 投掷）
/// 挂在 P3 的根物体上（有 Character、CharacterHandleWeapon 的那一层）
/// </summary>
[DefaultExecutionOrder(9999)] // 尽量最后执行，压过其他脚本
public class P3BossOverride : MonoBehaviour
{
    public KeyCode ShootKey = KeyCode.Joystick1Button0;  // 开枪键
    public KeyCode ThrowKey = KeyCode.Joystick1Button1;  // 扔石头键

    private Character _character;
    private CharacterHandleWeapon _weapon;
    private CharacterRockThrower _rockThrower;

    private bool _throwCharging = false;  // 自己跟踪是否在蓄力状态

    void Awake()
    {
        _character = GetComponent<Character>();
        _weapon = GetComponent<CharacterHandleWeapon>();
        _rockThrower = GetComponent<CharacterRockThrower>();

        if (_character == null)
        {
            Debug.LogError("[P3BossOverride] 没找到 Character，脚本挂的位置不对");
        }
        else if (_character.PlayerID != "Player3")
        {
            Debug.LogWarning("[P3BossOverride] 提醒：这个脚本是给 Player3 用的，当前 PlayerID = " + _character.PlayerID);
        }

        if (_weapon == null)
        {
            Debug.LogWarning("[P3BossOverride] 没找到 CharacterHandleWeapon（P3就不能射击了）");
        }

        if (_rockThrower == null)
        {
            Debug.LogWarning("[P3BossOverride] 没找到 CharacterRockThrower（P3就不能扔石头了）");
        }
    }

    void Update()
    {
        if (_character == null)
        {
            return;
        }

        // 死亡 / 冻结 不处理
        var state = _character.ConditionState.CurrentState;
        if (state == CharacterStates.CharacterConditions.Dead
            || state == CharacterStates.CharacterConditions.Frozen)
        {
            // 顺便保证停枪 & 停蓄力
            if (_weapon != null)
            {
                _weapon.ShootStop();
            }

            if (_throwCharging && _rockThrower != null)
            {
                _rockThrower.SendMessage("StopCharging", SendMessageOptions.DontRequireReceiver);
                _rockThrower.SendMessage("ThrowRock", SendMessageOptions.DontRequireReceiver);
                _throwCharging = false;
            }

            return;
        }

        HandleShooting();
        HandleThrowing();
    }

    /// <summary>
    /// 开枪逻辑：Joystick1Button0 直接控制 CharacterHandleWeapon
    /// </summary>
    void HandleShooting()
    {
        if (_weapon == null) return;

        if (Input.GetKeyDown(ShootKey))
        {
            _weapon.ShootStart();
        }
        if (Input.GetKeyUp(ShootKey))
        {
            _weapon.ShootStop();
        }
    }

    /// <summary>
    /// 扔石头逻辑：Joystick1Button1 直接控制 CharacterRockThrower 里的私有方法
    /// StartCharging / UpdateCharging / StopCharging / ThrowRock
    /// </summary>
    void HandleThrowing()
    {
        if (_rockThrower == null) return;

        bool down = Input.GetKeyDown(ThrowKey);
        bool held = Input.GetKey(ThrowKey);
        bool up = Input.GetKeyUp(ThrowKey);

        // 按下：开始蓄力
        if (down)
        {
            _throwCharging = true;
            _rockThrower.SendMessage("StartCharging", SendMessageOptions.DontRequireReceiver);
        }

        // 长按：持续蓄力 + 画线
        if (_throwCharging && held)
        {
            _rockThrower.SendMessage("UpdateCharging", SendMessageOptions.DontRequireReceiver);
        }

        // 松开：停止蓄力并投掷
        if (_throwCharging && up)
        {
            _throwCharging = false;
            _rockThrower.SendMessage("StopCharging", SendMessageOptions.DontRequireReceiver);
            _rockThrower.SendMessage("ThrowRock", SendMessageOptions.DontRequireReceiver);
        }
    }
}
