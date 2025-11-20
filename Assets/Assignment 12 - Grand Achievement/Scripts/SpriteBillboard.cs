using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.forward = Camera.main.transform.forward;

        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.x = 0;
        rotation.z = 0;
        transform.rotation = Quaternion.Euler(rotation);
        //Code from https://www.youtube.com/watch?v=eiGvVgwtJ8k
    }
}
