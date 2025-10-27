using UnityEngine;

public class FoodFall : MonoBehaviour
{
    public FoodDataSO data;
    public string playerTag = "Player";
    public string recycleTag = "Killer";
    public AudioClip collectSound;



    void Update()
    {
        if (data != null)
        {
            transform.position += Vector3.down * data.fallSpeed * Time.deltaTime;
        }
        if (Camera.main)
        {
            float bottomY = Camera.main.transform.position.y - Camera.main.orthographicSize - 1f;
            if (transform.position.y < bottomY) Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            if (data != null)
            {
                GameState.Instance?.Add(data.deltaHappy, data.deltaHunger, data.deltaHealth);
            }
            
            if (collectSound && Camera.main)
                AudioSource.PlayClipAtPoint(collectSound, Camera.main.transform.position);
            
            Destroy(gameObject);
            return;
        }
        if (other.CompareTag(recycleTag)) Destroy(gameObject);
    }
}