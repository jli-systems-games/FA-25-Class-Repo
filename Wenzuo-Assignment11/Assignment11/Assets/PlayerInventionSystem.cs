using UnityEngine;

public class PlayerInterventionSystem : MonoBehaviour
{
    public GameObject bombPrefab;
    public GameObject fanPrefab;
    public float cooldown = 0.6f;
    public Vector2 extent = new Vector2(18f, 18f);
    public Vector3 offset = Vector3.up * 2f;
    public Vector3 platformCenter;
    float nextTime;

    void Update()
    {
        if (Time.time < nextTime) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bool fan = Random.value > 0.5f;
            Spawn(fan ? fanPrefab : bombPrefab);
            nextTime = Time.time + cooldown;
        }
    }

    void Spawn(GameObject prefab)
    {
        if (prefab == null) return;
        Vector2 r = new Vector2(Random.Range(-extent.x, extent.x), Random.Range(-extent.y, extent.y));
        Vector3 pos = new Vector3(platformCenter.x + r.x, platformCenter.y, platformCenter.z + r.y) + offset;
        Quaternion rot = fanPrefab == prefab ? Quaternion.Euler(0f, Random.Range(0f, 360f), 0f) : Quaternion.identity;
        Instantiate(prefab, pos, rot);
    }
}
