using UnityEngine;

public class BallOutOfBounds : MonoBehaviour
{
    public float minY = -5f;  

    void Update()
    {
        if (transform.position.y < minY)
        {
            Destroy(gameObject);
        }
    }
}
