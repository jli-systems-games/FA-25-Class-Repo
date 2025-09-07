using System.Collections.Generic;
using UnityEngine;

public class GridPlacementSystem : MonoBehaviour
{
    [SerializeField]
    GameObject mouseIndicator, cellIndicator;
    [SerializeField]
    private GridInputManager gridInputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private GameObject paintCellPrefab;
    private Dictionary<Vector3Int, GameObject> paintedCells = new Dictionary<Vector3Int, GameObject>();

    private void Update()
    {
        Vector3 mousePosition = gridInputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        mouseIndicator.transform.position = mousePosition;
        cellIndicator.transform.position = grid.CellToWorld(gridPosition);
        //Code from https://www.youtube.com/watch?v=l0emsAHIBjU&list=PLcRSafycjWFepsLiAHxxi8D_5GGvu6arf

        if (Input.GetMouseButton(0))
        {
            if (!paintedCells.ContainsKey(gridPosition))
            {
                Vector3 worldPosition = grid.CellToWorld(gridPosition);

                GameObject newPaintedCell = Instantiate(paintCellPrefab, worldPosition, Quaternion.identity);

                paintedCells.Add(gridPosition, newPaintedCell);

                if (paintedCells.Count == 100)
                {
                    Debug.Log("Painting Complete!");
                }
                else if (paintedCells.Count == 50 && IsCheckerPattern())
                {
                    Debug.Log("Checkerboard Complete!");
                }
                else if (paintedCells.Count == 50 && IsVerticalStripes())
                {
                    Debug.Log("Vertical Stripes Complete!");
                }
                else if (paintedCells.Count == 50 && IsHorizontalStripes())
                {
                    Debug.Log("Horizontal Stripes Complete!");
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
}
