using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject cubePrefab;
    public Transform spawnPoint;
    public float moveInDuration = 2f;
    public Vector3 targetPosition = new Vector3(0f, 1f, 0f);

    public void SpawnCube()
    {
        if (!cubePrefab || !spawnPoint) return;
        var cube = Instantiate(cubePrefab, spawnPoint.position, Quaternion.identity);
        cube.transform.rotation = Quaternion.Euler(0f, 25f, 0f);
        StartCoroutine(MoveIn(cube.transform, targetPosition, moveInDuration));
    }

    private System.Collections.IEnumerator MoveIn(Transform t, Vector3 target, float dur)
    {
        Vector3 s = t.position;
        float el = 0f;
        while (el < dur)
        {
            el += Time.deltaTime;
            float k = Mathf.SmoothStep(0f, 1f, el / dur);
            t.position = Vector3.Lerp(s, target, k);
            yield return null;
        }
        t.position = target;
    }
}
