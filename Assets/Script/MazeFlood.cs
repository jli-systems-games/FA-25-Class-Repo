// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
// using TMPro;

// public class MazeFloodGame : MonoBehaviour
// {
//     [Header("可选：手动起点（留空则自动用两个角）")]
//     public Transform startA;
//     public Transform startB;

//     [Header("节奏/表现")]
//     public float tickInterval = 0.1f;
//     public bool autoStartOnPlay = true;
//     public bool revealContestedDuringPlay = true;

//     [Header("UI（可选）")]
//     public TMP_Text uiStepText;
//     public TMP_Text uiScoreText;
//     public TMP_Text uiWinnerText;

//     Dictionary<Vector2Int, MazeCell> grid;
//     Dictionary<Vector2Int, CellState> states;
//     int maxLayer;

//     IEnumerator Start()
//     {
//         // 等 1 帧，避免和迷宫生成器抢时序
//         yield return null;

//         // 如果是运行时生成很多帧后才出格子，可换成：
//         // yield return new WaitUntil(() => FindObjectsOfType<MazeCell>().Length > 0);

//         BuildGrid();

//         if (grid == null || grid.Count == 0)
//         {
//             Debug.LogError("[MazeFlood] 没找到任何 MazeCell。请确认已实例化且挂了 MazeCell 组件。");
//             yield break;
//         }

//         if (autoStartOnPlay) StartMatch();
//     }

//     public void StartMatch()
//     {
//         if (grid == null || grid.Count == 0)
//         {
//             Debug.LogError("[MazeFlood] grid 为空，无法开始。");
//             return;
//         }

//         ResetAllStates();
//         var pair = PickStartCells();
//         if (pair.a == null || pair.b == null)
//         {
//             Debug.LogError("[MazeFlood] 起点选择失败。");
//             return;
//         }

//         ComputeDistances(pair.a, true);
//         ComputeDistances(pair.b, false);
//         StartCoroutine(AnimateWavefront());
//     }

//     void BuildGrid()
//     {
//         grid = new Dictionary<Vector2Int, MazeCell>();
//         states = new Dictionary<Vector2Int, CellState>();

//         var cells = FindObjectsOfType<MazeCell>();
//         foreach (var cell in cells)
//         {
//             // 你的 MazeCell 里应有整数网格坐标：GridPos（Vector2Int）
//             var pos = cell.GridPos;
//             grid[pos] = cell;

//             // 绑定/添加 CellState，并把它的 targetRenderer 指向 Unvisited Block
//             var st = cell.GetComponent<CellState>();
//             if (st == null) st = cell.gameObject.AddComponent<CellState>();

//             if (st.targetRenderer == null)
//             {
//                 var tr = cell.transform.Find("Unvisited Block");
//                 if (tr != null) st.targetRenderer = tr.GetComponent<Renderer>();
//             }

//             states[pos] = st;
//         }

//         if (grid.Count == 0)
//             Debug.LogWarning("[MazeFlood] BuildGrid 完成，但没有 MazeCell。");
//     }

//     void ResetAllStates()
//     {
//         foreach (var st in states.Values)
//         {
//             st.distA = int.MaxValue;
//             st.distB = int.MaxValue;
//             st.owner = CellOwner.None;
//             st.ApplyHidden();
//         }
//         maxLayer = 0;
//         if (uiStepText) uiStepText.text = "Step: 0";
//         if (uiScoreText) uiScoreText.text = "A 0 : 0 B";
//         if (uiWinnerText) uiWinnerText.text = "";
//     }

//     (MazeCell a, MazeCell b) PickStartCells()
//     {
//         if (grid == null || grid.Count == 0) return (null, null);

//         // 手动起点（就近吸附到格子）
//         if (startA != null && startB != null)
//         {
//             var aCell = FindNearestCell(startA.position);
//             var bCell = FindNearestCell(startB.position);
//             if (aCell != null && bCell != null) return (aCell, bCell);
//             Debug.LogWarning("[MazeFlood] 手动起点附近未匹配到 MazeCell，改用自动角落。");
//         }

//         var keys = grid.Keys.ToList();
//         if (keys.Count == 0) return (null, null);

//         int minX = keys[0].x, maxX = keys[0].x, minZ = keys[0].y, maxZ = keys[0].y;
//         foreach (var k in keys)
//         {
//             if (k.x < minX) minX = k.x;
//             if (k.x > maxX) maxX = k.x;
//             if (k.y < minZ) minZ = k.y;
//             if (k.y > maxZ) maxZ = k.y;
//         }

//         var pA = new Vector2Int(minX, minZ);
//         var pB = new Vector2Int(maxX, maxZ);

//         grid.TryGetValue(pA, out var cellA);
//         grid.TryGetValue(pB, out var cellB);

//         if (cellA == null || cellB == null)
//         {
//             // 兜底：用“直径两端”
//             var aGuess = keys[0];
//             var bGuess = keys.OrderBy(k => SqrDist(k, aGuess)).Last();
//             var aGuess2 = keys.OrderBy(k => SqrDist(k, bGuess)).Last();
//             cellA = grid[aGuess2];
//             cellB = grid[bGuess];
//         }
//         return (cellA, cellB);
//     }

