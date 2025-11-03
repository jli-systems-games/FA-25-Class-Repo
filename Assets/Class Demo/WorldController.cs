using UnityEngine;
using System.Collections.Generic;

public class WorldController : MonoBehaviour
{
    public List<GameObject> rooms = 
        new List<GameObject>();

    public GameObject room;
    //public int roomMax;
    public int roomHeight, roomWidth;

    public List<GameObject> roomPool =
        new List<GameObject>();

    private void Start()
    {
        GenerateWorld();
    }

    private void GenerateWorld()
    {
        int previousRoom = -1;

        for (int i = 0; i < roomWidth; i++)
        {
            for (int j = 0; j < roomHeight; j++)
            {
                Vector3 tempPosition = room.transform.position;
                tempPosition.x = i;
                tempPosition.y = j;

                int tempIndex = Random.Range(0, roomPool.Count);
                if (tempIndex == roomPool.Count)
                {
                    tempIndex--;
                }
                //Make one int minus if it goes to roomPool.Count since Random.Range starts from 0 not 1
                //Random Range uses floats that round down to int, so you can't just do roomPool.Count - 1 because then 2 will almost never appear
                while (previousRoom == tempIndex)
                {
                    tempIndex = Random.Range(0, roomPool.Count);
                    if (tempIndex == roomPool.Count)
                    {
                        tempIndex--;
                    }

                    if (roomPool.Count < 0)
                    {
                        break;
                    }
                }

                GameObject tempRoom = Instantiate(roomPool[tempIndex], tempPosition, Quaternion.identity);

                rooms.Add(tempRoom);

                previousRoom = tempIndex;
            }
        }
    }
}
