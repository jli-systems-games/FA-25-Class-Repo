using UnityEngine;
using System.Collections.Generic;

public class BeatSpawner : MonoBehaviour
{
    public DriveSmoothGrounded driver;

    [Header("Prefabs")]
    public GameObject[] tumbleweedPrefabs; // 风滚草
    public GameObject[] signPrefabs;       // 路牌/旗杆等

    [Header("距离扇区")]
    public float innerRadius = 10f; // 内圈禁止生成（解决贴脸/进车内）
    public float minDist = 18f;     // 普通物体外圈最小距离
    public float maxDist = 28f;
    public float arcDeg = 70f;      // 只在前方 ±arcDeg/2

    [Header("风滚草专用距离（比路牌更远）")]
    public float weedMinDist = 22f;
    public float weedMaxDist = 34f;

    [Header("控制")]
    public int maxAlive = 14;
    public float beatInterval = 1.0f;
    public LayerMask groundMask = ~0;

    float nextBeat;
    readonly List<GameObject> pool = new();

    void Update()
    {
        if (!driver) return;
        if (Time.time < nextBeat) return;
        nextBeat = Time.time + beatInterval;

        pool.RemoveAll(go => go == null);
        if (pool.Count >= maxAlive) return;

        // 随机选择类型：路牌/风滚草
        bool spawnWeed = Random.value < 0.5f;
        var set = spawnWeed ? tumbleweedPrefabs : signPrefabs;
        if (set == null || set.Length == 0) return;

        var cam = driver.cam;
        Vector3 fwd = cam.transform.forward; fwd.y = 0; fwd.Normalize();
        Vector3 rgt = cam.transform.right; rgt.y = 0; rgt.Normalize();

        float distMin = spawnWeed ? weedMinDist : minDist;
        float distMax = spawnWeed ? weedMaxDist : maxDist;

        // 距离随机，但保证 > innerRadius
        float dist = Mathf.Max(Random.Range(distMin, distMax), innerRadius + 0.5f);

        float ang = Random.Range(-arcDeg * 0.5f, arcDeg * 0.5f) * Mathf.Deg2Rad;
        Vector3 dir = (fwd * Mathf.Cos(ang) + rgt * Mathf.Sin(ang)).normalized;

        // 初始点：前方远处+抬高，随后往下 Raycast 落地
        Vector3 pos = cam.transform.position + dir * dist + Vector3.up * 12f;
        if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, 60f, groundMask, QueryTriggerInteraction.Ignore))
            pos = hit.point + Vector3.up * 0.05f;

        var prefab = set[Random.Range(0, set.Length)];
        var go = Instantiate(prefab, pos, Quaternion.identity);
        // 让它面对行进方向或相机
        go.transform.LookAt(cam.transform.position, Vector3.up);

        pool.Add(go);
        StartCoroutine(DespawnWhenFar(go, cam.transform, distMax));
    }

    System.Collections.IEnumerator DespawnWhenFar(GameObject go, Transform cam, float far)
    {
        while (go && Vector3.Distance(go.transform.position, cam.position) < far * 1.8f)
            yield return null;
        if (go) Destroy(go);
    }
}
