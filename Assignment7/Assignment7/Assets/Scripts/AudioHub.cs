using UnityEngine;

public class AudioHub : MonoBehaviour
{
    static AudioHub inst;
    AudioSource src;

    public static void Ensure()
    {
        if (inst) return;
        var go = new GameObject("AudioHub");
        Object.DontDestroyOnLoad(go);
        inst = go.AddComponent<AudioHub>();
        inst.src = go.AddComponent<AudioSource>();
        inst.src.spatialBlend = 0f;
        inst.src.playOnAwake = false;
        inst.src.loop = false;
        inst.src.volume = 1f;
        var listener = Object.FindFirstObjectByType<AudioListener>();
        if (!listener)
        {
            var cam = Camera.main ? Camera.main.gameObject : new GameObject("Main Camera", typeof(Camera));
            if (!cam.GetComponent<AudioListener>()) cam.AddComponent<AudioListener>();
        }
    }

    public static void Play2D(AudioClip clip, float volume = 1f)
    {
        if (!clip) return;
        Ensure();
        inst.src.PlayOneShot(clip, Mathf.Clamp01(volume));
    }
}
