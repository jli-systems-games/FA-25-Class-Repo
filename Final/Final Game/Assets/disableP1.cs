using UnityEngine;
using MoreMountains.TopDownEngine;

public class P1KeyboardMovement : CharacterMovement
{
    [Header("拖拽石头武器")]
    public Weapon BigStoneWeapon;      // 拖“大石头 Weapon”进来
    public Weapon SmallStoneWeapon;    // 拖“小石头 Weapon”进来

    [Header("石头额外速度倍率（在别的减速之上再乘一次）")]
    [Range(0f, 1f)] public float BigRockFactor = 0.3f;   // 拿大石头：额外乘 0.3
    [Range(0f, 1f)] public float SmallRockFactor = 0.6f; // 拿小石头：额外乘 0.6

    private CharacterHandleWeapon _handle;
    private string _bigStoneID;
    private string _smallStoneID;

    // 当前“石头减速倍率”，默认 1（没减速）
    private float _rockFactor = 1f;

    protected override void Initialization()
    {
        base.Initialization();
        _handle = GetComponent<CharacterHandleWeapon>();
        CacheWeaponIDs();
    }

    void OnValidate()
    {
        CacheWeaponIDs();
    }

    void CacheWeaponIDs()
    {
        _bigStoneID = (BigStoneWeapon != null) ? BigStoneWeapon.WeaponID : string.Empty;
        _smallStoneID = (SmallStoneWeapon != null) ? SmallStoneWeapon.WeaponID : string.Empty;
    }

    public override void ProcessAbility()
    {
        // 先走 TDE 原本的移动流程（状态机、加速减速等等）
        base.ProcessAbility();

        // 然后只更新 _rockFactor，不直接改 MovementSpeedMultiplier
        UpdateRockFactor();
    }

    /// <summary>
    /// 根据当前武器，更新石头减速倍率（只算一个系数，不立即作用）
    /// </summary>
    void UpdateRockFactor()
    {
        if (_character == null || _handle == null)
        {
            _rockFactor = 1f;
            return;
        }

        // 只管 P1
        if (_character.PlayerID != "Player1")
        {
            _rockFactor = 1f;
            return;
        }

        float factor = 1f;
        Weapon current = _handle.CurrentWeapon;

        if (current != null)
        {
            if (!string.IsNullOrEmpty(_bigStoneID) && current.WeaponID == _bigStoneID)
            {
                factor = BigRockFactor;      // 大石头
            }
            else if (!string.IsNullOrEmpty(_smallStoneID) && current.WeaponID == _smallStoneID)
            {
                factor = SmallRockFactor;    // 小石头
            }
        }

        _rockFactor = factor;
    }

    /// <summary>
    /// 这里是关键：把石头减速“叠加”到 MovementSpeedMultiplier 上，而不是覆盖
    /// </summary>
    protected override void SetMovement()
    {
        // 先记住别人设好的 MovementSpeedMultiplier（墙体减速、技能减速等全在这里）
        float originalMultiplier = MovementSpeedMultiplier;

        // 在原来的基础上，再乘一个石头的倍率
        MovementSpeedMultiplier = originalMultiplier * _rockFactor;

        // 调用原版 CharacterMovement 的 SetMovement()，里面会用 MovementSpeed * MovementSpeedMultiplier * ContextSpeedMultiplier
        base.SetMovement();

        // 用完之后把 MovementSpeedMultiplier 还原，避免影响下一帧别的逻辑
        MovementSpeedMultiplier = originalMultiplier;
    }

    // ====== 只读键盘输入 ======
    protected override void HandleInput()
    {
        // 保留原本的输入开关逻辑
        if (ScriptDrivenInput)
        {
            return;
        }

        if (!InputAuthorized)
        {
            _horizontalMovement = 0f;
            _verticalMovement = 0f;
            return;
        }

        float h = 0f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) h -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) h += 1f;

        float v = 0f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) v -= 1f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) v += 1f;

        Vector2 input = new Vector2(h, v);
        if (input.sqrMagnitude > 1f)
        {
            input = input.normalized;
        }

        _horizontalMovement = input.x;
        _verticalMovement = input.y;
    }
}
