using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public enum Winner { A, B, Draw }
public struct MatchResult { public int scoreA, scoreB, neutral; public Winner winner; }

public class MazeFloodGame_Overlay : MonoBehaviour
{
    public Transform startA;
    public Transform startB;
    public float tickInterval = 0.08f;
    public bool autoStartOnPlay = false;
    public float overlayYOffset = 0.02f;
    public Vector2 overlaySize = new Vector2(0.9f, 0.9f);
    public enum TieRule { PreferA, PreferB }
    public TieRule tieRule = TieRule.PreferA;
    public float matchDuration = 15f;
    public TMP_Text scoreAText;
    public TMP_Text scoreBText;
    public GameObject winnerAPrefab;
    public GameObject winnerBPrefab;
    public Transform winnerSpawnPoint;
    public event Action<MatchResult> OnMatchFinished;

    class CellInfo { public int distA=int.MaxValue, distB=int.MaxValue; public CellOwner owner=CellOwner.None; public ClaimOverlay overlay; }

    Dictionary<Vector2Int, MazeCell> grid;
    Dictionary<Vector2Int, CellInfo> infos;
    int maxLayer;
    bool _built;
    Coroutine _watchdog;
    Coroutine _pendingStart;
    int stableFrameCount = 3;
    float watchdogInterval = 0.5f;

    IEnumerator Start()
    {
        yield return StartCoroutine(EnsureBuilt());
        if (autoStartOnPlay) StartMatch();
    }

    void OnEnable()
    {
        if (_watchdog == null) _watchdog = StartCoroutine(Watchdog());
    }

    void OnDisable()
    {
        if (_watchdog != null) { StopCoroutine(_watchdog); _watchdog = null; }
    }

    public void RequestMatch()
    {
        if (_pendingStart != null) return;
        _pendingStart = StartCoroutine(RequestMatchCo());
    }
    IEnumerator RequestMatchCo()
    {
        yield return StartCoroutine(EnsureBuilt());
        StartMatch();
        _pendingStart = null;
    }

    IEnumerator EnsureBuilt()
    {
        if (_built) yield break;

        int last = -1, stable = 0;
        while (true)
        {
            int cnt = FindObjectsByType<MazeCell>(FindObjectsSortMode.None).Length;
            if (cnt > 0 && cnt == last) { stable++; if (stable >= stableFrameCount) break; }
            else { stable = 0; }
            last = cnt;
            yield return null;
        }

        BuildGridAndOverlays();
        _built = true;
    }

    IEnumerator Watchdog()
    {
        var wait = new WaitForSeconds(watchdogInterval);
        while (true)
        {
            if (_built)
            {
                int cells = FindObjectsByType<MazeCell>(FindObjectsSortMode.None).Length;
                int overlays = FindObjectsByType<ClaimOverlay>(FindObjectsSortMode.None).Length;
                if (cells > 0 && overlays < cells)
                {
                    foreach (var ov in FindObjectsByType<ClaimOverlay>(FindObjectsSortMode.None))
                        if (ov.gameObject.name == "ClaimOverlay") Destroy(ov.gameObject);
                    _built = false;
                    yield return StartCoroutine(EnsureBuilt());
                }
            }
            yield return wait;
        }
    }

    public void StartMatch()
    {
        if (!_built) { Debug.LogWarning("StartMatch 被调用时未建好，自动补建"); StartCoroutine(RequestMatchCo()); return; }
        if (grid == null || grid.Count == 0) { Debug.LogError("grid 空"); return; }

        StopAllCoroutines();
        ResetAll();

        var (a, b) = PickStartCells();
        if (a == null || b == null) { Debug.LogError("起点失败"); return; }

        ComputeDistances(a, true);
        ComputeDistances(b, false);
        StartCoroutine(AnimateWavefront());
    }

    void BuildGridAndOverlays()
    {
        grid  = new Dictionary<Vector2Int, MazeCell>();
        infos = new Dictionary<Vector2Int, CellInfo>();

        foreach (var cell in FindObjectsByType<MazeCell>(FindObjectsSortMode.None))
        {
            Vector2Int p = cell.GridPos;
            if (grid.ContainsKey(p)) continue;
            grid[p] = cell;

            var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.name = "ClaimOverlay";
            go.transform.SetParent(cell.transform, false);
            go.transform.localPosition = new Vector3(0, overlayYOffset, 0);
            go.transform.localRotation = Quaternion.Euler(90, 0, 0);
            go.transform.localScale    = new Vector3(overlaySize.x, overlaySize.y, 1f);
            var col = go.GetComponent<Collider>(); if (col) Destroy(col);

            var mr = go.GetComponent<MeshRenderer>();
            if (mr && mr.sharedMaterial && mr.sharedMaterial.HasProperty("_Cull"))
                mr.sharedMaterial.SetFloat("_Cull", 0);

            var overlay = go.AddComponent<ClaimOverlay>();
            overlay.SetOwner(CellOwner.None);

            infos[p] = new CellInfo { overlay = overlay };
        }
    }

    void ResetAll()
    {
        foreach (var kv in infos)
        {
            kv.Value.distA = int.MaxValue; kv.Value.distB = int.MaxValue;
            kv.Value.owner = CellOwner.None;
            kv.Value.overlay.SetOwner(CellOwner.None);
        }
        maxLayer = 0;
        if (scoreAText) scoreAText.text = "0";
        if (scoreBText) scoreBText.text = "0";
    }

