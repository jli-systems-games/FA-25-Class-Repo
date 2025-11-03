using UnityEngine;
using System.Collections.Generic;

public class WorldController : MonoBehaviour
{
    public List<GameObject> rooms =
        new List<GameObject>();

    public GameObject room;

    public int roomMax;
    public int roomHeight, roomWidth;

    public List<GameObject> roomPool =
        new List<GameObject>();


    void Start()
    {
        GenerateWorld();

    }

    public void GenerateWorld()
    {
        int previousRoom = -1;

        for(int i = 0; i < roomWidth; i++)
        {
            for (int j = 0; j < roomHeight; i++)
            {
                Vector3 tempPosition = room.transform.position;
                tempPosition.x = i;
                tempPosition.y = j;


                int tempIndex = Random.Range(0, roomPool.Count);
                if (tempIndex == roomPool.Count)
                    tempIndex--;
                while (previousRoom == tempIndex)
                {
                    tempIndex = Random.Range(0, roomPool.Count);
                    if (tempIndex == roomPool.Count)
                        tempIndex--;

                }


                GameObject tempRoom =
                    Instantiate(roomPool[tempIndex], tempPosition, Quaternion.identity);

                //GameObject tempRoom =
                //    Instantiate(room, room.transform.position, Quaternion.identity);

                rooms.Add(tempRoom);

                previousRoom = tempIndex;
            }
        }

}
}
