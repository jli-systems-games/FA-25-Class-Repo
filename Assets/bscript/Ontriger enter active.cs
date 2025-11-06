using UnityEngine;

public class Ontrigerenteractive : MonoBehaviour
{
    public GameObject active;
    public GameObject deactive;
    public AudioSource interactsound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            active.SetActive(true);
            deactive.SetActive(false);
            interactsound.Play();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