//     MazeCell FindNearestCell(Vector3 world)
//     {
//         var guess = new Vector2Int(Mathf.RoundToInt(world.x), Mathf.RoundToInt(world.z));
//         if (grid.TryGetValue(guess, out var cell)) return cell;

//         float best = float.PositiveInfinity;
//         MazeCell bestCell = null;
//         foreach (var kv in grid)
//         {
//             float d = (new Vector2(world.x, world.z) - new Vector2(kv.Key.x, kv.Key.y)).sqrMagnitude;
//             if (d < best) { best = d; bestCell = kv.Value; }
//         }
//         return bestCell;
//     }

//     static int SqrDist(Vector2Int a, Vector2Int b)
//     {
//         int dx = a.x - b.x, dz = a.y - b.y;
//         return dx * dx + dz * dz;
//     }

//     void ComputeDistances(MazeCell start, bool isA)
//     {
//         var startPos = start.GridPos;
//         var q = new Queue<Vector2Int>();
//         var visited = new HashSet<Vector2Int>();

//         if (isA) states[startPos].distA = 0; else states[startPos].distB = 0;
//         q.Enqueue(startPos);
//         visited.Add(startPos);
//         maxLayer = 0;

//         while (q.Count > 0)
//         {
//             var p = q.Dequeue();
//             int baseDist = isA ? states[p].distA : states[p].distB;
//             maxLayer = Mathf.Max(maxLayer, baseDist);

//             foreach (var n in GetOpenNeighbors(p))
//             {
//                 if (!visited.Contains(n))
//                 {
//                     visited.Add(n);
//                     q.Enqueue(n);
//                     if (isA) states[n].distA = baseDist + 1;
//                     else     states[n].distB = baseDist + 1;
//                     maxLayer = Mathf.Max(maxLayer, baseDist + 1);
//                 }
//             }
//         }
//     }

//     // —— 通过墙体激活状态判断通路：墙“关闭/被移除”=> 可通行 ——
//     IEnumerable<Vector2Int> GetOpenNeighbors(Vector2Int p)
//     {
//         if (!grid.ContainsKey(p)) yield break;

//         var cell = grid[p];

//         bool openL = IsOpenLeft(cell);
//         bool openR = IsOpenRight(cell);
//         bool openF = IsOpenFront(cell);
//         bool openB = IsOpenBack(cell);

//         if (openL)
//         {
//             var np = new Vector2Int(p.x - 1, p.y);
//             if (grid.ContainsKey(np)) yield return np;
//         }
//         if (openR)
//         {
//             var np = new Vector2Int(p.x + 1, p.y);
//             if (grid.ContainsKey(np)) yield return np;
//         }
//         if (openF) // +z
//         {
//             var np = new Vector2Int(p.x, p.y + 1);
//             if (grid.ContainsKey(np)) yield return np;
//         }
//         if (openB) // -z
//         {
//             var np = new Vector2Int(p.x, p.y - 1);
//             if (grid.ContainsKey(np)) yield return np;
//         }
//     }

//     // —— 这些方法按你的字段名做了默认推断 —— 
//     //    规则：墙体 GameObject 处于“禁用/隐藏” => 该方向开口
//     bool IsOpenLeft (MazeCellEVE c)  => c.LeftWall  == null || !c.LeftWall.activeSelf;
//     bool IsOpenRight(MazeCellEVE c)  => c.RightWall == null || !c.RightWall.activeSelf;
//     bool IsOpenFront(MazeCellEVE c)  => c.FrontWall == null || !c.FrontWall.activeSelf;
//     bool IsOpenBack (MazeCellEVE c)  => c.BackWall  == null || !c.BackWall.activeSelf;

//     IEnumerator AnimateWavefront()
//     {
//         int step = 0;
//         while (step <= maxLayer)
//         {
//             foreach (var kv in states)
//             {
//                 var st = kv.Value;
//                 if (st.owner != CellOwner.None) continue;

//                 bool reachA = (st.distA == step);
//                 bool reachB = (st.distB == step);

//                 if (reachA && reachB)
//                 {
//                     if (revealContestedDuringPlay) st.RevealAs(CellOwner.Contested);
//                 }
//                 else if (reachA) st.RevealAs(CellOwner.A);
//                 else if (reachB) st.RevealAs(CellOwner.B);
//             }

//             if (uiStepText) uiStepText.text = $"Step: {step}";
//             yield return new WaitForSeconds(tickInterval);
//             step++;
//         }

//         int scoreA = 0, scoreB = 0, neutral = 0;
//         foreach (var st in states.Values)
//         {
//             if (st.distA == int.MaxValue && st.distB == int.MaxValue)
//             {
//                 st.RevealAs(CellOwner.Neutral); neutral++; continue;
//             }

//             if (st.distA < st.distB) { st.RevealAs(CellOwner.A); scoreA++; }
//             else if (st.distB < st.distA) { st.RevealAs(CellOwner.B); scoreB++; }
//             else { st.RevealAs(CellOwner.Neutral); neutral++; }
//         }

//         if (uiScoreText) uiScoreText.text = $"A {scoreA} : {scoreB} B  (Neutral {neutral})";
//         if (uiWinnerText)
//         {
//             if (scoreA > scoreB) uiWinnerText.text = "Winner: A";
//             else if (scoreB > scoreA) uiWinnerText.text = "Winner: B";
//             else uiWinnerText.text = "Draw";
//         }
//     }
// }