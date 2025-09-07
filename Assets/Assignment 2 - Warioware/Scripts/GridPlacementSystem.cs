using System.Collections.Generic;
using Unity.VisualScripting;
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

                if (paintedCells.Count >= 100)
                {
                    Debug.Log("Painting Complete!");
                }
            }
        }
    }
}
