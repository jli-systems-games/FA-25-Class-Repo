using UnityEngine;

public class DirectSpot : MonoBehaviour
{
    public StatePlayer player;
    public string stateName = "Base Layer/Crew_Dance_Twerk";
    public float holdSeconds = 2.5f;
    public float fade = 0.06f;
    public Transform snapPoint;
    public bool alignForward = true;
    public LayerMask crewMask = ~0;

    void Reset() { GetOrMakeRb(); }

    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & crewMask) == 0) return;
        var p = other.GetComponentInParent<StatePlayer>();
        if (p == null) p = player;
        if (p == null) return;
        if (snapPoint)
        {
            var t = p.transform;
            t.position = snapPoint.position;
            if (alignForward) t.rotation = Quaternion.LookRotation(snapPoint.forward, Vector3.up);
        }
        p.PlayState(stateName, fade, holdSeconds);
    }

    void GetOrMakeRb()
    {
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
        var rb = GetComponent<Rigidbody>();
        if (!rb) { rb = gameObject.AddComponent<Rigidbody>(); rb.isKinematic = true; rb.useGravity = false; }
    }
}
