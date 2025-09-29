using UnityEngine;

public class SFXVFXManager : MonoBehaviour
{
    public AudioSource oneShot;
    public AudioClip toggleSfx;
    public AudioClip warnBeep;
    public AudioClip shutter;
    public AudioClip boo;

    public void OnPlayerModeChanged(PlayerMode mode)
    {
        if (toggleSfx && oneShot) oneShot.PlayOneShot(toggleSfx);
    }

    public void OnKissCamArriveCheckpoint()
    {
        if (warnBeep && oneShot) oneShot.PlayOneShot(warnBeep);
    }

    public void OnKissCamLeaveCheckpoint() { }

    public void OnCaught()
    {
        if (shutter && oneShot) oneShot.PlayOneShot(shutter);
        if (boo && oneShot) oneShot.PlayOneShot(boo, 0.7f);
    }
}
