using UnityEngine;

public class StartForce : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        int randomForce = Random.Range(200, 400);
        Vector3 startForce = new Vector3(randomForce, randomForce, randomForce);

        rb.AddForce(startForce, ForceMode.Impulse);
        //Code from https://www.youtube.com/watch?v=ZOxnizAvMys&ab_channel=KetraGames
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
