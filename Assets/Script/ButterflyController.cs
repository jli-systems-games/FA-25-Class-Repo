using UnityEngine;

public class ZAxisMover : MonoBehaviour
{
 
    public float moveSpeed = 5f;
    public float maxZDistance = 5f;

    private float startZ;
    private bool isActive = false;  

    void Start()
    {
        startZ = transform.position.z;
    }

    void Update()
    {
        if (!isActive) return;  

        float input = Input.GetAxis("Horizontal"); 

        if (Mathf.Abs(input) > 0.01f)
        {
            float newZ = transform.position.z + input * moveSpeed * Time.deltaTime;
            newZ = Mathf.Clamp(newZ, startZ - maxZDistance, startZ + maxZDistance);

            transform.position = new Vector3(transform.position.x, transform.position.y, newZ);
        }
    }

    public void EnableMovement()
    {
        isActive = true;
    }


    public void DisableMovement()
    {
        isActive = false;
    }
}