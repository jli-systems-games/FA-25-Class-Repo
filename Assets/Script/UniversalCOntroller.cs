using UnityEngine;

public class ToggleObjectsAndTrigger : MonoBehaviour
{
    [Header("Objects to Disable")]
    public GameObject[] objectsToDisable;

    [Header("Objects to Enable")]
    public GameObject[] objectsToEnable;

    [Header("Animator Settings")]
    public Animator targetAnimator;
    public string triggerName = "Trigger";

    [Header("Input Settings")]
    public KeyCode key = KeyCode.Space; 

    void Update()
    {

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(key))
        {
            ToggleObjects();
            TriggerAnimation();
        }
    }

    void ToggleObjects()
    {

        foreach (GameObject obj in objectsToDisable)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        foreach (GameObject obj in objectsToEnable)
        {
            if (obj != null)
                obj.SetActive(true);
        }
    }

    void TriggerAnimation()
    {
        if (targetAnimator != null && !string.IsNullOrEmpty(triggerName))
        {
            targetAnimator.SetTrigger(triggerName);
        }
    }
}