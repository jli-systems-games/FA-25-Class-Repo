using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerLevel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerLevel = Data1.data_playerCharacter.characterLevel;
        Debug.Log(playerLevel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
