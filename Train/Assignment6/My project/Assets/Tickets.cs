using System;

[Serializable]
public class Ticket
{
    public string destination;
    public string date;
    public bool stampValid;

    public bool IsValid(string today, string[] validDestinations)
    {
        bool dateOk = (date == today);
        bool destOk = false;
        foreach (var d in validDestinations)
        {
            if (string.Equals(d, destination, StringComparison.OrdinalIgnoreCase))
            {
                destOk = true;
                break;
            }
        }
        bool stampOk = stampValid;
        return dateOk && destOk && stampOk;
    }
}
