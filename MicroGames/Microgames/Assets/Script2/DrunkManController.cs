using UnityEngine;

public class DrunkManController : MonoBehaviour
{
    [Header("Hop (Space)")]
    public float hopHeight = 1.2f;      // 抬一下高度
    public float hopDuration = 0.25f;   // 抬一下时长
    public AnimationCurve hopCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("FX")]
    public Animator animator;     // 控制器里要有两个 Trigger: Hit, Win
    public AudioSource audioSource;
    public AudioClip hitClip;
    public AudioClip winClip;

    public bool IsDodging { get; private set; }

    Vector3 groundPos;

    void Start()
    {
        groundPos = transform.position;
        if (animator) Debug.Log("Animator initialized"); // 调试
    }

    void Update()
    {
        if (!IsDodging && Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(CoHop());
    }

    System.Collections.IEnumerator CoHop()
    {
        IsDodging = true;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, hopDuration);
            float k = hopCurve.Evaluate(Mathf.Clamp01(t));
            float y = Mathf.Sin(k * Mathf.PI); // 0→1→0
            transform.position = new Vector3(groundPos.x, groundPos.y + y * hopHeight, groundPos.z);
            yield return null;
        }
        transform.position = groundPos;
        IsDodging = false;
    }

    // —— 结果表现 —— //
    public void PlayHitFly()
    {
        if (animator)
        {
            Debug.Log("Triggering Hit animation"); // 调试
            animator.SetTrigger("Hit");
        }
        if (audioSource && hitClip) audioSource.PlayOneShot(hitClip);
    }

    public void PlayWin()
    {
        if (animator)
        {
            Debug.Log("Triggering Win animation"); // 调试
            animator.SetTrigger("Win");
        }
        if (audioSource && winClip) audioSource.PlayOneShot(winClip);
    }
}