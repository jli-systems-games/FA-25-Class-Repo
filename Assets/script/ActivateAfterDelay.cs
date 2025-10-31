using UnityEngine;

public class ActivateAfterDelay : MonoBehaviour
{
    public GameObject[] enableList;
    public GameObject[] disableList;

    [Header("wait time (sec)")]
    public float waitTime = 25.5f;

    void Start()
    {
        Invoke(nameof(doSwitch), waitTime);
    }

    void doSwitch()
    {
    
        foreach (var o in enableList)
        {
            if (o != null)
                o.SetActive(true);
        }

     
        foreach (var o in disableList)
        {
            if (o != null)
                o.SetActive(false);
        }

    }
}