using UnityEngine;

[DisallowMultipleComponent]
public class BlockTag : MonoBehaviour
{
    [Header("方块标识（用于保存/统计）")]
    public string id = "cube_1x1";

    [Header("占用格子尺寸（单位：格）")]
    public Vector3Int size = Vector3Int.one;

    [Header("可选：放置时的可视化中心偏移（常用0）")]
    public Vector3 pivotOffset = Vector3.zero;
}