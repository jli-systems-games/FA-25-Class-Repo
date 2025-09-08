using UnityEngine;
using System.Collections;

public class FallController : MonoBehaviour
{
    public GameObject heartPrefab;   
    public float interval = 0.35f;  
    public float spawnY = 6f;       
    public float xMin = -8f, xMax = 8f; 
    public CollectManager manager;   

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            var pos = new Vector3(Random.Range(xMin, xMax), spawnY, 0f);
            var go = Instantiate(heartPrefab, pos, Quaternion.identity);

            // 하트가 매니저를 알도록 주입
            var h = go.GetComponent<FallingObject>();
            if (h != null) h.manager = manager;

            yield return new WaitForSeconds(interval);
        }
    }
}