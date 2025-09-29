using UnityEngine;

public class car : MonoBehaviour
{
    [Header("Movement Speed")]
    public float speed = 5f;   

    void Update()
    {
       
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
