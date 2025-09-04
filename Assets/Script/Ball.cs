using UnityEngine;
// From Youtube, Bilibili, and Comments
public class Ball : MonoBehaviour
{
    public AudioClip player1HitSound;//玩家1音效
    public AudioClip player2HitSound;//玩家2音效
    private AudioSource audioSource;    
    void Awake()
    {
        //audioSource检查
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        //音效设置
        audioSource.enabled = true;
        audioSource.playOnAwake = false;    //不要立即播放音频
        audioSource.volume = 1f;
    }
    
    void OnCollisionEnter2D(Collision2D col)
    {
        //检测碰撞的物体
        string objName = col.gameObject.name.ToLower();
        //判断触发音效
        //玩家1
        if (objName.Contains("player1") || objName.Contains("p1") || objName.Contains("left"))
        {
            if (player1HitSound != null)
            {
                audioSource.PlayOneShot(player1HitSound);
            }
        }
        //玩家2
        else if (objName.Contains("player2") || objName.Contains("p2") || objName.Contains("right"))
        {
            if (player2HitSound != null)
            {
                audioSource.PlayOneShot(player2HitSound);
            }
        }
    }
}