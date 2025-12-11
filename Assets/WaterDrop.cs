using UnityEngine;

public class WaterDrop : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float maxDistance;
    private Vector2 startPos;

    public float lateralAmplitude = 0.15f;
    public float lateralFrequency = 6f;
    public float verticalJitterAmplitude = 0.05f;

    private Vector2 perpendicular;
    private float startTime;
    private float randomPhase;
    private GameManager gameManager;


    public void Init(Vector2 dir, float spd, float range)
    {
        gameManager = GameObject.FindAnyObjectByType<GameManager>();
        direction = dir.normalized;
        speed = spd;
        maxDistance = range;
        startPos = transform.position;

        perpendicular = new Vector2(-direction.y, direction.x);

        startTime = Time.time;
        randomPhase = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        float t = Time.time - startTime;

        float mainDist = speed * t;
        Vector2 mainPos = startPos + direction * mainDist;

        float accuracy = gameManager.GetAccuracyMultiplier();

        float lateralOffset = Mathf.Sin(t * lateralFrequency + randomPhase)
                              * lateralAmplitude * accuracy;

        float verticalOffset = Mathf.Sin(t * lateralFrequency * 0.5f + randomPhase * 1.37f)
                               * verticalJitterAmplitude * accuracy;


        Vector2 finalPos = mainPos
                           + perpendicular * lateralOffset
                           + direction * verticalOffset;

        transform.position = finalPos;

        if (Vector2.Distance(startPos, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }
}
