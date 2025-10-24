using UnityEngine;

public class OrbManager : MonoBehaviour
{
    public OrbThrower orbThrower;

    public GameStat statAsset;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Slime") || other.gameObject.CompareTag("Ground"))
        {
            orbThrower.ResetOrb();
        }

        if (other.gameObject.CompareTag("Slime"))
        {
            statAsset.hungerStat += Data.feedAmount;
        }
    }
}
