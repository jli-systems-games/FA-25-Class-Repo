using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameResetter : MonoBehaviour
{
    public void DestroyAllDontDestroyOnLoadObjects()
    {
        // create a temporary thing to get ddol scene info
        GameObject temp = new GameObject();
        DontDestroyOnLoad(temp);
        Scene dontDestroyScene = temp.scene;
        Destroy(temp);

        // kill them all
        if (dontDestroyScene.IsValid())
        {
            foreach (GameObject obj in dontDestroyScene.GetRootGameObjects())
            {
                if (obj != this.gameObject) 
                {
                    Destroy(obj);
                }
            }
        }
    }


}