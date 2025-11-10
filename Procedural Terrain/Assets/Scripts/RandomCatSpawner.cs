using UnityEngine;

public class RandomCatSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject catPrefab;
    public Terrain terrain;
    public LayerMask environmentMask;

    [Header("Spawn Settings")]
    public float heightOffset = 0.3f;
    public int maxAttempts = 30;
    public float maxSlope = 30f;
    public float environmentCheckRadius = 0.8f;

    void Start()
    {
        SpawnCat();
    }

    void SpawnCat()
    {
        if (terrain == null || catPrefab == null)
        {
            Debug.LogWarning("❌ Missing Terrain or Cat Prefab reference!");
            return;
        }

        TerrainData td = terrain.terrainData;
        Vector3 terrainOrigin = terrain.GetPosition();

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            float x = Random.Range(0f, td.size.x);
            float z = Random.Range(0f, td.size.z);

            float worldX = terrainOrigin.x + x;
            float worldZ = terrainOrigin.z + z;
            float groundY = terrain.SampleHeight(new Vector3(worldX, 0f, worldZ))
                            + terrainOrigin.y
                            + heightOffset;

            float slope = td.GetSteepness(x / td.size.x, z / td.size.z);
            if (slope > maxSlope)
                continue;

            Vector3 spawnPos = new Vector3(worldX, groundY, worldZ);

            if (Physics.CheckSphere(spawnPos, environmentCheckRadius, environmentMask))
                continue;

            float randomY = Random.Range(0f, 360f);
            Quaternion rotation = Quaternion.Euler(-90f, randomY, 0f);

            GameObject cat = Instantiate(catPrefab, spawnPos, rotation);

            // Nudge so "feet" are on ground
            var rend = cat.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                float yOffset = rend.bounds.min.y - cat.transform.position.y;
                cat.transform.position -= Vector3.up * yOffset;
            }

            // --- CRUCIAL: Set up all dynamic references after spawning! ---

            // Set cat in GameManager
            GameManager gameM = FindObjectOfType<GameManager>();
            if (gameM != null) gameM.SetCat(cat.transform);

            // Set cat and spawn orbs in OrbSpawner
            OrbSpawner orbSp = FindObjectOfType<OrbSpawner>();
            if (orbSp != null)
            {
                orbSp.SetCat(cat);
                orbSp.SpawnOrbs();
            }

            // Set player in CatAudioController
            CatAudioController audioCtrl = cat.GetComponent<CatAudioController>();
            if (audioCtrl != null)
            {
                // Find player in scene (by tag or by name)
                var playerObj = GameObject.FindWithTag("Player") ?? GameObject.Find("Player");
                if (playerObj != null)
                    audioCtrl.SetPlayer(playerObj.transform);
            }

            Debug.Log($"🐈 Cat spawned at {cat.transform.position} after {attempt + 1} attempts");
            return;
        }

        Debug.LogWarning("⚠️ Failed to find valid spawn spot for cat after max attempts!");
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (terrain != null)
        {
            TerrainData td = terrain.terrainData;
            Vector3 origin = terrain.GetPosition();
            Gizmos.DrawWireCube(origin + td.size / 2f, td.size);
        }
    }
#endif
}