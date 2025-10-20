using UnityEngine;

[DisallowMultipleComponent]
public class BlockTag : MonoBehaviour
{
    public string id = "cube_1x1";
    public Vector3Int size = Vector3Int.one;
    public Vector3 pivotOffset = Vector3.zero;
}