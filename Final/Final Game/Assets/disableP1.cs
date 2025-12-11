using UnityEngine;
using MoreMountains.TopDownEngine;

public class P1KeyboardMovement : CharacterMovement
{
    [Header("拖拽石头武器")]
    public Weapon BigStoneWeapon;      // 拖“大石头 Weapon”进来
    public Weapon SmallStoneWeapon;    // 拖“小石头 Weapon”进来

    [Header("速度倍率")]
    [Range(0f, 1f)] public float BigRockFactor = 0.3f;
    [Range(0f, 1f)] public float SmallRockFactor = 0.6f;

    private CharacterHandleWeapon _handle;
    private string _bigStoneID;
    private string _smallStoneID;

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
        base.ProcessAbility();
        ApplyRockSlowdown();
    }

    void ApplyRockSlowdown()
    {
        if (_character == null || _handle == null)
            return;

        // 只管 P1
        if (_character.PlayerID != "Player1")
            return;

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

        // 用 MovementSpeedMultiplier，不会破坏 WalkSpeed 本身
        MovementSpeedMultiplier = factor;
    }

    // ====== 只读键盘输入 ======
    protected override void HandleInput()
    {
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
