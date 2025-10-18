using UnityEngine;

public class Seed : MonoBehaviour
{
    private bool planted = false;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (planted) return;

        if (col.collider.CompareTag("Soil"))
        {
            planted = true;
            FindObjectOfType<SeedDropper>().PlantSeed();
            Destroy(gameObject);
        }
        else if (col.collider.CompareTag("Rock"))
        {
            Destroy(gameObject);
        }
    }
}
