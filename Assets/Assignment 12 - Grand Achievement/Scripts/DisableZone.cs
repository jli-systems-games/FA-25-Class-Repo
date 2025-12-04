using UnityEngine;

public class DisableZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Disable Zone"))
        {
            Data.inDisableZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Disable Zone"))
        {
            Data.inDisableZone = false;
        }
    }
}
