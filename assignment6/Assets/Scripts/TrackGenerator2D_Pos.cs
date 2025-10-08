using UnityEngine;
using System.Collections.Generic;

public class TrackGenerator2D_Pos : MonoBehaviour
{
    [Header("引用")]
    public Train2DController_Pos train;

    [Header("预制体")]
    public TrackSegment2D straightPrefab;
    public TrackSegment2D curveLeftPrefab;
    public TrackSegment2D curveRightPrefab;
    public TrackSegment2D junctionPrefab;

    [Header("参数")]
    public int initialSegments = 3;
    public int maxLiveSegments = 8;
    public float spawnAheadDistance = 10f;
    public float junctionChance = 0.35f;

    // 运行时
    private readonly List<TrackSegment2D> live = new List<TrackSegment2D>();

    // 不再保存 Transform 引用，改为保存“下一次拼接的世界 Pose”
    private Vector3 _nextAttachPos;
    private Quaternion _nextAttachRot = Quaternion.identity;

    // 最近道岔信息：也只存 Pose
    private TrackSwitch2D_Pos _lastSwitch;
    private Vector3 _leftEndPos, _rightEndPos;
    private Quaternion _leftEndRot, _rightEndRot;

    // —— 小工具 —— //
    bool Alive(Object o) => o != null;

    static void PoseFrom(Transform t, out Vector3 pos, out Quaternion rot)
    {
        if (t) { pos = t.position; rot = t.rotation; }
        else { pos = Vector3.zero; rot = Quaternion.identity; }
    }

    // 把“prefab 实例”的 EntryAnchor 对齐到 给定 Pose（pos/rot）
    TrackSegment2D SpawnAtPoseAlignEntry(TrackSegment2D prefab, Vector3 pos, Quaternion rot)
    {
        if (!prefab)
        {
            Debug.LogWarning("[TrackGenerator2D_Pos] SpawnAtPoseAlignEntry 收到空 prefab，已跳过一次生成。");
            return null;
        }

        var inst = Instantiate(prefab); // 先裸实例
        var entry = inst.entryAnchor ? inst.entryAnchor : inst.transform;

        // 旋转对齐
        Quaternion deltaRot = rot * Quaternion.Inverse(entry.rotation);
        inst.transform.rotation = deltaRot * inst.transform.rotation;

        // 平移对齐
        Vector3 deltaPos = pos - entry.position;
        inst.transform.position += deltaPos;

        return inst;
    }

    // 由片段末端锚点得到“下一拼接 Pose”
    void UpdateNextAttachFromSegment(TrackSegment2D seg)
    {
        if (!Alive(seg))
        {
            _nextAttachPos = Vector3.zero;
            _nextAttachRot = Quaternion.identity;
            return;
        }
        Transform end = seg.isJunction
            ? (seg.rightAnchor ? seg.rightAnchor : seg.leftAnchor)
            : (seg.exitAnchor ? seg.exitAnchor : seg.transform);

        PoseFrom(end, out _nextAttachPos, out _nextAttachRot);
    }

    void Start()
    {
        if (!train)
        {
            Debug.LogError("[TrackGenerator2D_Pos] Train 未设置");
            enabled = false; return;
        }

        // 起始段
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
        // 根据最近道岔的选择，更新下一拼接 Pose（只用数值，绝不持有 Transform）
        if (_lastSwitch)
        {
            if (_lastSwitch.usingLeft)
            {
                _nextAttachPos = _leftEndPos;
                _nextAttachRot = _leftEndRot;
            }
            else
            {
                _nextAttachPos = _rightEndPos;
                _nextAttachRot = _rightEndRot;
            }
        }

        // 末段距离检测
        var last = live.Count > 0 ? live[live.Count - 1] : null;
        if (Alive(last))
        {
            Transform end = last.isJunction
                ? (last.rightAnchor ? last.rightAnchor : last.leftAnchor)
                : (last.exitAnchor ? last.exitAnchor : last.transform);

            if (end) // 末端还活着就用它测距离
            {
                float dist = Vector2.Distance(train.transform.position, end.position);
                if (dist < spawnAheadDistance) SpawnNextAtCurrentPose();
            }
            else
            {
                // 末端已被外部意外销毁，直接按记录的 Pose 继续生成
                SpawnNextAtCurrentPose();
            }
        }

        // 回收最早片段（我们现在不持有它的任何 Transform，用数值就不会抛错）
        if (live.Count > maxLiveSegments)
        {
            var oldest = live[0];
            live.RemoveAt(0);
            if (Alive(oldest)) Destroy(oldest.gameObject);
        }
    }

