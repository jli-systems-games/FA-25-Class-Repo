using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitPlacement : MonoBehaviour
{
    public Camera cam;
    public Tilemap tilemap;
    public GameObject unitPrefab;
    public MoneyManager moneyManager;
    public int unitCost = 20;

    private GameObject ghostUnit;

    void Update()
    {
        if (ghostUnit != null)
        {
            Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;

            Vector3Int cellPos = tilemap.WorldToCell(mouseWorld);
            ghostUnit.transform.position = tilemap.GetCellCenterWorld(cellPos);

            // Left-click to place
            if (Input.GetMouseButtonDown(0))
            {
                if (cellPos.x < 0) // only allow on player's side
                {
                    if (moneyManager.SpendMoney(unitCost))
                    {
                        Instantiate(unitPrefab, ghostUnit.transform.position, Quaternion.identity);
                        Destroy(ghostUnit);  // 👈 remove ghost after placing
                        ghostUnit = null;    // 👈 clear reference
                    }
                }
            }

            // Right-click to cancel placement
            if (Input.GetMouseButtonDown(1))
            {
                Destroy(ghostUnit);
                ghostUnit = null;
            }
        }
    }

    public void SelectUnitToPlace()
    {
        if (ghostUnit != null) Destroy(ghostUnit);
        ghostUnit = Instantiate(unitPrefab);
        ghostUnit.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f); // ghost look
    }
}
