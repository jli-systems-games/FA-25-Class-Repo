using UnityEngine;

[CreateAssetMenu(fileName = "CarSpec", menuName = "Scriptable Objects/CarSpec")]
public class CarSpec : ScriptableObject
{
    public string displayName;
    public GameObject prefab;
    public Vector3 rotationOffset;

    public float topSpeed_kmh;
    public float zeroTo100_s;
    public float brake_100to0_m;
    public float handling_rating;
    public float grip_rating;
    public float downforce_rating;
    public float weight_kg;
}