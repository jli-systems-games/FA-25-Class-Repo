using System.Collections;
using UnityEngine;

public class DisableZone : MonoBehaviour
{
    private static int disableZoneCounter = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Disable Zone"))
        {
            disableZoneCounter++;

            Data.inDisableZone = disableZoneCounter > 0;
            Debug.Log("Entered disable zone. Count: " + disableZoneCounter);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Disable Zone"))
        {
            disableZoneCounter = Mathf.Max(0, disableZoneCounter - 1);

            Data.inDisableZone = disableZoneCounter > 0;
            Debug.Log("Exited disable zone. Count: " + disableZoneCounter);
        }
    }
}