using UnityEngine;

public class ActivateObjectOnKey : MonoBehaviour
{
    [Header("要激活的物体")]
    public GameObject targetObject;

    [Header("触发按键")]
    public KeyCode activationKey = KeyCode.Space;

    void Update()
    {
        if (Input.GetKeyDown(activationKey))
        {
            if (targetObject != null)
            {
                targetObject.SetActive(true);
            }
        }
    }
}