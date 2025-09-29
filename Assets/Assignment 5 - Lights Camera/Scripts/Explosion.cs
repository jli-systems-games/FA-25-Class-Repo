using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    //Explosion Force Varables
    public float explosionRadius = 2600f;
    public float explosionForce = 700f;
    public float upwardForce = 1f;
    [Space(10)]

    //Random Explosion Force Variables
    public float minRandomForce = 20000f;
    public float minRandomTorque = 10000f;
    public float maxRandomForce = 50000f;
    public float maxRandomTorque = 20000f;

    public GameObject explosionParticle;

    private bool isInExplosion = false;

    public Timer timer;

    private void Start()
    {
        explosionParticle.SetActive(false);
        StartCoroutine(TimeBeforeExplosion(timer.currentTime));        
    }

    private IEnumerator TimeBeforeExplosion(float delay)
    {
        yield return new WaitForSeconds(delay);

        Explode();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Car"))
        {
            isInExplosion = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Car"))
        {
            isInExplosion = false;
        }
    }

    public void Explode()
    {
        explosionParticle.SetActive(true);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            Debug.Log(rb);

            if (rb != null)
            {
                if (!isInExplosion)
                {
                    Vector3 direction = (rb.transform.position - transform.position).normalized;

                    float distance = Vector3.Distance(transform.position, rb.transform.position);
                    float forceMultiplier = 1f - Mathf.Clamp01(distance / explosionRadius);

                    Vector3 finalForce = direction * (explosionForce * forceMultiplier) + Vector3.up * upwardForce;

                    rb.AddForce(finalForce);
                }
                else
                {
                    Vector3 randomForceVector = new Vector3(
                        Random.Range(minRandomForce, maxRandomForce),
                        Random.Range(minRandomForce, maxRandomForce),
                        Random.Range(minRandomForce, maxRandomForce)
                    );

                    Vector3 randomTorqueVector = new Vector3(
                        Random.Range(minRandomTorque, maxRandomTorque),
                        Random.Range(minRandomTorque, maxRandomTorque),
                        Random.Range(minRandomTorque, maxRandomTorque)
                    );

                    rb.AddForce(randomForceVector, ForceMode.Impulse);
                    rb.AddTorque(randomTorqueVector, ForceMode.Impulse);
                }
            }
        }
    }
}