    (MazeCell a, MazeCell b) PickStartCells()
    {
        if (grid.Count == 0) return (null, null);

        if (startA && startB)
        {
            var ca = FindNearestCell(startA.position);
            var cb = FindNearestCell(startB.position);
            if (ca && cb) return (ca, cb);
        }

        var keys = grid.Keys.ToList();
        int minX = keys.Min(k => k.x), maxX = keys.Max(k => k.x);
        int minZ = keys.Min(k => k.y), maxZ = keys.Max(k => k.y);
        grid.TryGetValue(new Vector2Int(minX, minZ), out var A);
        grid.TryGetValue(new Vector2Int(maxX, maxZ), out var B);
        if (!A || !B)
        {
            var aGuess = keys[0];
            var bGuess = keys.OrderBy(k => Sqr(k, aGuess)).Last();
            var aGuess2 = keys.OrderBy(k => Sqr(k, bGuess)).Last();
            A = grid[aGuess2]; B = grid[bGuess];
        }
        return (A, B);
    }

    MazeCell FindNearestCell(Vector3 w)
    {
        float best=float.PositiveInfinity; MazeCell bestC=null;
        foreach (var kv in grid)
        {
            var c = kv.Value.transform.position;
            float d = (new Vector2(w.x,w.z)-new Vector2(c.x,c.z)).sqrMagnitude;
            if (d<best){best=d; bestC=kv.Value;}
        }
        return bestC;
    }

    static float Sqr(Vector2Int a, Vector2Int b){int dx=a.x-b.x,dz=a.y-b.y; return dx*dx+dz*dz;}

    void ComputeDistances(MazeCell start, bool isA)
    {
        var q=new Queue<Vector2Int>(); var vis=new HashSet<Vector2Int>(); var s=start.GridPos;
        if (isA) infos[s].distA=0; else infos[s].distB=0;
        q.Enqueue(s); vis.Add(s);
        while(q.Count>0)
        {
            var p=q.Dequeue();
            int baseDist=isA?infos[p].distA:infos[p].distB;
            maxLayer=Mathf.Max(maxLayer, baseDist);
            foreach(var n in GetOpenNeighbors(p))
            {
                if (vis.Contains(n)) continue;
                vis.Add(n); q.Enqueue(n);
                if (isA) infos[n].distA=baseDist+1; else infos[n].distB=baseDist+1;
                maxLayer=Mathf.Max(maxLayer, baseDist+1);
            }
        }
    }

    IEnumerable<Vector2Int> GetOpenNeighbors(Vector2Int p)
    {
        if (!grid.TryGetValue(p, out var cell)) yield break;
        if (cell.IsOpenLeft)  { var np=new Vector2Int(p.x-1,p.y); if (grid.ContainsKey(np)) yield return np; }
        if (cell.IsOpenRight) { var np=new Vector2Int(p.x+1,p.y); if (grid.ContainsKey(np)) yield return np; }
        if (cell.IsOpenFront) { var np=new Vector2Int(p.x,p.y+1); if (grid.ContainsKey(np)) yield return np; }
        if (cell.IsOpenBack)  { var np=new Vector2Int(p.x,p.y-1); if (grid.ContainsKey(np)) yield return np; }
    }

    IEnumerator AnimateWavefront()
    {
        float elapsed = 0f;
        int step=0;
        while(elapsed < matchDuration)
        {
            foreach (var kv in infos)
            {
                var info=kv.Value;

                bool a=(info.distA==step), b=(info.distB==step);
                if (!a && !b) continue;

                if (a && b)
                {
                    info.owner = tieRule==TieRule.PreferA ? CellOwner.A : CellOwner.B;
                }
                else if (a)
                {
                    if (info.owner == CellOwner.None || info.owner == CellOwner.B)
                        info.owner = CellOwner.A;
                }
                else if (b)
                {
                    if (info.owner == CellOwner.None || info.owner == CellOwner.A)
                        info.owner = CellOwner.B;
                }

                info.overlay.SetOwner(info.owner);
            }

            var wait = new WaitForSeconds(tickInterval);
            yield return wait;
            elapsed += tickInterval;
            step++;
            
            if (step > maxLayer)
                step = 0;
        }

        int sa=0,sb=0;
        foreach (var v in infos.Values)
        {
            if (v.owner==CellOwner.A) sa++;
            else if (v.owner==CellOwner.B) sb++;
        }

        if (scoreAText) scoreAText.text = $"{sa}";
        if (scoreBText) scoreBText.text = $"{sb}";

        Winner w = sa>sb ? Winner.A : sb>sa ? Winner.B : Winner.Draw;

        if (winnerSpawnPoint != null)
        {
            if (w == Winner.A && winnerAPrefab != null)
            {
                Instantiate(winnerAPrefab, winnerSpawnPoint.position, winnerSpawnPoint.rotation);
            }
            else if (w == Winner.B && winnerBPrefab != null)
            {
                Instantiate(winnerBPrefab, winnerSpawnPoint.position, winnerSpawnPoint.rotation);
            }
        }

        OnMatchFinished?.Invoke(new MatchResult{scoreA=sa,scoreB=sb,neutral=0,winner=w});
    }
}