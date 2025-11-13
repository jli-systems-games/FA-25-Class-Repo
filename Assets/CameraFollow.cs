using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smooth = 5f;
    void Start()
    {
        if (target != null)
        {
            Vector3 pos = target.position;
            pos.z = -2;
            transform.position = pos;
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector3 pos = target.position;
        pos.z = -2;
        transform.position = Vector3.Lerp(transform.position, pos, smooth * Time.deltaTime);
    }
}
