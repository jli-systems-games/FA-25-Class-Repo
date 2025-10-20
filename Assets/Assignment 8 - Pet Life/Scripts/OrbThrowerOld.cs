using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class OrbThrowerOld : MonoBehaviour
{
    private GameObject orb;
    private Rigidbody orbRB;

    float startTime, endTime, swipeDistance, swipeTime;
    private Vector2 startPos;
    private Vector2 endPos;

    public float minSwipDist = 0;
    private float orbVelocity = 0;
    private float orbSpeed = 0;
    public float maxOrbSpeed = 40;
    private Vector3 angle;

    private bool thrown, holding;
    private Vector3 newPosition;

    public float smooth = 0.7f;

    private bool hasUpdatedStartPos;

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
            endPos = Vector2.zero;
            startPos = Vector2.zero;
            orbSpeed = 0;
            startTime = 0;
            endTime = 0;
            swipeDistance = 0;
            swipeTime = 0;
            thrown = holding = false;
            orbRB.angularVelocity = Vector3.zero;
            orbRB.useGravity = false;
            orb.transform.position = transform.position;
            hasUpdatedStartPos = false;
        }
    }

    void PickUpOrb()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane * 8f; //how close the orb is to the cam
        newPosition = Camera.main.ScreenToWorldPoint(mousePos);
        orb.transform.localPosition = Vector3.Lerp(orb.transform.localPosition, newPosition, 80f * Time.deltaTime);
    }
    private void Update()
    {
        if (holding) PickUpOrb();

        if (thrown) return;

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("is mouse click");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, interactableLayer))
            {
                if (hit.transform == orb.transform)
                {
                    startTime = Time.time;
                    startPos = Input.mousePosition;
                    holding = true;

                    Debug.Log("is holding");

                    if (!hasUpdatedStartPos)
                    {
                        StartCoroutine(UpdateStartPosOnHold(1f));
                    }
                    else if (hasUpdatedStartPos)
                    {
                        hasUpdatedStartPos = false;
                    }
                    //Update the start position every second
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            endTime = Time.time;
            endPos = Input.mousePosition;
            swipeDistance = (endPos - startPos).magnitude;
            swipeTime = endTime - startTime;

            if (swipeDistance > 1f) //check if there was flick
            {
                CalculateSpeed();
                CalculateAngle();

                orbRB.AddForce(new Vector3(angle.x * orbSpeed, angle.y * orbSpeed, -angle.z * orbSpeed));
                orbRB.useGravity = true;

                holding = false;
                thrown = true;

                Invoke("ResetOrb", 5f);
            }
            else //drop orb if there was no flick
            {
                if (holding)
                {
                    orbRB.useGravity = true;
                    Invoke("ResetOrb", 2f);

                    thrown = true;
                    holding = false;
                }
            }
        }
    }

    private IEnumerator UpdateStartPosOnHold(float delay)
    {
        yield return new WaitForSeconds(delay);

        startPos = Input.mousePosition;
        startTime = Time.time;
        hasUpdatedStartPos = true;
    }

    void CalculateSpeed()
    {
        if (swipeTime > 0)
        {
            orbVelocity = swipeDistance / (swipeDistance - swipeTime);
        }

        orbSpeed = orbVelocity * 40f;

        if (orbSpeed >= maxOrbSpeed)
        {
            orbSpeed = maxOrbSpeed;
        }

        swipeTime = 0;
    }

    void CalculateAngle()
    {
        angle = Camera.main.ScreenToWorldPoint(new Vector3(endPos.x, endPos.y + 50f, (Camera.main.nearClipPlane + 5f)));
    }
}