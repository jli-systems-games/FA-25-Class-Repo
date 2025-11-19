using UnityEngine;

public class WaterDrop : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float maxDistance;
    private Vector2 startPos;

    public void Init(Vector2 dir, float spd, float range)
    {
        direction = dir;
        speed = spd;
        maxDistance = range;
        startPos = transform.position;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        if (Vector2.Distance(startPos, transform.position) >= maxDistance)
            Destroy(gameObject);
    }
}
