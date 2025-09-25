using UnityEngine;

public class entershow : MonoBehaviour
{
    public GameObject penguinKing;
    public GameObject bomb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "penguinKing")
        {
            bomb.SetActive(true);
            penguinKing.SetActive(false);

        }

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
