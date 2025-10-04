using UnityEngine;

public class MusicController : MonoBehaviour
{
    [Header("要检测的物体")]
    public GameObject objectA;
    public GameObject objectB;

    [Header("背景音乐")]
    public AudioSource musicSource; // 挂载背景音乐 AudioSource

    private void Start()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.Play(); // 开始时播放
        }
    }

    private void Update()
    {
        if (musicSource == null) return;

        bool aActive = (objectA != null && objectA.activeInHierarchy && objectA.activeSelf);
        bool bActive = (objectB != null && objectB.activeInHierarchy && objectB.activeSelf);

        if (aActive || bActive)
        {
            // 如果A或B启用 → 暂停音乐
            if (musicSource.isPlaying)
            {
                musicSource.Pause();
            }
        }
        else
        {
            // 如果A和B都禁用 → 继续播放
            if (!musicSource.isPlaying)
            {
                musicSource.UnPause(); // 用 UnPause 保持进度
            }
        }
    }
}
