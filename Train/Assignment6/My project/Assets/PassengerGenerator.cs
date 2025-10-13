using UnityEngine;

public static class PassengerGenerator
{
    private static readonly string[] poolValidDest = { "Tokyo", "Berlin", "Paris" };
    private static readonly string[] poolInvalidDest = { "Toiyo", "Pariss", "Moon", "Nowhere" };

    public static string[] ValidDestinations => poolValidDest;

    public static Passenger Create(string today, Sprite[] portraitPool)
    {
        var passenger = new Passenger();
        if (portraitPool != null && portraitPool.Length > 0)
        {
            int idx = Random.Range(0, portraitPool.Length);
            passenger.portrait = portraitPool[idx];
        }
        else
        {
            passenger.portrait = null;
        }

        var ticket = new Ticket
        {
            destination = poolValidDest[Random.Range(0, poolValidDest.Length)],
            date = today,
            stampValid = true
        };

        if (Random.value < 0.30f)
        {
            int which = Random.Range(0, 3);
            switch (which)
            {
                case 0: ticket.destination = poolInvalidDest[Random.Range(0, poolInvalidDest.Length)]; break;
                case 1: ticket.date = "2099-01-01"; break;
                case 2: ticket.stampValid = false; break;
            }
        }

        passenger.ticket = ticket;
        passenger.isTicketValid = ticket.IsValid(today, poolValidDest);
        return passenger;
    }
}
