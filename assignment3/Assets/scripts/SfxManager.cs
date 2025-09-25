using UnityEngine;

public class SfxManager : MonoBehaviour
{
    public AudioClip[] taps;
    public AudioClip[] drags;
    public AudioClip[] pops;

    public void PlayOneShot(AudioSource src, AudioClip clip, float vol = 1f)
    {
        if (!src || !clip) return;
        src.PlayOneShot(clip, vol);
    }

    public AudioClip RandomOf(AudioClip[] arr)
    {
        if (arr == null || arr.Length == 0) return null;
        return arr[Random.Range(0, arr.Length)];
    }
}
