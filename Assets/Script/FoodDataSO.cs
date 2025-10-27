using UnityEngine;

[CreateAssetMenu(menuName="FoodData", fileName="FoodData")]
public class FoodDataSO : ScriptableObject
{
    public string displayName;
    public int deltaHappy = 0;
    public int deltaHunger = 0;
    public int deltaHealth   = 0;
    public float fallSpeed = 6f;
}