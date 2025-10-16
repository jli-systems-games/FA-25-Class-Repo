using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;
using System.IO;

public class SeamlessLoopBgLocal : MonoBehaviour
{
    public string fileName = "bg.mp4";

    public RawImage imgA;
    public RawImage imgB;

    public RenderTexture rtA;
    public RenderTexture rtB;

    public float overlap = 0.25f;
    public float crossFade = 0.20f;

    VideoPlayer vpA, vpB;
    CanvasGroup cgA, cgB;

    void Awake()
    {
        cgA = imgA.GetComponent<CanvasGroup>() ?? imgA.gameObject.AddComponent<CanvasGroup>();
        cgB = imgB.GetComponent<CanvasGroup>() ?? imgB.gameObject.AddComponent<CanvasGroup>();
        cgA.alpha = 1f; cgB.alpha = 0f;

        vpA = CreatePlayer(rtA);
        vpB = CreatePlayer(rtB);
    }

    VideoPlayer CreatePlayer(RenderTexture rt)
    {
        var vp = gameObject.AddComponent<VideoPlayer>();
        vp.source = VideoSource.Url;
        vp.url = Path.Combine(Application.streamingAssetsPath, fileName);
        vp.renderMode = VideoRenderMode.RenderTexture;
        vp.targetTexture = rt;
        vp.audioOutputMode = VideoAudioOutputMode.None;
        vp.playOnAwake = false;
        vp.isLooping = false;
        vp.skipOnDrop = true;
        return vp;
    }

    IEnumerator Start()
    {
        yield return Prepare(vpA);
        yield return Prepare(vpB);

        vpA.frame = 0;
        vpA.Play();
        yield return DriveLoop();
    }

    IEnumerator Prepare(VideoPlayer vp)
    {
        vp.Prepare();
        while (!vp.isPrepared) yield return null;
    }

    IEnumerator DriveLoop()
    {
        while (true)
        {
            double dur = Duration(vpA);
            while (vpA.time < dur - overlap)
                yield return null;

            vpB.frame = 0;
            vpB.Play();
            yield return Cross(cgA, cgB, crossFade);

            vpA.Stop();
            yield return Prepare(vpA);

            (vpA, vpB) = (vpB, vpA);
            (cgA, cgB) = (cgB, cgA);
            (imgA, imgB) = (imgB, imgA);
        }
    }

    double Duration(VideoPlayer vp)
    {
        if (vp.frameRate > 0 && vp.frameCount > 0)
            return (double)vp.frameCount / vp.frameRate;
        return 5.0;
    }

    IEnumerator Cross(CanvasGroup from, CanvasGroup to, float t)
    {
        float e = 0f;
        while (e < t)
        {
            e += Time.deltaTime;
            float k = e / t;
            from.alpha = 1f - k;
            to.alpha = k;
            yield return null;
        }
        from.alpha = 0f; to.alpha = 1f;
    }
}