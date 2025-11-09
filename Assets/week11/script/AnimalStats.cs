using UnityEngine;

[CreateAssetMenu(menuName = "Battle/AnimalStats")]
public class AnimalStats : ScriptableObject
{
    public string displayName;
    public Sprite sprite;

    [Header("Prefab (per animal)")]
    public AnimalController prefab;

    [Header("Base")]
    public int maxHP = 50;
    public float moveImpulse = 5f;
    public float impulseInterval = 1.2f;

    [Header("Chances 0~1")]
    public float attackChance = 0.5f;  // 충돌 시 공격 시도
    public float evadeChance = 0.2f;  // 회피(완전 무효)
    public float blockChance = 0.2f;  // 방어(완전 무효)
    public float critChance = 0.1f;  // 치명타(2배)
    public float counterChance = 0.0f;  // 카운터(맞은 직후 1회 반격)

    [Header("Damage")]
    public int baseDamage = 8;
}
