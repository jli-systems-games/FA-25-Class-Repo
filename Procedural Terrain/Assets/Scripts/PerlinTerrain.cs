using UnityEngine;

public class PerlinTerrain : MonoBehaviour
{
    [Header("Terrain Settings")]
    public Terrain terrain;
    public float heightScale = 0.2f;
    public float detailScale = 5f;

    [Header("Environment Prefabs")]
    public GameObject treePrefab;
    public GameObject rockPrefab;
    public GameObject cactusPrefab;
    public GameObject crystalPrefab;
    public GameObject bushPrefab;

    private int biomeType; 
    private Transform environmentParent;

    void Start()
    {
        biomeType = RandomizeBiome();      
        GenerateTerrain();
        environmentParent = new GameObject("EnvironmentRoot").transform;
        SpawnEnvironment();
        RandomizeLighting();
    }

   
    void GenerateTerrain()
    {
        if (terrain == null)
        {
            Debug.LogWarning("Terrain not assigned!");
            return;
        }

        TerrainData td = terrain.terrainData;
        int width = td.heightmapResolution;
        int height = td.heightmapResolution;

        float[,] heights = new float[width, height];
        float offsetX = Random.Range(0f, 9999f);
        float offsetY = Random.Range(0f, 9999f);

        // Randomize hill shapes each run
        heightScale = Random.Range(0.1f, 0.4f);
        detailScale = Random.Range(3f, 10f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (x / (float)width) * detailScale + offsetX;
                float yCoord = (y / (float)height) * detailScale + offsetY;
                heights[x, y] = Mathf.PerlinNoise(xCoord, yCoord) * heightScale;
            }
        }

        td.SetHeights(0, 0, heights);
    }

   
    int RandomizeBiome()
    {
        int biome = Random.Range(0, 4);
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));

        switch (biome)
        {
            case 0: 
                mat.color = new Color(0.25f, 0.5f, 0.25f);
                RenderSettings.fogColor = new Color(0.3f, 0.5f, 0.3f);
                RenderSettings.fogDensity = 0.004f;
                break;

            case 1: 
                mat.color = new Color(0.9f, 0.8f, 0.5f);
                RenderSettings.fogColor = new Color(0.8f, 0.7f, 0.5f);
                RenderSettings.fogDensity = 0.002f;
                break;

            case 2: 
                mat.color = new Color(0.9f, 0.9f, 1f);
                RenderSettings.fogColor = new Color(0.7f, 0.8f, 0.9f);
                RenderSettings.fogDensity = 0.005f;
                break;

            case 3: 
                mat.color = new Color(0.45f, 0.45f, 0.45f);
                RenderSettings.fogColor = new Color(0.4f, 0.4f, 0.45f);
                RenderSettings.fogDensity = 0.006f;
                break;
        }

        terrain.materialTemplate = mat;
        return biome;
    }

    // ------------------------------
    // 3️⃣ ENVIRONMENT OBJECT SPAWNING
    // ------------------------------
    void SpawnEnvironment()
    {
        if (terrain == null) return;

        TerrainData td = terrain.terrainData;
        int objectCount = Random.Range(30, 60); 

        for (int i = 0; i < objectCount; i++)
        {
            float x = Random.Range(0f, td.size.x);
            float z = Random.Range(0f, td.size.z);
            Vector3 pos = new Vector3(x, 0, z);

            
            Vector3 normal = td.GetInterpolatedNormal(x / td.size.x, z / td.size.z);
            if (Vector3.Angle(normal, Vector3.up) > 30f) continue;

            float y = terrain.SampleHeight(pos) + terrain.transform.position.y;
            Vector3 position = new Vector3(x, y, z);
            Quaternion rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            switch (biomeType)
            {
                case 0: 
                    if (Random.value < 0.7f && treePrefab != null)
                        Instantiate(treePrefab, position, rotation, environmentParent);
                    else if (bushPrefab != null)
                        Instantiate(bushPrefab, position, rotation, environmentParent);
                    break;

                case 1:
                    if (Random.value < 0.7f && cactusPrefab != null)
                        Instantiate(cactusPrefab, position, rotation, environmentParent);
                    else if (rockPrefab != null)
                        Instantiate(rockPrefab, position, rotation, environmentParent);
                    break;

                case 2: 
                    if (Random.value < 0.6f && crystalPrefab != null)
                        Instantiate(crystalPrefab, position, rotation, environmentParent);
                    else if (rockPrefab != null)
                        Instantiate(rockPrefab, position, rotation, environmentParent);
                    break;

                case 3: 
                    if (Random.value < 0.5f && rockPrefab != null)
                        Instantiate(rockPrefab, position, rotation, environmentParent);
                    else if (treePrefab != null)
                        Instantiate(treePrefab, position, rotation, environmentParent);
                    break;
            }
        }
    }

   
    // LIGHTING VARIATION
   
    void RandomizeLighting()
    {
        Light sun = RenderSettings.sun ?? FindObjectOfType<Light>();
        if (sun == null) return;

        switch (biomeType)
        {
            case 0: // Forest
                sun.color = new Color(1f, 0.95f, 0.85f);
                sun.intensity = 0.9f;
                break;

            case 1: // Desert
                sun.color = new Color(1f, 0.95f, 0.7f);
                sun.intensity = 1.2f;
                break;

            case 2: // Snow
                sun.color = new Color(0.8f, 0.9f, 1f);
                sun.intensity = 0.8f;
                break;

            case 3: // Hills
                sun.color = new Color(1f, 0.85f, 0.8f);
                sun.intensity = 1f;
                break;
        }
    }
}