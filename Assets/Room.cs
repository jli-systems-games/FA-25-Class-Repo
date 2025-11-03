using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<GameObject> roomPool =
      new List<GameObject>();

    void Start()
    {

        int tempIndex = Random.Range(0, roomPool.Count);
        if (tempIndex == roomPool.Count)
            tempIndex--;


        GameObject tempRoom =
            Instantiate(roomPool[tempIndex], tempPosition, Quaternion.identity);
    }

}
