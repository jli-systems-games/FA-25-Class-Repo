using UnityEngine;
using MoreMountains.TopDownEngine;

public class StonePickup : MonoBehaviour
{
    [Header("要给的石头武器 (预制体)")]
    public Weapon WeaponToEquip; // 拖入 Item_SmallStone 或 Item_BigStone

    private void OnTriggerStay(Collider other)
    {
        // 1. 获取角色组件
        Character character = other.GetComponent<Character>();
        if (character == null) return;

        bool pickupInput = false;

        // 2. 只有对应的玩家按下对应的键，才算“拾取”
        if (character.PlayerID == "Player1")
        {
            if (Input.GetKeyDown(KeyCode.F)) pickupInput = true;
        }
        else if (character.PlayerID == "Player2")
        {
            // 注意：P2 需要开启输入或者我们直接检测键盘
            if (Input.GetKeyDown(KeyCode.Return)) pickupInput = true;
        }

        // 3. 执行换武器
        if (pickupInput)
        {
            CharacterHandleWeapon handleWeapon = character.FindAbility<CharacterHandleWeapon>();
            if (handleWeapon != null)
            {
                // 换上新武器 (ID随意填)
                handleWeapon.ChangeWeapon(WeaponToEquip, "Stone");
                Debug.Log(character.PlayerID + " 捡起了 " + WeaponToEquip.name);
            }
        }
    }
}