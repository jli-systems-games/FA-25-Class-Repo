using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class GravityBody : MonoBehaviour
{
    GravityAttractor currentPlanet;

    private void Start()
    {
        SetPlanet(FindClosestPlanet());
    }

    private void FixedUpdate()
    {
        if (currentPlanet != null)
        {
            currentPlanet.Attract(transform);
        }
        
    }

    public void SetPlanet(GravityAttractor newPlanet)
    {
        currentPlanet = newPlanet;
    }

    private GravityAttractor FindClosestPlanet()
    {
        GravityAttractor[] planets = Object.FindObjectsByType<GravityAttractor>(FindObjectsSortMode.None);
        GravityAttractor closestPlanet = null;
        float minDistance = Mathf.Infinity;

        foreach (var planet in planets)
        {
            float distance = Vector3.Distance(transform.position, planet.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestPlanet = planet;
            }
        }

        return closestPlanet;
    }

}
