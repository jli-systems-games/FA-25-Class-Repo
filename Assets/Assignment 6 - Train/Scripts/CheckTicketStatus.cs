using UnityEngine;
using System.Linq;

public class CheckTicketStatus : MonoBehaviour
{
    private TicketManager[] allTicketManagers;

    void Awake()
    {
        allTicketManagers = FindObjectsOfType<TicketManager>();
    }

    public bool HaveAllPassengersBeenChecked()
    {
        foreach (TicketManager passengerManager in allTicketManagers)
        {
            Collider passengerCollider = passengerManager.GetComponent<Collider>();

            if (passengerCollider != null && passengerCollider.enabled)
            {
                return false;
            }
        }

        return true;
    }

    public bool HasPlayerMadeMistake()
    {
        bool mistakeCaught = allTicketManagers.Any(tm => tm.isMistakenCaught);
        bool mistakeFine = allTicketManagers.Any(tm => tm.isMistakeFine);

        return mistakeCaught || mistakeFine;
    }
}
