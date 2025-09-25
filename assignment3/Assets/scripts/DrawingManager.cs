using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DrawingManager : MonoBehaviour
{
    public Camera cam;
    public Transform strokeContainer;
    public GameObject strokePrefab;

    public BrushManager brushMgr;
    public SfxManager sfx;
    public SlimeCanvas slime;

    public bool magnetToggle = true;

    readonly Stack<GameObject> _history = new();

    Stroke _current;
    Vector2 _lastPos;
    float _lastUpdate;

    void Start()
    {
        if (!cam) cam = Camera.main;
        if (!strokeContainer) strokeContainer = GameObject.Find("StrokeContainer").transform;
    }

    void Update()
    {
        if (EventSystem.current && EventSystem.current.IsPointerOverGameObject()) return;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0)) BeginStroke(GetWorld(Input.mousePosition));
        if (Input.GetMouseButton(0))     MoveStroke(GetWorld(Input.mousePosition));
        if (Input.GetMouseButtonUp(0))   EndStroke();
#else
        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began) BeginStroke(GetWorld(t.position));
            if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary) MoveStroke(GetWorld(t.position));
            if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) EndStroke();
        }
#endif
    }

    Vector2 GetWorld(Vector2 screen) => cam.ScreenToWorldPoint(screen);

    void BeginStroke(Vector2 pos)
    {
        var b = brushMgr.Current;
        if (!b) return;

        GameObject go = Instantiate(strokePrefab, strokeContainer);
        var stroke = go.GetComponent<Stroke>();
        stroke.magnetOn = magnetToggle;
        var begin = b.beginClip ? b.beginClip : sfx?.RandomOf(sfx?.taps);
        stroke.Begin(b, pos, begin);


        _current = stroke;
        _lastPos = pos;
        _lastUpdate = Time.time;

        slime?.Pulse();
    }

    void MoveStroke(Vector2 pos)
    {
        if (!_current) return;
        float dt = Time.time - _lastUpdate;
        var draw = _current.brush && _current.brush.drawClip ? _current.brush.drawClip : sfx?.RandomOf(sfx?.drags);
        _current.Append(pos, dt, draw);

        _lastPos = pos;
        _lastUpdate = Time.time;
    }

    void EndStroke()
    {
        if (!_current) return;
        var end = _current.brush && _current.brush.endClip ? _current.brush.endClip : sfx?.RandomOf(sfx?.pops);
        _current.End(end);

        _history.Push(_current.gameObject);
        _current = null;
        slime?.Pulse();
    }

    public void UndoLast()
    {
        if (_current != null) return;
        if (_history.Count == 0) return;
        var go = _history.Pop();
        Destroy(go);
        slime?.Pulse();
    }

    public void ClearAll()
    {
        if (_current != null) return;
        foreach (Transform c in strokeContainer) Destroy(c.gameObject);
        _history.Clear();
        slime?.Pulse();
    }
}
