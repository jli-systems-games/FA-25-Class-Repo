using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<GameObject> roomPool =
    new List<GameObject>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int tempIndex = Random.Range(0, roomPool.Count);
        if (tempIndex == roomPool.Count)
            tempIndex--;

        GameObject tempRoom =
            Instantiate(roomPool[tempIndex],
            transform.position,
            Quaternion.identity
            );
    }

}