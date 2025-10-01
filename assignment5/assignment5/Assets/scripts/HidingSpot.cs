using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var hide = other.GetComponent<PlayerHideState>();
        if (hide) hide.SetHidden(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var hide = other.GetComponent<PlayerHideState>();
        if (hide) hide.SetHidden(false);
    }
}
