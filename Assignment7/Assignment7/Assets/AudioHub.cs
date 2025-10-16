using UnityEngine;

public class AudioHub : MonoBehaviour
{
    static AudioHub _inst;
    AudioSource _src;

    public static void Ensure()
    {
        if (_inst) return;
        var go = new GameObject("AudioHub");
        DontDestroyOnLoad(go);
        _inst = go.AddComponent<AudioHub>();
        _inst._src = go.AddComponent<AudioSource>();
        _inst._src.spatialBlend = 0f;
        _inst._src.playOnAwake = false;
        _inst._src.loop = false;
        _inst._src.volume = 1f;
    }

    public static void Play2D(AudioClip clip, float volume = 1f)
    {
        if (!clip) return;
        Ensure();
        _inst._src.PlayOneShot(clip, Mathf.Clamp01(volume));
    }
}
