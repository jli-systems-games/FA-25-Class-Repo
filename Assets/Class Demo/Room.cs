using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<GameObject> roomPool =
        new List<GameObject>();

    void Start()
    {
        int tempIndex = Random.Range(0, roomPool.Count);
        if (tempIndex == roomPool.Count)
        {
            tempIndex--;
        }
        //Make one int minus if it goes to roomPool.Count since Random.Range starts from 0 not 1
        //Random Range uses floats that round down to int, so you can't just do roomPool.Count - 1 because then 2 will almost never appear

        GameObject tempRoom = Instantiate(roomPool[tempIndex], transform.position, Quaternion.identity);
    }
}
