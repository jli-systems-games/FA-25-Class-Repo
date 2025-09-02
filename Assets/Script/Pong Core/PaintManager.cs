using UnityEngine;

/// 把场地映射到 W×H 网格；沿球的轨迹盖“圆印”，并增量统计日/月占格数。
[RequireComponent(typeof(Renderer))]
public class PaintManager3D : MonoBehaviour
{
    [Header("网格尺寸（建议 128×72 起步）")]
    public int width = 128, height = 72;

    [Header("世界范围（XZ）")]
    public Vector2 xRange = new(-9f, 9f);
    public Vector2 zRange = new(-5f, 5f);

    [Header("可视化颜色")]
    public Color unpainted = new(0.12f, 0.12f, 0.12f);
    public Color dayColor = new(1.00f, 0.78f, 0.47f);
    public Color nightColor = new(0.47f, 0.71f, 1.00f);

    [Header("贴图刷新（每 N 帧 Apply 一次）")]
    public int applyEveryNFrames = 4;

    // -1=未染, 0=日, 1=月
    sbyte[,] owner;
    public int dayCount { get; private set; }
    public int nightCount { get; private set; }

    Texture2D tex;
    Color32[] pixels;
    float cellSizeX, cellSizeZ;
    int frame;

    void Awake()
    {
        owner = new sbyte[width, height];
        pixels = new Color32[width * height];
        cellSizeX = (xRange.y - xRange.x) / width;
        cellSizeZ = (zRange.y - zRange.x) / height;

        tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        GetComponent<Renderer>().material.mainTexture = tex;

        ResetGrid();
    }

    public void ResetGrid()
    {
        dayCount = nightCount = 0;
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                owner[x, y] = -1;
                pixels[x + y * width] = unpainted;
            }
        tex.SetPixels32(pixels); tex.Apply(false, false);
    }

    public void StampLineXZ(Vector3 a, Vector3 b, float radiusWorld, int team) // team: 0 日 / 1 月
    {
        // 越界全丢？——不，分段采样，自然会过滤出在场地内的点
        float dist = Vector3.Distance(a, b);
        float cell = Mathf.Min(cellSizeX, cellSizeZ);
        int steps = Mathf.Max(1, Mathf.CeilToInt(dist / cell * 1.2f));
        int rCells = Mathf.CeilToInt(radiusWorld / cell);

        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector3 p = Vector3.Lerp(a, b, t);
            WorldToCell(p, out int cx, out int cy);
            FillDisk(cx, cy, rCells, (sbyte)team);
        }

        // 节流刷新
        if (++frame % applyEveryNFrames == 0)
        { tex.SetPixels32(pixels); tex.Apply(false, false); }
    }

    void FillDisk(int cx, int cy, int r, sbyte team)
    {
        if (r <= 0) r = 1;
        int r2 = r * r;
        for (int y = -r; y <= r; y++)
            for (int x = -r; x <= r; x++)
            {
                if (x * x + y * y > r2) continue;
                int gx = cx + x, gy = cy + y;
                if (gx < 0 || gx >= width || gy < 0 || gy >= height) continue;

                sbyte prev = owner[gx, gy];
                if (prev == team) continue;

                // 计数增量
                if (prev == 0) dayCount--;
                else if (prev == 1) nightCount--;

                owner[gx, gy] = team;
                if (team == 0) { dayCount++; pixels[gx + gy * width] = dayColor; }
                else { nightCount++; pixels[gx + gy * width] = nightColor; }
            }
    }

    public void WorldToCell(Vector3 w, out int cx, out int cy)
    {
        cx = Mathf.FloorToInt((w.x - xRange.x) / (xRange.y - xRange.x) * width);
        cy = Mathf.FloorToInt((w.z - zRange.x) / (zRange.y - zRange.x) * height);
    }

    public (int day, int night) GetScores() => (dayCount, nightCount);
}