using UnityEngine;

public class RoomAdapter : MonoBehaviour
{
    public GameObject[] roomPool;

    void Start()
    {
        if (roomPool == null || roomPool.Length == 0) return;
        var pf = roomPool[Random.Range(0, roomPool.Length)];
        Instantiate(pf, transform.position, transform.rotation, transform.parent);
        Destroy(gameObject);
    }
}
