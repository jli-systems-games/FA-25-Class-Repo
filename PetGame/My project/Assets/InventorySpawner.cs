using UnityEngine;

public class InventorySpawner : MonoBehaviour
{
    public GameObject beerPrefab;
    public Transform worldAnchor;
    public Vector2 screenMargin = new Vector2(40, 40);
    public float spawnHeight = 0.2f;

    public void SpawnBeer()
    {
        if (!beerPrefab) return;
        Vector3 pos;
        if (worldAnchor) pos = worldAnchor.position;
        else pos = ScreenToGround(new Vector2(Screen.width - screenMargin.x, screenMargin.y));
        pos.y = spawnHeight;
        Instantiate(beerPrefab, pos, Quaternion.identity);
    }

    Vector3 ScreenToGround(Vector2 screenPos)
    {
        var cam = Camera.main;
        Ray r = cam.ScreenPointToRay(screenPos);
        new Plane(Vector3.up, Vector3.zero).Raycast(r, out float d);
        return r.GetPoint(d);
    }
}
