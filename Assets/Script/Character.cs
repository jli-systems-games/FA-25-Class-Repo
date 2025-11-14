using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("角色信息")]
    public string characterName = "角色";

    private bool isAlive = true;

    public bool IsAlive => isAlive;

    public void Die()
    {
        isAlive = false;
        Debug.Log($"💀 {characterName} 死亡！");
    }
}
