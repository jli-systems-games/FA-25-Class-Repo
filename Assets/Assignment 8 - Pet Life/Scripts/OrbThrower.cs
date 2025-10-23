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

    public float movementRange = 1.85f;
    public float movementSpeed = 1.5f;

    private bool isThrown = false;

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

    public void ResetOrb()
    {
        if (orb != null)
        {
            angle = Vector3.zero;
            orbRB.linearVelocity = Vector3.zero;
            orbRB.angularVelocity = Vector3.zero;
            orbRB.useGravity = false;
            orb.transform.position = transform.position;
            isThrown = false;
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
                    isThrown = true;

                    angle = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));

                    orbRB.AddForce(new Vector3(0, 90, -angle.z * orbSpeed));
                    orbRB.useGravity = true;
                }
            }
        }
        //Got code for ball throw from https://www.youtube.com/watch?v=fljP8zJ75Lk

        MoveOrbSideToSide();
    }
    void MoveOrbSideToSide()
    {
        if (!isThrown)
        {
            float pingPongValue = Mathf.PingPong(Time.time * movementSpeed, movementRange * 2f);
            float newX = pingPongValue - movementRange;

            if (orb != null)
            {
                Vector3 newPosition = new Vector3(newX, orb.transform.position.y, orb.transform.position.z);
                orb.transform.position = newPosition;
            }
        }
    }
}