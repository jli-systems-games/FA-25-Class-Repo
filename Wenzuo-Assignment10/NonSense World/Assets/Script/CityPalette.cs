using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "City/City Palette")]
public class CityPalette : ScriptableObject
{
    [Header("Road Prefabs (5 types)")]
    public GameObject roadStraight;   // 直道
    public GameObject roadCorner;     // 转角
    public GameObject roadTJunction;  // 三岔
    public GameObject roadCross;      // 十字
    public GameObject roadDeadEnd;    // 死胡同

    [Header("Lot Prefabs (3 shells)")]
    public GameObject lotBuilding;
    public GameObject lotPark;
    public GameObject lotWater;

    [Header("Props SpawnTable (optional)")]
    public SpawnTable props;          // 给 LotDecorator.smallProps

    [Header("Rare Landmarks (optional)")]
    public List<GameObject> landmarks = new();

    [Header("Tile Settings")]
    [Tooltip("每个格子的世界单位尺寸（你的道路/地块预制体最好按这个尺寸建模）。")]
    public float tileSize = 1f;

    [Tooltip("实例化时给模型整体抬高/压低一点（地面厚度不一时很有用）。")]
    public float yOffset = 0f;
}
