using System.Collections.Generic;
using UnityEngine;

public static class Data
{
    public static VehicleDef vehicle = new VehicleDef();
    public static LibraryDef lib = new LibraryDef();
    public static void ResetVehicle() { vehicle = new VehicleDef(); }
}

public enum FrameType { Car, Bike }
public enum WheelColliderMode { Polygon, Circle, Box }

[System.Serializable]
public struct WheelItemDef
{
    public Sprite sprite;
    public WheelColliderMode colliderMode;
    public float scale;
    public float baseFriction;
    public bool isDrive;
    public float targetAngularSpeed;
    public float maxTorque;
    public float grip;
    public float angularDrag;
    public AudioClip selectClip;
    public AudioClip runClip;
}

[System.Serializable]
public struct VehicleDef
{
    public FrameType frameType;
    public float bodyMass;
    public Vector2 centerOfMassOffset;
    public List<WheelDef> wheels;
}

[System.Serializable]
public struct WheelDef
{
    public Vector2 localPos;
    public int itemIndex;
    public float scale;
}

[System.Serializable]
public struct LibraryDef
{
    public Sprite bodyCarSprite;
    public Sprite bodyBikeSprite;
    public List<WheelItemDef> wheelItems;
}
