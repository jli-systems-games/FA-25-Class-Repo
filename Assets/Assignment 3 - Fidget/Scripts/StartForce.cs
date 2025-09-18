using UnityEngine;
using UnityEngine.Audio;

public class StartForce : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip bounceAudio;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Rigidbody rb = GetComponent<Rigidbody>();

        int randomForce = Random.Range(50, 200);
        Vector3 startForce = new Vector3(randomForce, randomForce, randomForce);

        rb.AddForce(startForce, ForceMode.Impulse);
        //Code from https://www.youtube.com/watch?v=ZOxnizAvMys&ab_channel=KetraGames
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Hit player");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        audioSource.PlayOneShot(bounceAudio);
    }
}
