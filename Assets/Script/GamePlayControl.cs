using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class GamePlayControl : MonoBehaviour
{
    public static bool AllCoinsCollected { get; private set; } = false;
    public static event Action OnAllCoinsCollected;

    private static GamePlayControl _instance;
    public static GamePlayControl Instance => _instance;

    [Header("Scene Transition")]
    [SerializeField] private string nextSceneName;

    [Header("Maze")]
    [SerializeField] private Transform mazeRoot;
    [SerializeField] private float spawnYOffset = 1.0f;
    [SerializeField] private float goalYOffset  = 1.5f;

    [Header("Goal")]
    [SerializeField] private GameObject goalPrefab;

    [Header("Coins")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int coinCount = 5;
    [SerializeField] private float coinYOffset = 1.2f;

    [Header("UI")]
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private TMP_Text toastTemplate;
    [SerializeField] private float toastFadeTime = 0.6f;
    [SerializeField] private float toastRiseDistance = 40f;

    private int _collected = 0;
    private int _total     = 0;
    private void Awake()
    {
        _instance = this;
    }

    private void OnEnable()
    {
        Coin.OnCollected += HandleCoinCollected;
    }

    private void OnDisable()
    {
        Coin.OnCollected -= HandleCoinCollected;
        if (_instance == this) _instance = null;
    }

    private void Start()
    {
        StartCoroutine(PlaceAfterMazeReady());
    }

    private IEnumerator PlaceAfterMazeReady()
    {
        yield return null;

        var cells = GetAllCells();
        if (cells.Count == 0)
        {
            yield break;
        }

        var graph = BuildGrid(cells, out var coordToCell);

        var any = cells[0];
        var A = FarthestFrom(any, graph);
        var B = FarthestFrom(A,   graph);

        MovePlayerTo(A.transform.position + Vector3.up * spawnYOffset);

        if (!goalPrefab)
        {
            yield break;
        }
        var goal = Instantiate(goalPrefab,
                               B.transform.position + Vector3.up * goalYOffset,
                               Quaternion.identity,
                               mazeRoot ? mazeRoot : null);

        var goalComp = goal.GetComponentInChildren<Goal>() ?? goal.GetComponent<Goal>();
        if (goalComp)
        {
            goalComp.requireAllCoins = true;
        }

        SpawnCoinsRandom(cells, A, B);

        AllCoinsCollected = (_total == 0);
        if (AllCoinsCollected)
        {
            OnAllCoinsCollected?.Invoke();
            UpdateProgressUI(0, 0);
        }
    }

    private void HandleCoinCollected(Coin _)
    {
        _collected++;
        UpdateProgressUI(_collected, _total);
        ShowToast("+1");

        if (_collected >= _total && _total > 0)
        {
            AllCoinsCollected = true;
            OnAllCoinsCollected?.Invoke();
        }
    }

    private void UpdateProgressUI(int collected, int total)
    {
        if (!progressText) return;
        progressText.text = total > 0 ? $"{collected}/{total}" : "Coins: 0/0";
    }

    public static void ShowToast(string text)
    {
        var inst = Instance;
        if (!inst || !inst.toastTemplate) return;
        inst.StartCoroutine(inst.ToastRoutine(text));
    }

    public static void OnGoalReached()
    {
        var inst = Instance;
        if (!inst) return;
        inst.LoadNextScene();
    }

    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        }
    }

    private IEnumerator ToastRoutine(string text)
    {
        var t = Instantiate(toastTemplate, toastTemplate.transform.parent);
        t.gameObject.SetActive(true);
        t.text = text;

        var cg = t.GetComponent<CanvasGroup>();
        if (!cg) cg = t.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0f;

        var rt = t.rectTransform;
        Vector2 start = rt.anchoredPosition;
        Vector2 end   = start + Vector2.up * toastRiseDistance;

        float time = 0f;
        while (time < toastFadeTime)
        {
            time += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(time / toastFadeTime);

            cg.alpha = 1f - Mathf.Abs(k * 2f - 1f);
            rt.anchoredPosition = Vector2.Lerp(start, end, k);
            yield return null;
        }
        Destroy(t.gameObject);
    }
    private void MovePlayerTo(Vector3 pos)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (!player)
        {
            return;
        }

        var cc = player.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;

        var rb = player.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        player.transform.SetPositionAndRotation(pos, Quaternion.identity);

        if (cc) cc.enabled = true;
    }

    private List<MazeCell> GetAllCells()
    {
        MazeCell[] arr = null;
        if (mazeRoot) arr = mazeRoot.GetComponentsInChildren<MazeCell>(includeInactive: false);
        if (arr == null || arr.Length == 0) arr = FindObjectsByType<MazeCell>(FindObjectsSortMode.None);
        return arr?.ToList() ?? new List<MazeCell>();
    }

    private Dictionary<MazeCell, List<MazeCell>> BuildGrid(List<MazeCell> cells, out Dictionary<Vector2Int, MazeCell> map)
    {
        map = new Dictionary<Vector2Int, MazeCell>();
        foreach (var c in cells) map[c.GridPos] = c;

        var g = new Dictionary<MazeCell, List<MazeCell>>(cells.Count);
        foreach (var c in cells)
        {
            var nb = new List<MazeCell>();
            var p = c.GridPos;

            if (c.IsOpenLeft  && map.TryGetValue(new Vector2Int(p.x - 1, p.y), out var L)) nb.Add(L);
            if (c.IsOpenRight && map.TryGetValue(new Vector2Int(p.x + 1, p.y), out var R)) nb.Add(R);
            if (c.IsOpenFront && map.TryGetValue(new Vector2Int(p.x, p.y + 1), out var F)) nb.Add(F);
            if (c.IsOpenBack  && map.TryGetValue(new Vector2Int(p.x, p.y - 1), out var B)) nb.Add(B);

            g[c] = nb;
        }
        return g;
    }

    private MazeCell FarthestFrom(MazeCell start, Dictionary<MazeCell, List<MazeCell>> graph)
    {
        var q = new Queue<MazeCell>();
        var dist = new Dictionary<MazeCell, int>();
        q.Enqueue(start);
        dist[start] = 0;

        MazeCell far = start;

        while (q.Count > 0)
        {
            var u = q.Dequeue();
            if (dist[u] > dist[far]) far = u;

            foreach (var v in graph[u])
            {
                if (dist.ContainsKey(v)) continue;
                dist[v] = dist[u] + 1;
                q.Enqueue(v);
            }
        }
        return far;
    }

    private void SpawnCoinsRandom(List<MazeCell> cells, MazeCell A, MazeCell B)
    {
        if (!coinPrefab || coinCount <= 0)
        {
            _collected = 0; _total = 0;
            UpdateProgressUI(_collected, _total);
            return;
        }

        var candidates = cells.Where(c => c != A && c != B).ToList();
        if (candidates.Count == 0)
        {
            _collected = 0; _total = 0;
            UpdateProgressUI(_collected, _total);
            return;
        }

        int n = Mathf.Min(coinCount, candidates.Count);

        for (int i = 0; i < n; i++)
        {
            int pick = UnityEngine.Random.Range(i, candidates.Count);
            (candidates[i], candidates[pick]) = (candidates[pick], candidates[i]);
        }

        for (int i = 0; i < n; i++)
        {
            var cell = candidates[i];
            var pos = cell.transform.position + Vector3.up * coinYOffset;
            Instantiate(coinPrefab, pos, Quaternion.identity, mazeRoot ? mazeRoot : null);
        }

        _collected = 0;
        _total     = n;
        UpdateProgressUI(_collected, _total);
    }
}