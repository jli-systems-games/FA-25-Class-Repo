using TMPro;
using UnityEngine;

public class RaycastLogic : MonoBehaviour
{
    public Camera playerCamera;
    public float interactionDistance = 3f;
    public Canvas interactionCanvas;
    public Canvas ticketCanvas;
    public LayerMask interactableLayer;

    void Start()
    {
        interactionCanvas.gameObject.SetActive(false);
        ticketCanvas.gameObject.SetActive(false);
    }
    
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.red);

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            if (hit.collider.CompareTag("Passenger"))
            {
                Debug.Log("Found passenger");
                interactionCanvas.gameObject.SetActive(true);
            }
            else
            {
                interactionCanvas.gameObject.SetActive(false);
            }
        }

        if (interactionCanvas.isActiveAndEnabled)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ticketCanvas.gameObject.SetActive(true);
            }
        }

        if (ticketCanvas.isActiveAndEnabled)
        {
            interactionCanvas.gameObject.SetActive(false);

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ticketCanvas.gameObject.SetActive(false);
            }
        }
    }
}
