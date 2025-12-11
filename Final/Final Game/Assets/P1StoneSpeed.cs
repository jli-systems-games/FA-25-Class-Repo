using UnityEngine;
using MoreMountains.TopDownEngine;

/// <summary>
/// 只给 P1 用的「拿石头减速」脚本：
/// - 不改输入逻辑，输入还是交给 P1KeyboardMovement
/// - 只根据当前拿的 Weapon（石头）去改 CharacterMovement.MovementSpeedMultiplier
/// - 大石头：原速 * BigStoneMultiplier
/// - 小石头：原速 * SmallStoneMultiplier
/// </summary>
public class P1StoneSpeedLimiter : MonoBehaviour
{
    [Header("引用（拖到 Inspector）")]
    [Tooltip("P1 身上负责移动的 CharacterMovement（比如你的 P1KeyboardMovement）")]
    public CharacterMovement Movement;          // 拖 P1 身上的 P1KeyboardMovement 进来

    [Tooltip("小石头 Weapon（拖 Weapon prefab 或场景中的 Weapon 带组件的对象）")]
    public Weapon SmallStoneWeaponTemplate;     // 拖「小石头武器」进来

    [Tooltip("大石头 Weapon（拖 Weapon prefab 或场景中的 Weapon 带组件的对象）")]
    public Weapon BigStoneWeaponTemplate;       // 拖「大石头武器」进来

    [Header("速度倍率")]
    [Tooltip("拿小石头时速度倍率")]
    public float SmallStoneMultiplier = 0.6f;

    [Tooltip("拿大石头时速度倍率")]
    public float BigStoneMultiplier = 0.3f;

    [Tooltip("没拿石头时速度倍率")]
    public float DefaultMultiplier = 1f;

    // --- 内部缓存 ---
    private Character _character;
    private CharacterHandleWeapon _handleWeapon;

    // 用名字做匹配（因为运行时实例和 prefab 不是同一个引用）
    private string _smallStoneWeaponName;
    private string _bigStoneWeaponName;

    private void Awake()
    {
        _character = GetComponent<Character>();
        _handleWeapon = GetComponent<CharacterHandleWeapon>();

        if (Movement == null)
        {
            // 兜底：如果没拖，就自动找身上的 CharacterMovement（比如 P1KeyboardMovement）
            Movement = GetComponent<CharacterMovement>();
        }

        if (SmallStoneWeaponTemplate != null)
        {
            _smallStoneWeaponName = SmallStoneWeaponTemplate.WeaponName;
        }

        if (BigStoneWeaponTemplate != null)
        {
            _bigStoneWeaponName = BigStoneWeaponTemplate.WeaponName;
        }
    }

    private void Update()
    {
        // 1. 没组件直接退出
        if (Movement == null || _character == null)
        {
            return;
        }

        // 2. 只对 Player1 生效
        if (_character.PlayerID != "Player1")
        {
            return;
        }

        // 3. 如果没有拿武器，直接恢复正常速度
        if (_handleWeapon == null || _handleWeapon.CurrentWeapon == null)
        {
            Movement.MovementSpeedMultiplier = DefaultMultiplier;
            return;
        }

        Weapon current = _handleWeapon.CurrentWeapon;
        string currentName = current.WeaponName;

        // 4. 根据当前拿的武器名字决定倍率
        float targetMultiplier = DefaultMultiplier;

        if (!string.IsNullOrEmpty(_bigStoneWeaponName) && currentName == _bigStoneWeaponName)
        {
            // 大石头
            targetMultiplier = BigStoneMultiplier;
        }
        else if (!string.IsNullOrEmpty(_smallStoneWeaponName) && currentName == _smallStoneWeaponName)
        {
            // 小石头
            targetMultiplier = SmallStoneMultiplier;
        }

        // 5. 应用到 TDE 的速度系统
        // CharacterMovement.SetMovement() 里面会用 MovementSpeed * MovementSpeedMultiplier * ContextSpeedMultiplier
        Movement.MovementSpeedMultiplier = targetMultiplier;
    }
}
