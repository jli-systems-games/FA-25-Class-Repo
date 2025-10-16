using UnityEngine;
using System.Collections.Generic;

public class Data
{
    public static VehicleData vehicle;       
    public static Library lib = new Library(); 

    public static void ResetVehicle()
    {
        vehicle = new VehicleData();
    }
}

[System.Serializable]
public struct VehicleData
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
public class Library
{
    public Sprite bodyCarSprite;
    public Sprite bodyBikeSprite;
    public List<WheelItemDef> wheelItems = new List<WheelItemDef>();
}

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

public enum FrameType { Car, Bike }
public enum WheelColliderMode { Box, Circle, Polygon }
