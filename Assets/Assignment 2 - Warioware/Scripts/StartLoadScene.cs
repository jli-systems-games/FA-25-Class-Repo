using UnityEngine;

public class StartLoadScene : MonoBehaviour
{
    public GameManager GameManager;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            GameManager.LoadRandomGame();
        }
    }   
}
