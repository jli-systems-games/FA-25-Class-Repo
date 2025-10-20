using System.Collections.Generic;
using UnityEngine;

public class PlaySceneLoader : MonoBehaviour
{
    public PrefabLibrary library;
    public Transform blocksParent;
    public EarthquakeBaseMover earthquakeBase;

void Start()
{
    var blocks = SaveLoad.Load();
    foreach (var b in blocks)
    {
        var prefab = library.Get(b.prefabName);
        if (!prefab) continue;

        var go = Instantiate(prefab, b.position, b.rotation, blocksParent);
        var rb = go.GetComponent<Rigidbody>() ?? go.AddComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        go.layer = LayerMask.NameToLayer("Block");
    }
}
}