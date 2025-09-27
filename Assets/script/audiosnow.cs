using UnityEngine;

public class audiosnow : MonoBehaviour
{
    public AudioSource snow;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        
            snow.Play();

        

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
