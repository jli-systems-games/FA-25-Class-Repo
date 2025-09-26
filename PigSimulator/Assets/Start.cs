using UnityEngine;
using System.Collections;

public class CameraIntroLerp : MonoBehaviour
{
    public Transform target;
    public float duration = 2f;
    public Vector3 startOffset = new Vector3(0f, 8f, -1f);
    public bool detachDuringIntro = true;
    public MonoBehaviour[] enableAfter;

    Transform originalParent;

    void Awake()
    {
        if (enableAfter != null) for (int i = 0; i < enableAfter.Length; i++) if (enableAfter[i]) enableAfter[i].enabled = false;
    }

    IEnumerator Start()
    {
        originalParent = transform.parent;
        Vector3 startPos = target.position + startOffset;
        Quaternion startRot = Quaternion.LookRotation((target.position - startPos).normalized, Vector3.up);
        if (detachDuringIntro) transform.SetParent(null);
        transform.position = startPos;
        transform.rotation = startRot;

        Vector3 endPos = target.position;
        Quaternion endRot = target.rotation;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / duration));
            transform.position = Vector3.Lerp(startPos, endPos, k);
            transform.rotation = Quaternion.Slerp(startRot, endRot, k);
            yield return null;
        }

        transform.position = endPos;
        transform.rotation = endRot;
        transform.SetParent(target, true);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        if (enableAfter != null) for (int i = 0; i < enableAfter.Length; i++) if (enableAfter[i]) enableAfter[i].enabled = true;

        Destroy(this);
    }
}
