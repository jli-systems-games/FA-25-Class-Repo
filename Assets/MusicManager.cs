using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioSource mainTrack;
    public AudioSource lipstickTrack;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (mainTrack != null && !mainTrack.isPlaying)
        {
            mainTrack.Play();
        }

        if (lipstickTrack != null)
        {
            lipstickTrack.Stop();
        }
    }

    public void UseLipstickTrack(bool use)
    {
        if (use)
        {
            if (mainTrack != null && mainTrack.isPlaying)
                mainTrack.Pause();

            if (lipstickTrack != null && !lipstickTrack.isPlaying)
                lipstickTrack.volume = 0.9f;
                lipstickTrack.Play();
        }
        else
        {
            if (lipstickTrack != null && lipstickTrack.isPlaying)
                lipstickTrack.Stop();

            if (mainTrack != null && !mainTrack.isPlaying)
                mainTrack.UnPause();
        }
    }
}
