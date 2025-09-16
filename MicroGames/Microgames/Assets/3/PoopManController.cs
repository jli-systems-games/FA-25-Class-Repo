using UnityEngine;

public class PoopManController : MonoBehaviour
{
    [Header("Refs")]
    public GameObject standImage;      // 站立图
    public GameObject crouchImage;     // 蹲下图

    [Header("SFX")]
    public AudioSource poopSource;     // 独立的 AudioSource（Loop=✓，Spatial=2D）
    public AudioClip poopLoopClip;     // 拉屎循环音效

    public bool IsCrouching { get; private set; }

    void Start()
    {
        SetPose(false); // 默认站立
        if (poopSource)
        {
            poopSource.loop = true;
            if (poopLoopClip) poopSource.clip = poopLoopClip;
        }
    }

    void Update()
    {
        bool wantCrouch = Input.GetKey(KeyCode.Space);
        if (wantCrouch != IsCrouching)
            SetPose(wantCrouch);
    }

    void SetPose(bool crouch)
    {
        IsCrouching = crouch;
        if (standImage) standImage.SetActive(!crouch);
        if (crouchImage) crouchImage.SetActive(crouch);

        // 声音
        if (poopSource)
        {
            if (crouch)
            {
                if (poopLoopClip && !poopSource.isPlaying) poopSource.Play();
            }
            else
            {
                if (poopSource.isPlaying) poopSource.Stop();
            }
        }
    }
}
