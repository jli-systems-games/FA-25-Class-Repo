using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class VideoManager : MonoBehaviour
{
    public GameObject[] videoPrefabs;
    public Transform canvas;

    public AudioSource videoAudioManager;

    public float spawnMinTime = 5f;
    public float spawnMaxTime = 10f;

    private float spawnTimer;
    private GameObject currentVideo;

    private void Start()
    {
        //stopButton1.onClick.AddListener(StopAudio);
        //stopButton2.onClick.AddListener(StopAudio);
        //stopButton3.onClick.AddListener(StopAudio);
        //stopButton4.onClick.AddListener(StopAudio);

        ResetSpawnTimer();
    }

    private void Update()
    {
        Debug.Log(spawnTimer);

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            Debug.Log("spawning vid");
            SpawnVideo();
            ResetSpawnTimer();
        }
    }

    private void SpawnVideo()
    {
        //videoAudioManager.clip = pianoSound;
        //videoAudioManager.Play();

        if (videoPrefabs.Length == 0) return;

        int index = Random.Range(0, videoPrefabs.Length);

        float x = Random.Range(-5f,5.6f);
        float y = Random.Range(-3f, 2.5f);
        Vector3 spawnPos = new Vector3(x, y, 0f);

        currentVideo = Instantiate(videoPrefabs[index], spawnPos, Quaternion.identity, transform);
        currentVideo.transform.SetParent(canvas, false);

        var vp = currentVideo.GetComponent<UnityEngine.Video.VideoPlayer>();
        if (vp != null) vp.Play();
    }

    private void ResetSpawnTimer()
    {
        spawnTimer = Random.Range(spawnMinTime, spawnMaxTime);
    }
    private void StopAudio()
    {
        if (videoAudioManager.isPlaying)
            videoAudioManager.Stop();
    }
}
