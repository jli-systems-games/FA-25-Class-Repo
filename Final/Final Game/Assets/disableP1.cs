using UnityEngine;
using MoreMountains.TopDownEngine;

public class P1KeyboardMovement : CharacterMovement
{
    [Header("拖拽石头武器")]
    public Weapon BigStoneWeapon;
    public Weapon SmallStoneWeapon;

    [Header("速度倍率")]
    [Range(0f, 1f)] public float BigRockFactor = 0.3f;
    [Range(0f, 1f)] public float SmallRockFactor = 0.6f;

    [Header("状态读取")]
    public CharacterMovement StateMovement;

    private CharacterHandleWeapon _handle;
    private string _bigStoneID;
    private string _smallStoneID;

    protected override void Initialization()
    {
        base.Initialization();
        _handle = GetComponent<CharacterHandleWeapon>();
        CacheWeaponIDs();

        if (StateMovement == null)
        {
            var allMovements = GetComponents<CharacterMovement>();
            foreach (var mv in allMovements)
            {
                if (mv != this && mv.GetType() == typeof(CharacterMovement))
                {
                    StateMovement = mv;
                    break;
                }
            }
        }

        if (StateMovement != null)
        {
            StateMovement.MovementSpeed = 0f;  // ✅ 只设置这个就够了
            StateMovement.enabled = true;
        }
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
        SyncStateFromOtherComponent();
    }

    void ApplyRockSlowdown()
    {
        if (_character == null || _handle == null)
            return;

        if (_character.PlayerID != "Player1")
            return;

        float factor = 1f;
        Weapon current = _handle.CurrentWeapon;

        if (current != null)
        {
            if (!string.IsNullOrEmpty(_bigStoneID) && current.WeaponID == _bigStoneID)
            {
                factor = BigRockFactor;
            }
            else if (!string.IsNullOrEmpty(_smallStoneID) && current.WeaponID == _smallStoneID)
            {
                factor = SmallRockFactor;
            }
        }

        MovementSpeedMultiplier = factor;
    }

    void SyncStateFromOtherComponent()
    {
        if (StateMovement == null || StateMovement == this)
            return;

        StateMovement.SetHorizontalMovement(_horizontalMovement);
        StateMovement.SetVerticalMovement(_verticalMovement);

        MovementSpeedMultiplier *= StateMovement.MovementSpeedMultiplier;
    }

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