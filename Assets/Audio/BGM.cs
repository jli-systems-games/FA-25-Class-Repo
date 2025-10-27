using UnityEngine;

public class BGM : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject); // 切换场景不销毁
    }

    void Start()
    {
        var audio = GetComponent<AudioSource>();
        if (audio != null && !audio.isPlaying)
        {
            audio.loop = true;
            audio.Play();
        }
    }
}