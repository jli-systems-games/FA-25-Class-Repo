using UnityEngine;
using System;

public static class NoiseSystem
{
    public static event Action<Vector3, float> OnNoise;
    public static void Broadcast(Vector3 pos, float radius) => OnNoise?.Invoke(pos, radius);
}
