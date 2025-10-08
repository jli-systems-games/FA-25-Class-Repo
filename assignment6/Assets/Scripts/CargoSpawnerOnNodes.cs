using UnityEngine;

[RequireComponent(typeof(TrackSegment2D))]
public class CargoSpawnerOnNodes : MonoBehaviour
{
    [Header("Cargo Prefab (Required)")]
    public GameObject cargoPrefab;

    [Header("Placement Rules")]
    [Tooltip("Target spacing between two cargo items (world units).")]
    public float spacing = 1.2f;

    [Tooltip("Probability (0–1) to place cargo at each candidate point (use <1.0 for sparse random).")]
    public float placeProbability = 1.0f;

    [Tooltip("Offset perpendicular to the path (positive/negative, 0 = centered on path).")]
    public float lateralOffset = 0f;

    [Tooltip("Keep this minimum clear distance from both ends of the path to avoid crowding junctions/ends.")]
    public float endMargin = 0.3f;

    [Header("Random Jitter (Optional)")]
    [Tooltip("Random jitter along the tangent direction (± value).")]
    public float jitterAlong = 0.0f;

    [Tooltip("Random jitter along the normal direction (± value).")]
    public float jitterNormal = 0.0f;

    [Header("Generation Mode")]
    [Tooltip("If true, only spawns during Play Mode (skip in edit-time previews).")]
    public bool spawnInEditorPlayModeOnly = true;

    private TrackSegment2D _seg;

    void Start()
    {
        if (spawnInEditorPlayModeOnly && !Application.isPlaying) return;

        _seg = GetComponent<TrackSegment2D>();
        if (!_seg || cargoPrefab == null) return;

        var nodes = _seg.GetNodePositions();
        if (nodes == null || nodes.Count < 2) return;

        float totalLen = 0f;
        for (int i = 1; i < nodes.Count; i++)
            totalLen += Vector3.Distance(nodes[i - 1], nodes[i]);

        float startS = Mathf.Max(0f, endMargin);
        float endS = Mathf.Max(0f, totalLen - endMargin);
        if (endS <= startS) return;

        float s = 0f;          
        int segIdx = 1;

        while (segIdx < nodes.Count)
        {
            Vector3 a = nodes[segIdx - 1];
            Vector3 b = nodes[segIdx];
            float len = Vector3.Distance(a, b);
            if (len <= Mathf.Epsilon)
            {
                segIdx++;
                continue;
            }

            Vector3 dir = (b - a).normalized;
            Vector3 normal = new Vector3(-dir.y, dir.x, 0f); 

            float nextS = s + len;

            while (true)
            {
                if (startS > s)
                {
                    float delta = Mathf.Min(nextS - s, startS - s);
                    s += delta;
                }

                float placeAt = Mathf.Ceil((s - startS) / spacing) * spacing + startS;
                if (placeAt < s) placeAt = s; 

                if (placeAt > nextS || placeAt > endS) break;

                float t = Mathf.InverseLerp(s, nextS, placeAt); 
                Vector3 posOnLine = Vector3.Lerp(a, b, t);

                float alongJ = (jitterAlong > 0f) ? Random.Range(-jitterAlong, jitterAlong) : 0f;
                float normJ = (jitterNormal > 0f) ? Random.Range(-jitterNormal, jitterNormal) : 0f;

                Vector3 finalPos = posOnLine
                                  + dir * alongJ
                                  + normal * (lateralOffset + normJ);

                if (Random.value <= placeProbability)
                {
                    Instantiate(cargoPrefab, finalPos, Quaternion.identity, this.transform);
                }

                s = placeAt + spacing;
                if (s > nextS) break;
            }

            s = nextS;
            segIdx++;
        }
    }

#if UNITY_EDITOR
    // Debug visualization: draw the node path when selected
    void OnDrawGizmosSelected()
    {
        var seg = GetComponent<TrackSegment2D>();
        if (!seg) return;
        var nodes = seg.GetNodePositions();
        if (nodes == null || nodes.Count < 2) return;

        Gizmos.color = new Color(1f, 0.85f, 0.2f, 0.9f);
        for (int i = 1; i < nodes.Count; i++)
        {
            Gizmos.DrawLine(nodes[i - 1], nodes[i]);
        }
    }
#endif
}
