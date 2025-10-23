using UnityEngine;

public class FoodFall : MonoBehaviour
{
    public float fallSpeed = 2f;
    public float pauseChance = 0.3f;
    private bool isPaused = false;

    private float pauseTime;
    private float timer = 0f;

    private void Start()
    {
       
        if (Random.value < pauseChance)
        {
            isPaused = true;
            pauseTime = Random.Range(1f, 3f); 
            timer = 0f;
        }
    }

    private void Update()
    {
        if (isPaused)
        {
            timer += Time.deltaTime;
            if (timer >= pauseTime)
            {
                isPaused = false; 
            }
        }
        else
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        }

        // Destroy if below screen
        if (Camera.main.WorldToViewportPoint(transform.position).y < -0.2f)
        {
            Destroy(gameObject);
        }
    }
}
