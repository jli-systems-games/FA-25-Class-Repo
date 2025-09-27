using UnityEngine;

public class shovel : MonoBehaviour
{
    public GameObject ice;
    public GameObject icebefore;
    public GameObject shovels;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "shovel")
        {
            ice.SetActive(true);
            icebefore.SetActive(false);
            shovels.SetActive(false);
        }
           

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
