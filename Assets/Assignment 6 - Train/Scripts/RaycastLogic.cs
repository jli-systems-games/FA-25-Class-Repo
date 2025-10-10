using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaycastLogic : MonoBehaviour
{
    public Camera playerCamera;
    public float interactionDistance = 3f;
    public Canvas interactionCanvas;
    public Canvas ticketCanvas;
    public LayerMask interactableLayer;
    public Canvas endCanvas;
    public GameObject notFinishText;

    public bool hasCheckedTrue = false;
    public bool hasCheckedFalse = false;

    private TicketManager currentTicketManager;

    public CheckTicketStatus checkTicketStatus;

    void Start()
    {
        interactionCanvas.gameObject.SetActive(false);
        ticketCanvas.gameObject.SetActive(false);
        endCanvas.gameObject.SetActive(false);
        notFinishText.SetActive(false);
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
                TicketManager ticketManager = hit.collider.GetComponent<TicketManager>();

                if (ticketManager != null)
                {
                    currentTicketManager = ticketManager;
                }

                Debug.Log("Found passenger");
                interactionCanvas.gameObject.SetActive(true);
                endCanvas.gameObject.SetActive(false);
                notFinishText.SetActive(false);
            }
            else if (hit.collider.CompareTag("End Game"))
            {
                endCanvas.gameObject.SetActive(true);
                interactionCanvas.gameObject.SetActive(false);

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    if (checkTicketStatus.HaveAllPassengersBeenChecked())
                    {
                        if (checkTicketStatus.HasPlayerMadeMistake())
                        {
                            SceneManager.LoadScene("Game Over Scene");
                        }
                        else if (!checkTicketStatus.HasPlayerMadeMistake())
                        {
                            SceneManager.LoadScene("Complete Scene");
                        }
                    }
                    else
                    {
                        notFinishText.SetActive(true);
                    }
                }
            }
        }
        else
        {
            interactionCanvas.gameObject.SetActive(false);
            endCanvas.gameObject.SetActive(false);
            notFinishText.SetActive(false);
        }

        if (interactionCanvas.isActiveAndEnabled)
        {
            hasCheckedFalse = false;
            hasCheckedTrue = false;

            if (Input.GetKeyDown(KeyCode.E))
            {
                ShowTicket(currentTicketManager);
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
            else if (Input.GetKeyDown(KeyCode.F))
            {
                hasCheckedFalse = true;
                ProcessTicket(currentTicketManager);
            }
            else if (Input.GetKey(KeyCode.T))
            {
                hasCheckedTrue = true;
                ProcessTicket(currentTicketManager);
            }
        }
    }

    private void ShowTicket(TicketManager ticketManager)
    {
        ticketManager.ShowTicketUI(ticketCanvas.gameObject);
    }

    private void ProcessTicket(TicketManager ticketManager)
    {
        ticketManager.CheckPassengerStatus();

        ticketCanvas.gameObject.SetActive(false);
    }
}
