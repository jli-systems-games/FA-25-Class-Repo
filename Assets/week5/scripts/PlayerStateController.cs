using UnityEngine;

public enum PlayerMode { Hug, Innocent }

public class PlayerStateController : MonoBehaviour
{
    public KeyCode toggleKey = KeyCode.Space;
    public float toggleCooldown = 0f;

    public PlayerMode current = PlayerMode.Innocent;

    // 각각의 장면을 담은 GameObject (각자 SpriteRenderer 갖고 있음)
    public GameObject hugObject;       // 안는 장면 오브젝트
    public GameObject innocentObject;  // 모른척 장면 오브젝트

    public AudienceSwitcher audienceSwitcher; // 관중 전환도 동시에 할 경우
    public SFXVFXManager sfx;

    float cdTimer;

    void Start()
    {
        Apply(); // 시작 상태 반영
    }

    void Update()
    {
        if (cdTimer > 0f) cdTimer -= Time.deltaTime;

        if (Input.GetKeyDown(toggleKey) && cdTimer <= 0f)
        {
            current = (current == PlayerMode.Hug) ? PlayerMode.Innocent : PlayerMode.Hug;
            Apply();

            audienceSwitcher?.ToggleAudience();
            sfx?.OnPlayerModeChanged(current);

            cdTimer = toggleCooldown;
        }
    }

    void Apply()
    {
        if (hugObject) hugObject.SetActive(current == PlayerMode.Hug);
        if (innocentObject) innocentObject.SetActive(current == PlayerMode.Innocent);
    }

    public bool IsHug() => current == PlayerMode.Hug;
    public bool IsInnocent() => current == PlayerMode.Innocent;
}
