using System.Collections;
using UnityEngine;

public class SimpleSpawnOnFirstPlatform : MonoBehaviour
{
    public Transform head;
    public float eyeHeight = 1.6f;

    CharacterController cc; Rigidbody rb;
    void Awake() { cc = GetComponent<CharacterController>(); rb = GetComponent<Rigidbody>(); }

    public void SpawnAt(Vector3 topCenter) { StartCoroutine(SpawnCo(topCenter)); }

    IEnumerator SpawnCo(Vector3 topCenter)
    {
        yield return null; // 等一帧让碰撞生成完
        bool ccOn = cc && cc.enabled; if (ccOn) cc.enabled = false;
        bool rbKin = rb && rb.isKinematic; if (rb) { rb.isKinematic = true; rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }

        Vector3 pos = topCenter + Vector3.up * 0.1f;
        transform.position = pos;
        if (head) head.position = pos + Vector3.up * (eyeHeight - 0.1f);

        yield return null;
        if (ccOn) cc.enabled = true;
        if (rb) { rb.isKinematic = rbKin; rb.linearVelocity = Vector3.zero; }
    }
}
