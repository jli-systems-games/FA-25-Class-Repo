using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FirstPersonAimInteract : MonoBehaviour
{
    [Header("Mouse Look")]
    public float sensitivity = 100f;
    public float maxLookX = 60f;
    public float minLookX = -60f;

    [Header("Hand UI")]
    public GameObject hand;
    public float handVisibleTime = 1f;

    [Header("Crosshair UI")]
    public GameObject defaultCrosshair;
    public GameObject npcCrosshair;

    [Header("Settings")]
    public float rayDistance = 10f; // max distance to detect NPC

    private float rotationX;
    private Coroutine handRoutine;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (hand != null) hand.SetActive(false);
        if (defaultCrosshair != null) defaultCrosshair.SetActive(true);
        if (npcCrosshair != null) npcCrosshair.SetActive(false);
    }

    void Update()
    {
        HandleMouseLook();
        HandleAiming();
        HandleClick();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        // rotate camera horizontally
        transform.Rotate(Vector3.up * mouseX);

        // rotate vertically
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, minLookX, maxLookX);
        transform.localEulerAngles = new Vector3(rotationX, transform.localEulerAngles.y, 0);
    }

    void HandleAiming()
    {
        // cast a ray from camera center
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, rayDistance);

        if (hit.collider != null && hit.collider.CompareTag("NPC"))
        {
            if (defaultCrosshair != null) defaultCrosshair.SetActive(false);
            if (npcCrosshair != null) npcCrosshair.SetActive(true);
        }
        else
        {
            if (defaultCrosshair != null) defaultCrosshair.SetActive(true);
            if (npcCrosshair != null) npcCrosshair.SetActive(false);
        }
    }

    void HandleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Show hand temporarily
            if (handRoutine != null) StopCoroutine(handRoutine);
            handRoutine = StartCoroutine(ShowHand());

            // Cast ray to detect NPC
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, rayDistance);

            if (hit.collider != null && hit.collider.CompareTag("NPC"))
            {
                NPCEmotion npc = hit.collider.GetComponent<NPCEmotion>();
                if (npc != null) npc.ChangeEmotion();
            }
        }
    }

    IEnumerator ShowHand()
    {
        if (hand != null) hand.SetActive(true);
        yield return new WaitForSeconds(handVisibleTime);
        if (hand != null) hand.SetActive(false);
    }
}
