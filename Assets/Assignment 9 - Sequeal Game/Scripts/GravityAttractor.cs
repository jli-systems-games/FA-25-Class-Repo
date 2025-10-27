using UnityEngine;
using System.Collections;

public class GravityAttractor : MonoBehaviour
{
    public float gravity = -10f;
   
    public void Attract(Transform body)
    {
        Vector3 targetDirection = (body.position - transform.position).normalized; //direction between body and the center of the planet
        Vector3 bodyUp = body.up;
            
        body.rotation = Quaternion.FromToRotation(bodyUp, targetDirection) * body.rotation; //applying the rotation

        if (body.CompareTag("Player") || body.CompareTag("Grounded"))
        {
            body.GetComponent<Rigidbody>().AddForce(targetDirection * gravity); //applying downwards force to simulate gravity
        }
    }
}
