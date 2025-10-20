using UnityEngine;
using UnityEngine.Video;

public class TransitionScene : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        // transition for end of video
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        GameManager.Instance.LoadNextGame();
    }
}
