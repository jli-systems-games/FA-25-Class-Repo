using UnityEngine;

public class LocationTrigger : MonoBehaviour
{
    public LocationType locationType;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var missionManager = FindObjectOfType<MissionManager>();
            if (missionManager != null)
            {
                missionManager.OnEnterLocation(locationType);
            }
        }
    }
}
