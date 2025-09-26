using UnityEngine;

public class TPCameraFixed : MonoBehaviour
{
    public Transform target;
    public Vector3 localOffset = new Vector3(0f, 1.6f, -3.5f);
    public Vector3 localEuler = new Vector3(10f, 0f, 0f);
    public float smooth = 10f;

    void LateUpdate()
    {
        if (!target) return;
        Vector3 desiredPos = target.TransformPoint(localOffset);
        transform.position = Vector3.Lerp(transform.position, desiredPos, 1f - Mathf.Exp(-smooth * Time.deltaTime));
        Quaternion desiredRot = target.rotation * Quaternion.Euler(localEuler);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, 1f - Mathf.Exp(-smooth * Time.deltaTime));
    }
}
