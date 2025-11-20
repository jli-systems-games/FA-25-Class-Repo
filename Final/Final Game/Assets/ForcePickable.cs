using UnityEngine;
using MoreMountains.TopDownEngine;

public class SimpleStoneGiver : MonoBehaviour
{
    [Header("配置")]
    public Weapon WeaponToEquip; // 拖入 Item_SmallStone 或 Item_BigStone

    // 用来记录当前谁站在圈里
    private Character _playerInRange;

    private void OnTriggerEnter(Collider other)
    {
        // 尝试获取角色组件 (包括父物体，防止子物体碰撞导致获取失败)
        Character character = other.GetComponentInParent<Character>();

        if (character != null)
        {
            _playerInRange = character;
            Debug.Log("玩家进入感应区: " + character.PlayerID);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Character character = other.GetComponentInParent<Character>();
        if (character != null && _playerInRange == character)
        {
            _playerInRange = null;
            Debug.Log("玩家离开感应区");
        }
    }

    private void Update()
    {
        // 如果没人站在圈里，啥都不做
        if (_playerInRange == null) return;

        bool verifyInput = false;

        // P1 按 F
        if (_playerInRange.PlayerID == "Player1" && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("P1 按下了 F");
            verifyInput = true;
        }
        // P2 按 回车
        else if (_playerInRange.PlayerID == "Player2" && Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("P2 按下了回车");
            verifyInput = true;
        }

        // 执行给武器
        if (verifyInput)
        {
            GiveWeapon();
        }
    }

    void GiveWeapon()
    {
        if (WeaponToEquip == null)
        {
            Debug.LogError("这就是原因！你忘记把 Item_SmallStone 拖进 Inspector 槽里了！");
            return;
        }

        // 查找角色身上的“拿武器”能力
        CharacterHandleWeapon handleWeapon = _playerInRange.FindAbility<CharacterHandleWeapon>();

        if (handleWeapon != null)
        {
            // 强制换武器
            handleWeapon.ChangeWeapon(WeaponToEquip, "Stone");
            Debug.Log("成功给 " + _playerInRange.PlayerID + " 装备了 " + WeaponToEquip.name);
        }
        else
        {
            Debug.LogError("角色身上没有 CharacterHandleWeapon 组件！无法拿东西。");
        }
    }
}