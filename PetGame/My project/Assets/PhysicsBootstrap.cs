using UnityEngine;

public class PhysicsBootstrap : MonoBehaviour
{
    void Awake()
    {
        int crew = LayerMask.NameToLayer("Crew");
        int props = LayerMask.NameToLayer("Props");
        int inter = LayerMask.NameToLayer("Interactable");

        if (crew >= 0 && props >= 0) Physics.IgnoreLayerCollision(crew, props, true);
    
    }
}
