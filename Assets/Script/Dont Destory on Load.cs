using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private void Awake()
    {
        // 保证这个物体切换场景不销毁
        DontDestroyOnLoad(gameObject);

        // 避免重复：如果场景里已经有一个同名音乐对象，就销毁自己
        if (FindObjectsOfType<MusicPlayer>().Length > 1)
        {
            Destroy(gameObject);
        }
    }
}