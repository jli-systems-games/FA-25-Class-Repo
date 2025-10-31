using UnityEngine;

public class DontDestroyBGM : MonoBehaviour
{
    private static DontDestroyBGM instance; 

    void Awake()
    {
 
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject); 
    }
}
