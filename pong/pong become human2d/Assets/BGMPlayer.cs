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

        //gpt��ʦdebug��ʱ��д�ķ�ֹ�ظ����ŵ��ǽ�����ֻ��һ���������Ҿ���û��Ҫ������������������

        DontDestroyOnLoad(gameObject);
    }
}
