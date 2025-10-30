using UnityEngine;

public class CatMeow : MonoBehaviour
{
    public AudioClip[] soundClips;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            PlayRandomSound();
        }
    }

    private void PlayRandomSound()
    {
        if (soundClips == null || soundClips.Length == 0) return;

        int index = Random.Range(0, soundClips.Length);
        AudioClip clip = soundClips[index];

        AudioSource.PlayClipAtPoint(clip, transform.position);
    }
}