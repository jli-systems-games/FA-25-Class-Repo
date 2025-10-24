using UnityEngine;

public class OrbManager : MonoBehaviour
{
    public OrbThrower orbThrower;
    public int orbAmount;

    public GameStat statAsset;
    public int feedAmount = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Slime") || other.gameObject.CompareTag("Ground"))
        {
            orbAmount -= 1;

            if (orbAmount > 0)
            {
                orbThrower.ResetOrb();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        if (other.gameObject.CompareTag("Slime"))
        {
            statAsset.hungerStat += feedAmount;
        }
    }
}
