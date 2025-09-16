using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ZoneChecker : MonoBehaviour
{
    public string safeZoneTag = "SafeZone";
    public PeeGameController game;   // мо PeeGameController

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(safeZoneTag))
            game.SetInZone(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(safeZoneTag))
            game.SetInZone(false);
    }
}
