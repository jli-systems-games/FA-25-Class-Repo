using UnityEngine;

public class WorldBrancherAdapter : MonoBehaviour
{
    public GameObject startingRoom;

    void Start()
    {
        if (startingRoom)
            Instantiate(startingRoom, transform.position, transform.rotation, transform.parent);
    }
}
