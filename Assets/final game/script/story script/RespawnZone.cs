using UnityEngine;

public class RespawnZone : MonoBehaviour
{
    public Transform respawnPoint;
    public string targetTag = "Player";

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("RespawnZone hit: " + other.name);

        Transform root = other.attachedRigidbody ? other.attachedRigidbody.transform : other.transform.root;

        if (!root.CompareTag(targetTag))
            return;

        if (respawnPoint == null)
        {
            Debug.LogWarning("RespawnZone: respawnPoint is not assigned.");
            return;
        }

        CharacterController cc = root.GetComponent<CharacterController>();

        if (cc != null)
        {
            cc.enabled = false;                      
            root.position = respawnPoint.position;   
            cc.enabled = true;                      
        }
        else
        {
            root.position = respawnPoint.position;
        }

        Rigidbody rb = root.GetComponent<Rigidbody>();
        if (rb != null && !rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
