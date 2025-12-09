using System.Collections;
using UnityEngine;

public class DisableZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Disable Zone"))
        {
            Data.inDisableZone = true;
            Debug.Log("Entered disable zone");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Disable Zone"))
        {
            Data.inDisableZone = false;
            Debug.Log("Exited disable zone");
        }
    }
}