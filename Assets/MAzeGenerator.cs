using System.Collections.Generic;
using UnityEngine;

public class MAzeGenerator : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public GameObject player;
    public Camera mainCamera;
    public GameObject oneSidedWall;
    public GameObject twoSidedWall;
    public GameObject threeSidedWall;
    public GameObject floorPrefab;
    public Transform mazeParent;
    public GameObject musicNotePrefab;
    public int noteCount = 8;

    private int[,] grid;

    void Start()
    {
        GenerateMaze();
    }

    void GenerateMaze()
    {
        grid = new int[width, height];
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        Vector2Int current = new Vector2Int(0, 0);
        grid[0, 0] = 1;
        stack.Push(current);

        while (stack.Count > 0)
        {
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(current);
            if (neighbors.Count > 0)
            {
                Vector2Int next = neighbors[Random.Range(0, neighbors.Count)];
                RemoveWallBetween(current, next);
                grid[next.x, next.y] = 1;
                stack.Push(next);
                current = next;
            }
            else current = stack.Pop();
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == 1)
                    Instantiate(floorPrefab, new Vector3(x, y, 0), Quaternion.identity, mazeParent);
            }
        }

        AddWalls();
        AddBoundaryWalls();
        PlacePlayerInside();

    }
    void PlacePlayerInside()
    {
        if (player == null) return;

        List<Vector2Int> pathPositions = new List<Vector2Int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == 1)
                    pathPositions.Add(new Vector2Int(x, y));
            }
        }

        if (pathPositions.Count == 0) return;

        Vector2Int spawn = pathPositions[Random.Range(0, pathPositions.Count)];

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.position = new Vector2(spawn.x, spawn.y);
        else
            player.transform.position = new Vector3(spawn.x, spawn.y, 0);

        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(spawn.x, spawn.y, -10f);
        }
        SpawnNotes();
    }

    void SpawnNotes()
    {
        List<Vector2Int> floors = new List<Vector2Int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == 1)
                    floors.Add(new Vector2Int(x, y));
            }
        }

        for (int i = 0; i < floors.Count; i++)
        {
            int rand = Random.Range(i, floors.Count);
            (floors[i], floors[rand]) = (floors[rand], floors[i]);
        }

        int spawnAmount = Mathf.Min(noteCount, floors.Count);

        for (int i = 0; i < spawnAmount; i++)
        {
            Vector2Int pos = floors[i];

            GameObject note = Instantiate(
                musicNotePrefab,
                new Vector3(pos.x, pos.y, 0),
                Quaternion.identity,
                mazeParent
            );

            note.GetComponent<MusicNote>().trackIndex = i % noteCount;
        }
    }

    void AddWalls()
    {
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (grid[x, y] != 0) continue;

                bool up = (grid[x, y + 1] == 1);
                bool down = (grid[x, y - 1] == 1);
                bool left = (grid[x - 1, y] == 1);
                bool right = (grid[x + 1, y] == 1);

                int count = (up ? 1 : 0) + (down ? 1 : 0) + (left ? 1 : 0) + (right ? 1 : 0);

                GameObject prefab = null;
                Quaternion rot = Quaternion.identity;

                if (count == 1)
                {
                    prefab = oneSidedWall;
                    if (right) rot = Quaternion.Euler(0, 0, 180);
                    else if (up) rot = Quaternion.Euler(0, 0, -90);
                    else if (down) rot = Quaternion.Euler(0, 0, 90);
                }
                else if (count == 2)
                {
                    prefab = twoSidedWall;

                    if (up && down)
                        rot = Quaternion.Euler(0, 0, 90);
                }
                else if (count == 3)
                {
                    prefab = threeSidedWall;

                    if (!down) rot = Quaternion.identity;
                    else if (!left) rot = Quaternion.Euler(0, 0, -90);
                    else if (!up) rot = Quaternion.Euler(0, 0, 180);
                    else if (!right) rot = Quaternion.Euler(0, 0, 90);
                }

                if (prefab != null)
                    Instantiate(prefab, new Vector3(x, y, 0), rot, mazeParent);
            }
        }
    }


    void AddBoundaryWalls()
    {
        for (int x = 0; x < width; x++)
            Instantiate(oneSidedWall, new Vector3(x, -1, 0), Quaternion.Euler(0, 0, -90), mazeParent);

        for (int x = 0; x < width; x++)
            Instantiate(oneSidedWall, new Vector3(x, height, 0), Quaternion.Euler(0, 0, 90), mazeParent);

        for (int y = 0; y < height; y++)
            Instantiate(oneSidedWall, new Vector3(-1, y, 0), Quaternion.Euler(0, 0, 180), mazeParent);

        for (int y = 0; y < height; y++)
            Instantiate(oneSidedWall, new Vector3(width, y, 0), Quaternion.Euler(0, 0, 180), mazeParent);
    }

    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int cell)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        foreach (var d in dirs)
        {
            Vector2Int n = cell + d * 2;
            if (n.x >= 0 && n.x < width && n.y >= 0 && n.y < height && grid[n.x, n.y] == 0)
                result.Add(n);
        }
        return result;
    }

    void RemoveWallBetween(Vector2Int a, Vector2Int b)
    {
        Vector2Int mid = (a + b) / 2;
        grid[mid.x, mid.y] = 1;
    }
}
