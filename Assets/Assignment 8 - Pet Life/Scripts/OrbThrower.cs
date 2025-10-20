using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class OrbThrower : MonoBehaviour
{
    private GameObject orb;
    private Rigidbody orbRB;

    public float orbSpeed = 70;
    private Vector3 angle;
    public LayerMask interactableLayer;

    private void Start()
    {
        SetUpOrb();
    }

    void SetUpOrb()
    {
        orb = GameObject.FindGameObjectWithTag("Orb");
        orbRB = orb.GetComponent<Rigidbody>();
        ResetOrb();
    }

    void ResetOrb()
    {
        if (orb != null)
        {
            angle = Vector3.zero;
            orbRB.angularVelocity = Vector3.zero;
            orbRB.useGravity = false;
            orb.transform.position = transform.position;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, interactableLayer))
            {
                if (hit.transform == orb.transform)
                {
                    angle = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));

                    orbRB.AddForce(new Vector3(0, 90, -angle.z * orbSpeed));
                    orbRB.useGravity = true;
                }
            }
        }
    }
}