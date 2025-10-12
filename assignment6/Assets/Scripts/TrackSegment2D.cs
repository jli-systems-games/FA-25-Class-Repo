using UnityEngine;
using System.Collections.Generic;

public class TrackSegment2D : MonoBehaviour
{
    [Header("Entry")]
    public Transform entryAnchor;
    public Transform exitAnchor;      

    [Header("Junction Y")]
    public Transform leftAnchor;
    public Transform rightAnchor;

    [Header("N0, N1, ...）")]
    public Transform nodesRoot;
    public List<Transform> nodes = new List<Transform>();

    [Header(" Y ？")]
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
