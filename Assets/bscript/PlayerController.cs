using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerlevel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerlevel = Data.date_playerCharacter.characterlevel;
        Debug.Log("sss");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
