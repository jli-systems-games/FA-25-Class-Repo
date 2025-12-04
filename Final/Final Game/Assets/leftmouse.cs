using UnityEngine;
using MoreMountains.TopDownEngine; // 引用 TDE

public class DisableLeftClickAttack : MonoBehaviour
{
    [Header("配置")]
    [Tooltip("是否禁用射击 (通常是左键)")]
    public bool DisableShooting = true;

    [Tooltip("是否禁用交互 (通常是空格或左键)")]
    public bool DisableInteraction = true;

    [Tooltip("是否隐藏鼠标指针")]
    public bool HideCursor = false;

    void Start()
    {
        // 1. 禁用能力
        DisablePlayerAbilities();

        // 2. 如果需要，隐藏鼠标
        if (HideCursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void DisablePlayerAbilities()
    {
        // 找到场景里所有的 Character
        Character[] players = FindObjectsByType<Character>(FindObjectsSortMode.None);

        foreach (var p in players)
        {
            // 只处理玩家类型的角色，不处理敌人
            if (p.CharacterType == Character.CharacterTypes.Player)
            {
                // --- 禁用射击 (HandleWeapon) ---
                if (DisableShooting)
                {
                    // 获取持有武器的能力
                    CharacterHandleWeapon handleWeapon = p.FindAbility<CharacterHandleWeapon>();
                    if (handleWeapon != null)
                    {
                        // 这会让左键点击失效
                        handleWeapon.AbilityPermitted = false;
                        // 强制停止当前的射击状态（防止按住左键时进入场景导致一直连射）
                        handleWeapon.ForceStop();
                    }
                }

                // --- 禁用交互 (ButtonActivation) ---
                // 如果左键也用于捡东西或按开关，这个也要禁掉
                if (DisableInteraction)
                {
                    CharacterButtonActivation interaction = p.FindAbility<CharacterButtonActivation>();
                    if (interaction != null)
                    {
                        interaction.AbilityPermitted = false;
                    }
                }
            }
        }
    }

    // (可选) 离开场景时恢复，防止影响下一个场景
    void OnDestroy()
    {
        if (HideCursor)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        // 注意：通常不需要在 OnDestroy 恢复 AbilityPermitted
        // 因为下一个场景会加载新的 Character 实例（除非你的角色是 DontDestroyOnLoad 的）
        // 如果你的角色是跨场景存在的，你需要在这里把 AbilityPermitted 设回 true
    }
}