using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Train2DController_Pos : MonoBehaviour
{
    [Header("Path (world positions)")]
    public List<Vector3> pathPositions = new List<Vector3>();

    [Header("Movement")]
    public float speed = 3f;
    public float accelPerSec = 0.4f;
    public float arriveThreshold = 0.08f;
    public bool isAlive = true;

    [Header("Visual heading")]
    [Tooltip("Your sprite's forward direction.\n" +
             "Sprite faces RIGHT -> 0\n" +
             "Sprite faces UP    -> -90\n" +
             "Sprite faces DOWN  -> 90\n" +
             "Sprite faces LEFT  -> 180")]
    public float spriteAngleOffset = -90f;

    private int _index = 0;
    private Rigidbody2D _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.isKinematic = true; 
    }

    void Update()
    {
        if (!isAlive || pathPositions == null || pathPositions.Count == 0) return;

        if (_index >= pathPositions.Count)
        {
            isAlive = false;
            GameManager2D.I?.Win();
            return;
        }

        speed += accelPerSec * Time.deltaTime;

        Vector3 target = pathPositions[_index];
        Vector2 pos = transform.position;
        Vector2 dir = (Vector2)target - pos;

        if (dir.magnitude <= arriveThreshold)
        {
            _index++;
            if (_index >= 8) TrimPassedPoints(); 
            return;
        }

        Vector2 step = dir.normalized * speed * Time.deltaTime;
        transform.position = pos + step;

        if (step.sqrMagnitude > 1e-6f)
        {
            float ang = Mathf.Atan2(step.y, step.x) * Mathf.Rad2Deg + spriteAngleOffset;
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.Euler(0, 0, ang),
                0.25f
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            isAlive = false;
            GameManager2D.I?.GameOver();
        }
    }

    public void SetNewPathPositions(List<Vector3> pts, int startIndex = 0)
    {
        pathPositions = (pts != null) ? new List<Vector3>(pts) : new List<Vector3>();
        _index = Mathf.Clamp(startIndex, 0, Mathf.Max(0, pathPositions.Count - 1));
    }

    public void AppendPositions(List<Vector3> more)
    {
        if (more == null || more.Count == 0) return;
        Vector3 cur = CurrentTargetPos();
        if (pathPositions.Count == 0) pathPositions.Add(cur);
        foreach (var p in more)
            if ((p - cur).sqrMagnitude > 1e-6f) pathPositions.Add(p);
    }

    public Vector3 CurrentTargetPos()
    {
        if (_index >= 0 && _index < pathPositions.Count) return pathPositions[_index];
        if (pathPositions.Count > 0) return pathPositions[pathPositions.Count - 1];
        return transform.position;
    }

    public void TrimPassedPoints()
    {
        if (_index <= 0) return;
        pathPositions.RemoveRange(0, _index);
        _index = 0;
    }
}
