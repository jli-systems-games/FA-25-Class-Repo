using UnityEngine;
using System.Collections.Generic;

public class TrackGenerator2D_Pos : MonoBehaviour
{
    [Header("Refs")]
    public Train2DController_Pos train;

    [Header("Prefabs")]
    public TrackSegment2D straightPrefab;
    public TrackSegment2D curveLeftPrefab;
    public TrackSegment2D curveRightPrefab;
    public TrackSegment2D junctionPrefab;

    [Header("Generation")]
    public int initialSegments = 3;
    public int maxLiveSegments = 8;
    public float spawnAheadDistance = 10f;
    [Range(0f, 1f)]
    public float junctionChance = 0.35f;

    [Header("Obstacles (spawn at segment exit)")]
    public GameObject obstaclePrefab;              
    [Range(0f, 1f)] public float obstacleChance = 0.15f; 
    [Tooltip("denhsity")]
    public int obstacleCooldownSegments = 2;
    [Tooltip("a bit right")]
    public float obstacleForwardOffset = 0.35f;
    [Tooltip("bit up")]
    public float obstacleLateralOffset = 0f;
    [Tooltip("put obstacle?")]
    public bool obstaclesOnJunctionBranches = false;

    private readonly List<TrackSegment2D> live = new List<TrackSegment2D>();

    private Vector3 _nextAttachPos;
    private Quaternion _nextAttachRot = Quaternion.identity;

    private TrackSwitch2D_Pos _lastSwitch;
    private Vector3 _leftEndPos, _rightEndPos;
    private Quaternion _leftEndRot, _rightEndRot;

    private int _obstacleCooldown = 0;

    bool Alive(Object o) => o != null;

    static void PoseFrom(Transform t, out Vector3 pos, out Quaternion rot)
    {
        if (t) { pos = t.position; rot = t.rotation; }
        else { pos = Vector3.zero; rot = Quaternion.identity; }
    }

    TrackSegment2D SpawnAtPoseAlignEntry(TrackSegment2D prefab, Vector3 pos, Quaternion rot)
    {
        if (!prefab) return null;

        var inst = Instantiate(prefab);
        var entry = inst.entryAnchor ? inst.entryAnchor : inst.transform;

        Quaternion deltaRot = rot * Quaternion.Inverse(entry.rotation);
        inst.transform.rotation = deltaRot * inst.transform.rotation;

        Vector3 deltaPos = pos - entry.position;
        inst.transform.position += deltaPos;

        return inst;
    }

    void UpdateNextAttachFromSegment(TrackSegment2D seg)
    {
        if (!Alive(seg)) { _nextAttachPos = Vector3.zero; _nextAttachRot = Quaternion.identity; return; }
        Transform end = seg.isJunction
            ? (seg.rightAnchor ? seg.rightAnchor : seg.leftAnchor)
            : (seg.exitAnchor ? seg.exitAnchor : seg.transform);

        PoseFrom(end, out _nextAttachPos, out _nextAttachRot);
    }

    void Start()
    {
        if (!train)
        {
            Debug.LogError("[TrackGenerator2D_Pos] Train not set.");
            enabled = false; return;
        }

        if (!junctionPrefab) junctionChance = 0f;

        var startSeg = FindObjectOfType<TrackSegment2D>();
        if (!startSeg) startSeg = Instantiate(straightPrefab, Vector3.zero, Quaternion.identity);

        live.Add(startSeg);
        UpdateNextAttachFromSegment(startSeg);
        train.SetNewPathPositions(startSeg.GetNodePositions(), 0);

        for (int i = 0; i < initialSegments - 1; i++)
            SpawnNextAtCurrentPose();
    }

    void Update()
    {
        if (_lastSwitch)
        {
            if (_lastSwitch.usingLeft) { _nextAttachPos = _leftEndPos; _nextAttachRot = _leftEndRot; }
            else { _nextAttachPos = _rightEndPos; _nextAttachRot = _rightEndRot; }
        }

        var last = live.Count > 0 ? live[live.Count - 1] : null;
        if (Alive(last))
        {
            Transform end = last.isJunction
                ? (last.rightAnchor ? last.rightAnchor : last.leftAnchor)
                : (last.exitAnchor ? last.exitAnchor : last.transform);

            if (end)
            {
                float dist = Vector2.Distance(train.transform.position, end.position);
                if (dist < spawnAheadDistance) SpawnNextAtCurrentPose();
            }
            else
            {
                SpawnNextAtCurrentPose(); 
            }
        }

        if (live.Count > maxLiveSegments)
        {
            var oldest = live[0];
            live.RemoveAt(0);
            if (Alive(oldest)) Destroy(oldest.gameObject);
        }
    }

