using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class WorldController : MonoBehaviour
{
    public List<GameObject> rooms =
        new List<GameObject>();
    public GameObject room;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // public int roomMax;
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

        for (int i = 0; i < roomWidth; i++)
        {
            for (int j = 0; j < roomHeight; j++)
            {
                Vector3 tempPosition = room.transform.position;
                tempPosition.y = j;
                tempPosition.x = i;



                int tempIndex = Random.Range(0, roomPool.Count);
                if (tempIndex == roomPool.Count)
                    tempIndex--;

                while (previousRoom == tempIndex)
                {
                    tempIndex = Random.Range(0, roomPool.Count);
                    if (tempIndex == roomPool.Count)
                        tempIndex--;

                    if (roomPool.Count < 0)
                        break;
                }



                GameObject tempRoom =
                    Instantiate(roomPool[tempIndex],
                                tempPosition,
                                Quaternion.identity);


                rooms.Add(tempRoom);
                previousRoom = tempIndex;
            }
            
        }
    }
}
