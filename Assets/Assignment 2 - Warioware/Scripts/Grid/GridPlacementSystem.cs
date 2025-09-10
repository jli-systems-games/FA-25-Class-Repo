using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridPlacementSystem : MonoBehaviour
{
    [SerializeField]
    GameObject mouseIndicator, cellIndicator;
    [SerializeField]
    private GridInputManager gridInputManager;
    [SerializeField]
    private Grid grid;

    public GameObject grid10;
    public GameObject grid4;

    private int paintMode;
    private int paintLevel;

    [SerializeField]
    private GameObject paintCellPrefab;
    private Dictionary<Vector3Int, GameObject> paintedCells = new Dictionary<Vector3Int, GameObject>();

    private bool isFullMode = false;
    private bool isVerticalMode = false;
    private bool isHorizontalMode = false;
    private bool isCheckerMode = false;

    private bool isEasyLevel = false;
    private bool isHardLevel = false;

    private Vector3 easyModeScale = new Vector3(2, 2, 1);
    private Vector3 hardModeScale = new Vector3(1, 1, 1);
    private Vector3 easyModePosition = new Vector3(1f, 1f, -0.05f);
    private Vector3 hardModePosition = new Vector3(0.5f, 0.5f, -0.05f);

    public Timer timer;
    public GameManager gameManager;
    public ParticleSystem confettiParticle;

    private void Start()
    {
        confettiParticle.gameObject.SetActive(false);

        grid10.SetActive(false);
        grid4.SetActive(false);

        paintMode = Data.globalPaintMode;
        paintLevel = Data.globalLevel;

        Debug.Log("Paint mode in grid script" + paintMode);

        if (paintMode == 0) isFullMode = true;
        else if (paintMode == 1) isVerticalMode = true;
        else if (paintMode == 2) isHorizontalMode = true;
        else if (paintMode == 3) isCheckerMode = true;

        if (paintLevel == 1 || paintLevel == 2) isEasyLevel = true;
        else if (paintLevel == 3 || paintLevel == 4) isHardLevel = true;

        if (isEasyLevel)
        {
            //Change grid and cell parameters to easy mode
            grid4.SetActive(true);
            grid.cellSize = new Vector3(2f, 2f, 2f);
            paintCellPrefab.transform.GetChild(0).localScale = easyModeScale;
            paintCellPrefab.transform.GetChild(0).position = easyModePosition;
            cellIndicator.transform.GetChild(0).localScale = easyModeScale;
            cellIndicator.transform.GetChild(0).position = easyModePosition;
        }
        else if (isHardLevel)
        {
            grid10.SetActive(true);
            grid.cellSize = new Vector3(1f, 1f, 1f);
            paintCellPrefab.transform.GetChild(0).localScale = hardModeScale;
            paintCellPrefab.transform.GetChild(0).position = hardModePosition;
            cellIndicator.transform.GetChild(0).localScale = hardModeScale;
            cellIndicator.transform.GetChild(0).position = hardModePosition;
        }
    }

    private void Update()
    {
        Vector3 mousePosition = gridInputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        mouseIndicator.transform.position = mousePosition;
        cellIndicator.transform.position = grid.CellToWorld(gridPosition);
        //Code from https://www.youtube.com/watch?v=l0emsAHIBjU&list=PLcRSafycjWFepsLiAHxxi8D_5GGvu6arf

        if (Input.GetMouseButton(0))
        {
            Debug.Log("Painted cells count: " + paintedCells.Count);

            if (!paintedCells.ContainsKey(gridPosition))
            {
                Vector3 worldPosition = grid.CellToWorld(gridPosition);

                GameObject newPaintedCell = Instantiate(paintCellPrefab, worldPosition, Quaternion.identity);

                paintedCells.Add(gridPosition, newPaintedCell);

                if (isEasyLevel)
                {
                    if (isFullMode)
                    {
                        if (paintedCells.Count == 16)
                        {
                            Debug.Log("Painting Complete!");
                            CompletePattern();
                        }
                    }
                    else if (isVerticalMode)
                    {
                        if (paintedCells.Count == 8 && IsVerticalStripes())
                        {
                            Debug.Log("Vertical Stripes Complete!");
                            CompletePattern();
                        }
                    }
                    else if (isHorizontalMode)
                    {
                        if (paintedCells.Count == 8 && IsHorizontalStripes())
                        {
                            Debug.Log("Horizontal Stripes Complete!");
                            CompletePattern();
                        }
                    }
                    else if (isCheckerMode)
                    {
                        if (paintedCells.Count == 8 && IsCheckerPattern())
                        {
                            Debug.Log("Checkerboard Complete!");
                            CompletePattern();
                        }
                    }
                }
                else if (isHardLevel)
                {
                    if (isFullMode)
                    {
                        if (paintedCells.Count == 100)
                        {
                            Debug.Log("Painting Complete!");
                            CompletePattern();
                        }
                    }
                    else if (isVerticalMode)
                    {
                        if (paintedCells.Count == 50 && IsVerticalStripes())
                        {
                            Debug.Log("Vertical Stripes Complete!");
                            CompletePattern();
                        }
                    }
                    else if (isHorizontalMode)
                    {
                        if (paintedCells.Count == 50 && IsHorizontalStripes())
                        {
                            Debug.Log("Horizontal Stripes Complete!");
                            CompletePattern();
                        }
                    }
                    else if (isCheckerMode)
                    {
                        if (paintedCells.Count == 50 && IsCheckerPattern())
                        {
                            Debug.Log("Checkerboard Complete!");
                            CompletePattern();
                        }
                    }
                }
            }
        }
    }

    private bool IsCheckerPattern()
    {
        //Get the parity of the first cell painted
        Vector3Int firstCell = new List<Vector3Int>(paintedCells.Keys)[0];
        int checkerParity = Mathf.Abs(firstCell.x + firstCell.y) % 2; //Add the cell's x and y coordinates, and get the remainder (if the sum is even it returns 0, if the sum is odd, it returns 1 (Use Abs to make sure -1 becomes 1 too))

        //Check the parity of each painted cell and compare them to the first cell's parity
        foreach (var cell in paintedCells.Keys) 
        {
            int currentParity = Mathf.Abs(cell.x + cell.y) % 2;
            if (currentParity != checkerParity)
            {
                return false;
            }
        }

        return true;
    }

    private bool IsVerticalStripes()
    {
        Vector3Int firstCell = new List<Vector3Int>(paintedCells.Keys)[0];
        int stripesXParity = Mathf.Abs(firstCell.x) % 2;

        foreach (var cell in paintedCells.Keys)
        {
            int currentXParity = Mathf.Abs(cell.x) % 2;

            if (currentXParity != stripesXParity)
            {
                return false;
            }
        }

        return true;
    }

    private bool IsHorizontalStripes()
    {
        Vector3Int firstCell = new List<Vector3Int>(paintedCells.Keys)[0];
        int stripesYParity = Mathf.Abs(firstCell.y) % 2;

        foreach (var cell in paintedCells.Keys)
        {
            int currentYParity = Mathf.Abs(cell.y) % 2;

            if (currentYParity != stripesYParity)
            {
                return false;
            }
        }

        return true;
    }

    private void CompletePattern()
    {
        isFullMode = false;
        isVerticalMode = false;
        isHorizontalMode = false;
        isCheckerMode = false;
        isEasyLevel = false;
        isHardLevel = false;

        Debug.Log("Pattern complete!");
        timer.isTimerRunning = false;
        Data.globalConsecutiveRound += 1;
        confettiParticle.gameObject.SetActive(true);
        StartCoroutine(CompletionDelay(2f));
    }

    private IEnumerator CompletionDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        grid10.SetActive(false);
        grid4.SetActive(false);
        gameManager.LoadRandomGame();
    }
}
