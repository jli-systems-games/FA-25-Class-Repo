using UnityEngine;

public class CarStats : MonoBehaviour
{
    [Header("Physics")]
    public float mass = 1200f;

    [Header("Move")]
    public float accel = 80f;
    public float maxSpeed = 40f;
    public float turnSpeed = 160f;

    [Header("Hit Reaction")]
    public float hitImpulse = 12f;
}
