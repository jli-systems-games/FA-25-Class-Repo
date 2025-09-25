using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer), typeof(EdgeCollider2D), typeof(AudioSource))]
public class Stroke : MonoBehaviour
{
    public BrushSettings brush;
    public bool magnetOn = true;
    public float gridSize = 0.25f;
    public Transform pointsRoot; 

    LineRenderer lr;
    EdgeCollider2D ec;
    AudioSource audioSrc;

    readonly List<Vector2> pts = new();
    float _lastAddTime;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        ec = GetComponent<EdgeCollider2D>();
        audioSrc = GetComponent<AudioSource>();
    }

    public void Begin(BrushSettings b, Vector2 start, AudioClip beginClip)
    {
        brush = b;
        lr.widthCurve = brush.widthCurve;
        lr.colorGradient = brush.colorGradient;
        lr.positionCount = 0;
        pts.Clear();
        AddPoint(start, force: true);
        if (beginClip) audioSrc.PlayOneShot(beginClip, 0.9f);
    }

    public void Append(Vector2 pos, float dt, AudioClip dragClip)
    {
        if (Time.timeScale == 0f) return;
        if (dt <= 0f) dt = Time.deltaTime;
        float minInterval = 1f / Mathf.Max(brush.maxPointPerSecond, 1f);
        if (Time.time - _lastAddTime < minInterval) return;

        Vector2 last = pts[pts.Count - 1];
        if (Vector2.Distance(last, pos) < brush.pointMinDistance) return;

        var noise = (Vector2)Random.insideUnitCircle * brush.noiseAmount * 0.04f;
        pos += noise;

        if (magnetOn && brush.magnetStrength > 0f)
        {
            Vector2 grid = new Vector2(
                Mathf.Round(pos.x / gridSize) * gridSize,
                Mathf.Round(pos.y / gridSize) * gridSize
            );
            pos = Vector2.Lerp(pos, grid, brush.magnetStrength * 0.35f);
        }

        AddPoint(pos);

        if (dragClip && Random.value < 0.1f) audioSrc.PlayOneShot(dragClip, 0.4f);
    }

    public void End(AudioClip endClip)
    {
        if (brush && brush.elasticReturn > 0f)
        {
            Vector3 center = Vector3.zero;
            for (int i = 0; i < lr.positionCount; i++) center += lr.GetPosition(i);
            center /= Mathf.Max(lr.positionCount, 1);
            for (int i = 0; i < lr.positionCount; i++)
            {
                Vector3 p = lr.GetPosition(i);
                Vector3 q = Vector3.Lerp(p, center, brush.elasticReturn * 0.05f);
                lr.SetPosition(i, q);
            }
            SyncCollider();
        }
        if (endClip) audioSrc.PlayOneShot(endClip, 0.9f);
    }

    void AddPoint(Vector2 p, bool force = false)
    {
        pts.Add(p);
        lr.positionCount = pts.Count;
        lr.SetPosition(pts.Count - 1, p);
        _lastAddTime = Time.time;
        SyncCollider(force);
    }

    void SyncCollider(bool force = false)
    {
        if (pts.Count < 2) return;
        Vector2[] local = new Vector2[pts.Count];
        for (int i = 0; i < pts.Count; i++) local[i] = transform.InverseTransformPoint(pts[i]);
        ec.points = local;
    }

    public void SetColorGradient(Gradient g) { lr.colorGradient = g; }
}
