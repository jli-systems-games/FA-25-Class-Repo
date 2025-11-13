using System.Collections;
using UnityEngine;

public class CameraDirector2D : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float shakeAmount = 0.2f;
    [SerializeField] private float zoomPunch = 0.85f;

    Vector3 basePos;
    float baseSize;
    bool busy;

    private void Awake()
    {
        if (!cam) cam = Camera.main;
        basePos = cam.transform.position;
        baseSize = cam.orthographicSize;
    }

    public void Punch(float dur = 0.12f, float back = 0.12f)
    {
        if (!gameObject.activeInHierarchy) return;
        if (busy) return;
        StartCoroutine(PunchCo(dur, back));
    }

    IEnumerator PunchCo(float dur, float back)
    {
        busy = true;
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            cam.transform.position = basePos + (Vector3)Random.insideUnitCircle * shakeAmount;
            cam.orthographicSize = Mathf.Lerp(baseSize, baseSize * zoomPunch, t / dur);
            yield return null;
        }
        t = 0f;
        while (t < back)
        {
            t += Time.deltaTime;
            cam.transform.position = Vector3.Lerp(cam.transform.position, basePos, t / back);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, baseSize, t / back);
            yield return null;
        }
        cam.transform.position = basePos;
        cam.orthographicSize = baseSize;
        busy = false;
    }
}
