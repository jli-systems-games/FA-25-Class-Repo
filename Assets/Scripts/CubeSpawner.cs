using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public GameObject cube;
    public Transform spawnTransform;


    public void SpawnCube()
    {
        GameObject temp = Instantiate(cube);
        temp.transform.position = spawnTransform.position;
    }
}
