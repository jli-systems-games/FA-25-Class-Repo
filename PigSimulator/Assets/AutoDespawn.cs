using UnityEngine;

public class AutoAddDespawnOnTag : MonoBehaviour
{
    public string targetTag = "Knockable";
    public float speedThreshold = 0.5f;
    public float distanceThreshold = 0.15f;
    public float lifetimeAfterTriggered = 5f;
    public bool destroyObject = true;

    void Start()
    {
        var all = GameObject.FindGameObjectsWithTag(targetTag);
        for (int i = 0; i < all.Length; i++)
        {
            var go = all[i];
            var d = go.GetComponent<TimedDespawnOnMotion>();
            if (!d) d = go.AddComponent<TimedDespawnOnMotion>();
            d.speedThreshold = speedThreshold;
            d.distanceThreshold = distanceThreshold;
            d.lifetimeAfterTriggered = lifetimeAfterTriggered;
            d.destroyObject = destroyObject;
        }
    }
}
