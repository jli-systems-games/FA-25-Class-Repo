using UnityEngine;

public class MoverSpawner : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }
    //善良的小女孩正让她的画廊移动
}