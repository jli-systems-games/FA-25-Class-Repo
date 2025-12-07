using UnityEngine;

public class InteractHintUI : MonoBehaviour
{
    public static InteractHintUI Instance;

    [Header("iconRect")]
    public RectTransform iconRect;

    [Header("worldHeightOffset")]
    public float worldHeightOffset = 1.5f;

    private string currentInteractionId = null;
    private Transform currentTarget = null;

    private Camera mainCam;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        mainCam = Camera.main;

        if (iconRect != null)
        {
            iconRect.gameObject.SetActive(false); 
        }
    }

    private void LateUpdate()
    {
        if (currentTarget == null || iconRect == null || mainCam == null) return;

        Vector3 worldPos = currentTarget.position + new Vector3(0, worldHeightOffset, 0);
        Vector3 screenPos = mainCam.WorldToScreenPoint(worldPos);

        iconRect.position = screenPos;
    }

    public void ShowHint(string interactionId, Transform target)
    {
        currentInteractionId = interactionId;
        currentTarget = target;

        if (iconRect != null)
        {
            iconRect.gameObject.SetActive(true);
        }
    }

    public void HideHint(string interactionId)
    {
        if (interactionId != currentInteractionId) return;

        currentInteractionId = null;
        currentTarget = null;

        if (iconRect != null)
        {
            iconRect.gameObject.SetActive(false);
        }
    }
}
