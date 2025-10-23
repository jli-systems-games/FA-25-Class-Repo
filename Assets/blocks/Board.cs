using DG.Tweening.Core.Easing;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI.Table;
using System.Collections;


/// <summary>
/// code referencing https://www.youtube.com/watch?v=ODLzYI4d-J8
/// </summary>

public class Board : MonoBehaviour
{
    public GameManager gameManager;

    public Tilemap tilemap {  get; private set; }
    public Piece activePiece { get; private set; }
    public TetrominoData[] tetrominoes;
    public Vector3Int spawnPosition;
    public AudioSource audioSource;
    public AudioClip lineClearSFX;
    public Vector2Int boardSize = new Vector2Int(10, 20);

    [Header("Random Texts While Wating")]
    public GameObject[] rushTexts;
    private float timer = 0f;
    private float maxWaitTime = 30f;
    private bool isCoroutineRunning = false;

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(2, -7);
            Vector2Int size = new Vector2Int(this.boardSize.x - 2, this.boardSize.y+7);
            return new RectInt(position, size);
        }
    }

    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();
        for (int i = 0; i < tetrominoes.Length; i++)
        {
            this.tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        SpawnPiece();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= maxWaitTime && !isCoroutineRunning)
        {
            StartCoroutine(ActivateRandomObject());
            timer = 0f;
        }
    }

    public void SpawnPiece()
    {
        int random = Random.Range(0, this.tetrominoes.Length);
        TetrominoData data = this.tetrominoes[random];
        this.activePiece.Initialize(this, this.spawnPosition, data);
        if(IsValidPosition(this.activePiece, this.spawnPosition))
        {
            Set(this.activePiece);
        }
        else
        {
            GameOver(); 
        }
            Set(this.activePiece);

    }
    private void GameOver()
    {
        SceneManager.LoadScene("LoseScene");
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++) { 
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }
    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = this.Bounds;
        for (int i = 0;i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition)) {
                return false;
            }
            if (this.tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }
        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = this.Bounds;
        int row = bounds.yMin;
        bool lineCleared = false;

        while (row < bounds.yMax)
        {
            if (IsLineFull(row)){
                audioSource.PlayOneShot(lineClearSFX);
                LineClear(row);
                lineCleared = true;
            }
            else
            {
                row++;
            }
            if (lineCleared)
                timer = 0f;
        }
    }

    IEnumerator ActivateRandomObject()
    {
        isCoroutineRunning = true;

        if (rushTexts.Length > 0)
        {
            int index = Random.Range(0, rushTexts.Length);
            rushTexts[index].SetActive(true);
            yield return new WaitForSeconds(3f);
            rushTexts[index].SetActive(false);
        }

        isCoroutineRunning = false;
    }

    private bool IsLineFull(int row)
    {

        RectInt bounds = this.Bounds;
        for(int col  = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col,row,0);
            if (!this.tilemap.HasTile(position)){
                return false;
            }
        }
        return true;
    }

    private void LineClear(int row)
    {
        RectInt bounds = this.Bounds;
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            this.tilemap.SetTile(position, null);
        }

        while (row < bounds.yMax)
        {
            for(int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row+1, 0);
                TileBase above = this.tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                this.tilemap.SetTile(position,above);
            }
            row++;
        }
        if (gameManager != null)
        {
            gameManager.AddPoints();
        }
    }
    public void ClearAllTiles()
    {
        this.tilemap.ClearAllTiles();
    }
}
