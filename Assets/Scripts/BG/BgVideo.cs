using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.IO;

public class BgVideo : MonoBehaviour
{
    public static BgVideo I;

    public string fileName = "bg.mp4";

    VideoPlayer vp;

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        vp = gameObject.AddComponent<VideoPlayer>();
        vp.source = VideoSource.Url;
        vp.url = Path.Combine(Application.streamingAssetsPath, fileName);

        vp.renderMode = VideoRenderMode.CameraFarPlane;
        vp.targetCamera = Camera.main;

        vp.isLooping = false;
        vp.playOnAwake = false;
        vp.audioOutputMode = VideoAudioOutputMode.None;

        PrepareAndPlay();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    async void PrepareAndPlay()
    {
        vp.Prepare();
        while (!vp.isPrepared) await System.Threading.Tasks.Task.Yield();
        vp.Play();
    }

    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        var cam = Camera.main;
        if (cam != null) vp.targetCamera = cam;
        if (!vp.isPlaying) vp.Play();
    }

    void OnDestroy()
    {
        if (I == this) SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}