using UnityEngine;

[CreateAssetMenu(fileName = "UnitStats", menuName = "BattleSim/UnitStats")]
public class UnitStats : ScriptableObject
{
    public Sprite unitIcon;
    public string unitName;
    public float maxHealth = 100f;
    public float damage = 10f;
    public float moveSpeed = 2f;
    public int cost = 20;
}
