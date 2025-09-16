using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHit : MonoBehaviour
{
    public string sceneOnFail = "Scene1";
    public float delay = 2f;
    public string killZoneTag = "KillZone";

    Animator anim;
    float enableTime;
    bool played;

    void Awake()
    {
        anim = GetComponent<Animator>();
        if (anim) Debug.Log("PlayerHit Animator initialized"); // 调试
    }

    void OnEnable()
    {
        enableTime = Time.time;
        played = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (played) return;

        // 开场0.2s内忽略触发（防重叠/初始化抖动）
        if (Time.time - enableTime < 0.2f) return;

        if (other.CompareTag(killZoneTag))
        {
            played = true;
            if (anim)
            {
                Debug.Log("[PlayerHit] Triggering Hit"); // 调试
                anim.SetTrigger("Hit");
            }
            Invoke(nameof(Back), delay);
        }
    }

    void Back()
    {
        SceneManager.LoadScene(sceneOnFail);
    }
}