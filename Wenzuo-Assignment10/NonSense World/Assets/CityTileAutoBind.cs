using UnityEngine;

[DisallowMultipleComponent]
public class CityTileAutoBind : MonoBehaviour
{
    public CityPalette palette;
    CityTile tile;

    void Reset() { tile = GetComponent<CityTile>(); }
    void Awake()
    {
        if (!tile) tile = GetComponent<CityTile>();
        if (!tile || !palette) return;

        // 把 palette 的路/地块 Prefab 注入 CityTile 引用
        tile.roadStraight = palette.roadStraight;
        tile.roadCorner = palette.roadCorner;
        tile.roadTJunction = palette.roadTJunction;
        tile.roadCross = palette.roadCross;
        tile.roadDeadEnd = palette.roadDeadEnd;

        tile.lotBuilding = palette.lotBuilding;
        tile.lotPark = palette.lotPark;
        tile.lotWater = palette.lotWater;

        // 如果 LotDecorator 存在，就把 props 也注入
    

        // 统一的高度微调（给所有子物体一个 y 偏移）
        if (Mathf.Abs(palette.yOffset) > 0.0001f)
        {
            var t = transform;
            t.position = new Vector3(t.position.x, t.position.y + palette.yOffset, t.position.z);
        }
    }
}
