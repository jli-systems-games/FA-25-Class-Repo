using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float explosionRadius = 5f;
    public float explosionForce = 700f;
    public float upwardForce = 200f;

    public GameObject explosionEffect;

    // A function to trigger the explosion
    public void Explode()
    {
        // Find all colliders within the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        // Loop through each collider found
        foreach (Collider hit in colliders)
        {
            // Check if the collider has a Rigidbody component
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // Calculate the direction vector from the explosion center to the object
                Vector3 direction = (rb.transform.position - transform.position).normalized;

                // Calculate a force that diminishes with distance
                float distance = Vector3.Distance(transform.position, rb.transform.position);
                float forceMultiplier = 1f - Mathf.Clamp01(distance / explosionRadius);

                // Apply a combined force: a directional force and an upward lift
                Vector3 finalForce = direction * (explosionForce * forceMultiplier) + Vector3.up * upwardForce;

                // Apply the force to the rigidbody
                rb.AddForce(finalForce);
            }
        }

        // Optional: Instantiate the visual effect and destroy it after a short time
        if (explosionEffect != null)
        {
            GameObject effectInstance = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(effectInstance, 2f); // Destroys the effect after 2 seconds
        }

        // Destroy the explosion sphere itself after the effect is triggered
        Destroy(gameObject);
    }

    // Optional: Use this to visualize the explosion radius in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
