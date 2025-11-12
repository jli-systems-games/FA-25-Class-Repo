using UnityEngine;

public class WorldControllerAdapter : MonoBehaviour
{
    public int roomWidth = 14;
    public int roomHeight = 14;
    public float cellSize = 6.5f;
    public PrefabPalette palette;
    public int seed = 0;
    PlanarNonsenseBuilder planar;

    void Start() { GenerateWorld(); }

    public void GenerateWorld()
    {
        if (seed == 0) seed = System.Environment.TickCount;
        Random.InitState(seed);
        if (!planar)
            planar = FindObjectOfType<PlanarNonsenseBuilder>() ?? gameObject.AddComponent<PlanarNonsenseBuilder>();
        planar.palette = palette;
        planar.width = roomWidth;
        planar.height = roomHeight;
        planar.step = cellSize;
        planar.jitter = 0.25f;
        planar.roadLines = 8;
        planar.buildingFill = 0.32f;
        planar.Rebuild();
    }
}
