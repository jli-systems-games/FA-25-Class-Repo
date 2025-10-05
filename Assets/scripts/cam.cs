using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
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
    public float rayDistance = 10f;

    private float rotationX;
    private Coroutine handRoutine;

    [Header("Score")]
    public TextMeshProUGUI scoreText;
    private int score = 0;

    [Header("End")]
    public Animator winAnimator;
    public string winTrigger = "Win";
    public float pauseDelay = 2f;
    public GameObject bgm;
    public GameObject end;
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
        if (score >= 20)
        {
            end.SetActive(true);
            StartCoroutine(WinSequence());
        }
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;


        transform.Rotate(Vector3.up * mouseX);


        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, minLookX, maxLookX);
        transform.localEulerAngles = new Vector3(rotationX, transform.localEulerAngles.y, 0);
    }

    void HandleAiming()
    {

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

            if (handRoutine != null) StopCoroutine(handRoutine);
            handRoutine = StartCoroutine(ShowHand());

            // Cast ray to detect NPC
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, rayDistance);

            if (hit.collider != null && hit.collider.CompareTag("NPC"))
            {
                NPCEmotion npc = hit.collider.GetComponent<NPCEmotion>();
                if (npc != null) npc.ChangeEmotion();
                score++;
                UpdateScoreUI();

            }
        }
    }

    IEnumerator ShowHand()
    {
        if (hand != null) hand.SetActive(true);
        yield return new WaitForSeconds(handVisibleTime);
        if (hand != null) hand.SetActive(false);
    }
    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "" + score;
        }
    }
    IEnumerator WinSequence()
    {
        
        if (winAnimator != null)
        {
            winAnimator.SetTrigger(winTrigger);
        }

       
        yield return new WaitForSecondsRealtime(pauseDelay);

       
        Time.timeScale = 0f;
    }
}