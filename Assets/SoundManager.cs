using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public AudioSource[] tracks;

    public IEnumerator FadeTrackTo1(int index, float duration)
    {
        if (index < 0 || index >= tracks.Length) yield break;

        AudioSource a = tracks[index];
        float startVol = a.volume;
        float time = 0f;

        if (!a.isPlaying) a.Play();

        while (time < duration)
        {
            time += Time.deltaTime;
            a.volume = Mathf.Lerp(startVol, 1f, time / duration);
            yield return null;
        }

        a.volume = 1f;
    }
}
