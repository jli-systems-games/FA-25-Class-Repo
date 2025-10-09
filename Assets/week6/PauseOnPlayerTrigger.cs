// PauseOnPlayerTrigger.cs
using UnityEngine;
using UnityEngine.AI;

public class PauseOnPlayerTrigger : MonoBehaviour
{
    [Header("Effect")]
    public ParticleSystem effect;          // 재생할 파티클
    public AudioSource sfx;                // (선택) 효과음

    [Header("Optional: 끌 스크립트들")]
    public MonoBehaviour[] scriptsToDisable; // 예: PlayerCapsuleController, 스포너 등
    public NavMeshAgent[] agentsToStop;      // (선택) 움직이던 에이전트들

    [Header("UI/커서")]
    public bool showCursor = true;

    bool triggered;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;  // Player 태그만 반응

        triggered = true;

        // 플레이어/시스템 스크립트 끄기(업데이트 멈추게)
        foreach (var mb in scriptsToDisable)
            if (mb) mb.enabled = false;

        foreach (var ag in agentsToStop)
        {
            if (ag && ag.enabled)
            {
                ag.isStopped = true;
                ag.enabled = false;
            }
        }

        // 게임 일시정지
        Time.timeScale = 0f;            // 물리/애니/네비 갱신 정지
        AudioListener.pause = true;     // (선택) 전체 오디오 일시정지

        // 파티클은 '언스케일드 타임'으로 재생되게 설정
        if (effect)
        {
            var main = effect.main;
            main.useUnscaledTime = true; // timeScale=0이어도 재생됨
            effect.Play();
        }

        if (sfx)
        {
            sfx.ignoreListenerPause = true; // 전역 오디오 정지와 무관하게 재생
            sfx.Play();
        }

        if (showCursor)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        if (TimeOut.Instance) TimeOut.Instance.RegisterSuccess();
    }

    // (선택) 재시작용
    public void Resume()
    {
        AudioListener.pause = false;
        Time.timeScale = 1f;
    }
}
