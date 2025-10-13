using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerLevel;
    void Start()
    {
        playerLevel = Data.data_playerCharacter
            .characterLevel;
        Debug.Log(playerLevel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
