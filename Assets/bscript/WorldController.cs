using UnityEngine;
using System.Collections.Generic;

public class WorldController : MonoBehaviour
{
    public List<GameObject> rooms = new List<GameObject>();
    public List<GameObject> roomPool = new List<GameObject>();

    public int roomHeight;
    public int roomWidth;
    public float spacing;

    void Start()
    {
        GenerateWorld();
    }

    public void GenerateWorld()
    {
        int previousRoom = -1;

  
        float offsetX = (roomWidth - 1) * spacing / 2f;
        float offsetZ = (roomHeight - 1) * spacing / 2f;

        for (int i = 0; i < roomWidth; i++)
        {
            for (int j = 0; j < roomHeight; j++)
            {
                
                Vector3 tempPosition = new Vector3(
                    i * spacing - offsetX,
                    0f,
                    j * spacing - offsetZ
                );

                int tempIndex = Random.Range(0, roomPool.Count);
                while (previousRoom == tempIndex && roomPool.Count > 1)
                {
                    tempIndex = Random.Range(0, roomPool.Count);
                }

                GameObject tempRoom = Instantiate(
                    roomPool[tempIndex],
                    tempPosition,
                    Quaternion.identity
                );

                rooms.Add(tempRoom);
                previousRoom = tempIndex;
            }
        }

       
    }
}