    void SpawnNextAtCurrentPose()
    {
        TrackSegment2D prefabToUse =
            (junctionPrefab && Random.value < junctionChance)
            ? junctionPrefab
            : RandomStraightOrCurve();

        var seg = SpawnAtPoseAlignEntry(prefabToUse, _nextAttachPos, _nextAttachRot);
        if (!Alive(seg)) return;

        live.Add(seg);

        if (seg.isJunction)
        {
            var leftNext = SpawnAtPoseAlignEntry(RandomStraightOrCurve(), seg.leftAnchor.position, seg.leftAnchor.rotation);
            var rightNext = SpawnAtPoseAlignEntry(RandomStraightOrCurve(), seg.rightAnchor.position, seg.rightAnchor.rotation);

            if (Alive(leftNext)) live.Add(leftNext);
            if (Alive(rightNext)) live.Add(rightNext);

            var leftPath = ConcatPositions(seg.GetNodePositions(), Alive(leftNext) ? leftNext.GetNodePositions() : null);
            var rightPath = ConcatPositions(seg.GetNodePositions(), Alive(rightNext) ? rightNext.GetNodePositions() : null);

            var sw = seg.GetComponent<TrackSwitch2D_Pos>();
            if (Alive(sw))
            {
                sw.train = train;
                sw.leftPathPositions = leftPath;
                sw.rightPathPositions = rightPath;
                sw.usingLeft = true; 
            }
            _lastSwitch = sw;

            if (Alive(leftNext))
                PoseFrom(leftNext.exitAnchor ? leftNext.exitAnchor : leftNext.transform, out _leftEndPos, out _leftEndRot);
            if (Alive(rightNext))
                PoseFrom(rightNext.exitAnchor ? rightNext.exitAnchor : rightNext.transform, out _rightEndPos, out _rightEndRot);

            if (Alive(leftNext)) train.AppendPositions(leftNext.GetNodePositions());

            _nextAttachPos = _leftEndPos;
            _nextAttachRot = _leftEndRot;

            if (obstaclesOnJunctionBranches && obstaclePrefab)
            {
                if (Alive(leftNext)) TrySpawnObstacleAtExit(leftNext);
                if (Alive(rightNext)) TrySpawnObstacleAtExit(rightNext);
            }
        }
        else
        {
            train.AppendPositions(seg.GetNodePositions());
            UpdateNextAttachFromSegment(seg);

            TrySpawnObstacleAtExit(seg);
        }

        if (_obstacleCooldown > 0) _obstacleCooldown--;
    }

    void TrySpawnObstacleAtExit(TrackSegment2D seg)
    {
        if (!obstaclePrefab) return;
        if (_obstacleCooldown > 0) return;        
        if (Random.value > obstacleChance) return; 

        Transform exit = seg.exitAnchor ? seg.exitAnchor : seg.transform;

        Vector3 pos = exit.position
                    + exit.right * obstacleForwardOffset
                    + exit.up * obstacleLateralOffset;

        Quaternion rot = exit.rotation;

        var obs = Instantiate(obstaclePrefab, pos, rot, seg.transform);

        _obstacleCooldown = Mathf.Max(0, obstacleCooldownSegments);
    }

    TrackSegment2D RandomStraightOrCurve()
    {
        var pool = new List<TrackSegment2D>(3);
        if (straightPrefab) pool.Add(straightPrefab);
        if (curveLeftPrefab) pool.Add(curveLeftPrefab);
        if (curveRightPrefab) pool.Add(curveRightPrefab);

        if (pool.Count == 0)
        {
            Debug.LogError("[TrackGenerator2D_Pos] No straight/curve prefabs assigned.");
            return null;
        }
        return pool[Random.Range(0, pool.Count)];
    }

    static List<Vector3> ConcatPositions(List<Vector3> a, List<Vector3> b)
    {
        var list = new List<Vector3>((a?.Count ?? 0) + (b?.Count ?? 0));
        if (a != null) list.AddRange(a);
        if (b != null) list.AddRange(b);
        return list;
    }
}
