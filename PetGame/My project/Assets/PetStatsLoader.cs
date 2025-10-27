using UnityEngine;

public class PetStatsLoader : MonoBehaviour
{
    public PetStatsSO stats;
    public HUDInventoryUI hud;
    public CrewController crew;

    void Awake()
    {
        if (!hud) hud = FindFirstObjectByType<HUDInventoryUI>();
        if (!crew) crew = FindFirstObjectByType<CrewController>();
        if (stats && hud)
        {
            hud.drainSeconds = stats.drainSeconds;
        }
        if (stats && crew)
        {
            crew.deathReloadDelay = stats.deathReloadDelay;
        }
    }
}
