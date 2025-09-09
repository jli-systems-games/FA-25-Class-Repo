using UnityEngine;

public class ConeCatcher : MonoBehaviour
{
    public CatchIceCreamMicrogame parent;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("IceCream"))
        {
            parent.NotifyCaught();
        }
    }
}
