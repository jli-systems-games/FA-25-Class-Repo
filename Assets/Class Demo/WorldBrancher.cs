using System.Collections.Generic;
using UnityEngine;

public class WorldBrancher : MonoBehaviour
{
    public GameObject startingRoom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3 tempPosition = startingRoom.transform.position;

        GameObject tempRoom = Instantiate(startingRoom, tempPosition, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
