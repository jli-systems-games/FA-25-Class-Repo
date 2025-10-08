using UnityEngine;
using System.Collections.Generic;

public class TrackSegment2D : MonoBehaviour
{
    [Header("锚点（Entry 必须存在）")]
    public Transform entryAnchor;
    public Transform exitAnchor;         // 直/弯片段使用

    [Header("Junction 额外锚点（Y型道岔）")]
    public Transform leftAnchor;
    public Transform rightAnchor;

    [Header("路径节点（放在 nodesRoot 下：N0, N1, ...）")]
    public Transform nodesRoot;
    public List<Transform> nodes = new List<Transform>();

    [Header("是否为 Y 型道岔片段")]
    public bool isJunction = false;

    void OnValidate() { RefreshNodesFromChildren(); }

    public void RefreshNodesFromChildren()
    {
        nodes.Clear();
        if (!nodesRoot) return;
        for (int i = 0; i < nodesRoot.childCount; i++)
            nodes.Add(nodesRoot.GetChild(i));
    }

    public List<Vector3> GetNodePositions()
    {
        if (nodes == null || nodes.Count == 0) RefreshNodesFromChildren();
        var list = new List<Vector3>(nodes.Count);
        foreach (var t in nodes) if (t) list.Add(t.position);
        return list;
    }
}
