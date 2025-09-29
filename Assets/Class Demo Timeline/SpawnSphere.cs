using UnityEngine;

public class SpawnSphere : MonoBehaviour
{
    public GameObject sphere;
    public BoxCollider spawnArea;


    // Update is called once per frame
    public void SpawnSpheres()
    {
        Vector3 randomPoint = GetRandomPointInBounds(spawnArea.bounds);

        Instantiate(sphere, randomPoint, Quaternion.identity);
    }

    private Vector3 GetRandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}
