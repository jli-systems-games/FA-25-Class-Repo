using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHitDirect : MonoBehaviour
{
    [Header("Setup")]
    public Animator anim;                 // 拖 Player 的 Animator
    public string hitStateName = "Hitfly"; // 状态名要和 Animator 里一致
    public string sceneOnFail = "Scene1"; // 播完跳回的场景
    public string killZoneTag = "KillZone";
    public float fallbackLen = 2f;        // 读不到长度时使用

    float born;
    bool played;

    void Awake()
    {
        if (!anim) anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        born = Time.time;
        played = false;
        // 确保开局停在 Idle
        if (anim) { anim.Rebind(); anim.Update(0f); }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (played) return;
        if (Time.time - born < 0.2f) return; // 开局0.2s忽略，防止初始重叠

        if (other.CompareTag(killZoneTag))
        {
            played = true;

            // 强制从头播放指定状态（不走 Trigger/过渡）
            anim.ResetTrigger("Hit");          // 就算你还保留了Trigger也不会被卡住
            anim.Update(0f);
            anim.Play(hitStateName, 0, 0f);    // 立刻切入命名状态

            StartCoroutine(BackAfterState());
        }
    }

    IEnumerator BackAfterState()
    {
        // 等一帧，让 Animator 真的切进去
        yield return null;

        float len = fallbackLen;
        var info = anim.GetCurrentAnimatorStateInfo(0);
        if (info.IsName(hitStateName))
            len = info.length;

        yield return new WaitForSeconds(len);
        SceneManager.LoadScene(sceneOnFail);
    }
}
