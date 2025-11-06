using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public GameObject trackPrefab;
    public AudioClip[] trackClips;
    public AudioClip fullMixClip;

    public AudioSource mixTrack;
    private List<AudioSource> allTracks = new List<AudioSource>();
    private List<int> playedTracks = new List<int>();
    private List<int> currentSet = new List<int>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        foreach (var clip in trackClips)
        {
            GameObject newTrack = Instantiate(trackPrefab, transform);
            newTrack.name = "Track_" + clip.name;
            AudioSource src = newTrack.GetComponent<AudioSource>();
            src.clip = clip;
            src.playOnAwake = false;
            allTracks.Add(src);
        }

        PlayRandomSet();
    }

    public void PlayRandomSet()
    {
        foreach (var track in allTracks) track.Stop();
        currentSet.Clear();

        int count = Random.Range(3, 5);
        List<int> available = new List<int>();
        for (int i = 0; i < allTracks.Count; i++) available.Add(i);

        for (int i = 0; i < count; i++)
        {
            int idx = available[Random.Range(0, available.Count)];
            available.Remove(idx);
            allTracks[idx].Play();
            currentSet.Add(idx);

            if (!playedTracks.Contains(idx))
                playedTracks.Add(idx);
        }

        CheckUnlock();
    }

    void CheckUnlock()
    {
        if (playedTracks.Count >= allTracks.Count && !GameManager.Instance.isClipFullfilled)
        {
            Debug.Log("All tracks have been heard! Unlock full track.");
            GameManager.Instance.isClipFullfilled = true;
            SceneManager.LoadScene("TransitionScene");
        }
    }

    public void RefreshForNextScene()
    {
        PlayRandomSet();
    }
}
