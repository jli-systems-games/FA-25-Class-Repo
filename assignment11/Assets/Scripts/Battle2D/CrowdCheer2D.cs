using UnityEngine;

public class CrowdCheer2D : MonoBehaviour
{
    [SerializeField] private ParticleSystem leftCheer;
    [SerializeField] private ParticleSystem rightCheer;

    public void CheerLeft() { if (leftCheer) leftCheer.Play(); }
    public void CheerRight() { if (rightCheer) rightCheer.Play(); }
}
