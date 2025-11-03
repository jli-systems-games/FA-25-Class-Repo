using UnityEngine;

public class WorldBrancher : MonoBehaviour
{
    public GameObject startingRoom;


    void Start()
    {
        Vector3 tempPosition = startingRoom.transform.position;

        GameObject tempRoom =
            Instantiate(startingRoom, startingRoom.transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
