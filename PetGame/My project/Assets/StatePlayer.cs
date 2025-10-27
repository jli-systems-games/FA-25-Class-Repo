using UnityEngine;
using System.Collections;

public class StatePlayer : MonoBehaviour
{
    public Animator anim;
    public string locomotionState = "Base Layer/Locomotion";
    public bool lockDuringAction = true;
    public float defaultFade = 0.06f;

    bool inAction;
    Coroutine backCo;

    void Reset() { anim = GetComponent<Animator>(); }

    public void PlayState(string fullStateName, float fade = -1f, float holdSeconds = -1f)
    {
        if (anim == null) return;
        if (fade <= 0f) fade = defaultFade;
        if (backCo != null) StopCoroutine(backCo);
        inAction = lockDuringAction;
        anim.CrossFadeInFixedTime(fullStateName, fade, 0, 0f);
        if (holdSeconds > 0f) backCo = StartCoroutine(BackAfter(holdSeconds, fade));
    }

    public void PlayStateByHash(int stateHash, float fade = -1f, float holdSeconds = -1f)
    {
        if (anim == null) return;
        if (fade <= 0f) fade = defaultFade;
        if (backCo != null) StopCoroutine(backCo);
        inAction = lockDuringAction;
        anim.CrossFadeInFixedTime(stateHash, fade, 0, 0f);
        if (holdSeconds > 0f) backCo = StartCoroutine(BackAfter(holdSeconds, fade));
    }

    public void BackToLocomotion(float fade = -1f)
    {
        if (anim == null) return;
        if (fade <= 0f) fade = defaultFade;
        anim.CrossFadeInFixedTime(locomotionState, fade, 0, 0f);
        inAction = false;
    }

    IEnumerator BackAfter(float t, float fade)
    {
        yield return new WaitForSeconds(t);
        BackToLocomotion(fade);
    }

    public bool IsBusy() { return inAction; }
}