    void SpawnNextAtCurrentPose()
    {
        // 选下一个要拼的预制体
        TrackSegment2D prefabToUse =
            (junctionPrefab && Random.value < junctionChance)
            ? junctionPrefab
            : RandomStraightOrCurve();

        // 在 _nextAttachPos/_nextAttachRot 处对齐生成
        var seg = SpawnAtPoseAlignEntry(prefabToUse, _nextAttachPos, _nextAttachRot);
        if (!Alive(seg)) return;

        live.Add(seg);

        if (seg.isJunction)
        {
            // 立刻在左右锚点各接一段，以便玩家“看见前方”
            var leftNext = SpawnAtPoseAlignEntry(RandomStraightOrCurve(),
                                seg.leftAnchor.position, seg.leftAnchor.rotation);
            var rightNext = SpawnAtPoseAlignEntry(RandomStraightOrCurve(),
                                seg.rightAnchor.position, seg.rightAnchor.rotation);
            if (Alive(leftNext)) live.Add(leftNext);
            if (Alive(rightNext)) live.Add(rightNext);

            // 组合左右路径（位置数组）
            var leftPath = ConcatPositions(seg.GetNodePositions(), Alive(leftNext) ? leftNext.GetNodePositions() : null);
            var rightPath = ConcatPositions(seg.GetNodePositions(), Alive(rightNext) ? rightNext.GetNodePositions() : null);

            // 配置道岔
            var sw = seg.GetComponent<TrackSwitch2D_Pos>();
            if (Alive(sw))
            {
                sw.train = train;
                sw.leftPathPositions = leftPath;
                sw.rightPathPositions = rightPath;
                sw.usingLeft = true; // 默认左
            }
            _lastSwitch = sw;

            // 记录左右末端的 Pose（纯数值）
            if (Alive(leftNext))
            {
                PoseFrom(leftNext.exitAnchor ? leftNext.exitAnchor : leftNext.transform,
                         out _leftEndPos, out _leftEndRot);
            }
            if (Alive(rightNext))
            {
                PoseFrom(rightNext.exitAnchor ? rightNext.exitAnchor : rightNext.transform,
                         out _rightEndPos, out _rightEndRot);
            }

            // 先把“默认左侧”的节点追加到火车路径
            if (Alive(leftNext)) train.AppendPositions(leftNext.GetNodePositions());

            // 下一拼接点先沿用“左侧”末端 Pose
            _nextAttachPos = _leftEndPos;
            _nextAttachRot = _leftEndRot;
        }
        else
        {
            // 普通片段：把节点追加 & 更新下一拼接 Pose
            train.AppendPositions(seg.GetNodePositions());
            UpdateNextAttachFromSegment(seg);
        }
    }

    TrackSegment2D RandomStraightOrCurve()
    {
        var pool = new List<TrackSegment2D>(3);
        if (straightPrefab) pool.Add(straightPrefab);
        if (curveLeftPrefab) pool.Add(curveLeftPrefab);
        if (curveRightPrefab) pool.Add(curveRightPrefab);

        if (pool.Count == 0)
        {
            Debug.LogError("[TrackGenerator2D_Pos] 没有可用的直/弯片段 Prefab！请至少指定 straightPrefab。");
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
