using UnityEngine;

public class ArenaBounds : MonoBehaviour
{
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Car"))
            Destroy(other.gameObject);
    }
}