using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    private static BGMPlayer instance;

    void Awake()
    {
   
       // if (instance != null && instance != this)
        //{
          //  Destroy(gameObject);
            //return;
        //}
        //instance = this;

        //gpt老师debug的时候写的防止重复播放但是介于我只有一个播放器我觉得没必要。。。。。。。。。

        DontDestroyOnLoad(gameObject);
    }
}
