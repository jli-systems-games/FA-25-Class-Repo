using UnityEngine;

public class SimpleMusicFadeOut : MonoBehaviour
{
    public AudioSource bgm;      
    public float fadeTime = 1f; 

    public void FadeOut()
    {
        if (bgm != null)
            StartCoroutine(FadeOutCoroutine());
    }

    private System.Collections.IEnumerator FadeOutCoroutine()
    {
        float startVolume = bgm.volume;

        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            bgm.volume = Mathf.Lerp(startVolume, 0f, t / fadeTime);
            yield return null;
        }

        bgm.volume = 0f;
        bgm.Stop();
    }
}