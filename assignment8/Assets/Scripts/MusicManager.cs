using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour
{
    public static MusicManager I; 

    [Header("BGM Clips")]
    public AudioClip titleBgm;
    public AudioClip gameBgm;
    public float fadeTime = 1.0f;

    [Header("SFX Clips")]
    public AudioClip sfxClick;
    public AudioClip sfxGum;
    public AudioClip sfxBreatheOK;
    public AudioClip sfxBreathePartial;
    public AudioClip sfxSpike;
    public AudioClip sfxRebound;
    public AudioClip sfxWin;
    public AudioClip sfxFail;

    [Header("Volumes (0..1)")]
    [Range(0, 1)] public float musicVol = 0.6f;
    [Range(0, 1)] public float sfxVol = 0.8f;

    private const string KEY_MUSIC_VOL = "Vol_Music";
    private const string KEY_SFX_VOL = "Vol_SFX";

    private AudioSource bgmA, bgmB;
    private bool bgmUsingA = true;

    private List<AudioSource> sfxPool = new List<AudioSource>();
    public int sfxPoolSize = 6;

    void Awake()
    {
        if (I == null) I = this; else { Destroy(gameObject); return; }

        musicVol = PlayerPrefs.GetFloat(KEY_MUSIC_VOL, musicVol);
        sfxVol = PlayerPrefs.GetFloat(KEY_SFX_VOL, sfxVol);

        bgmA = gameObject.AddComponent<AudioSource>();
        bgmB = gameObject.AddComponent<AudioSource>();
        SetupBgmSource(bgmA);
        SetupBgmSource(bgmB);

        for (int i = 0; i < sfxPoolSize; i++)
        {
            var a = gameObject.AddComponent<AudioSource>();
            a.playOnAwake = false;
            a.loop = false;
            a.volume = sfxVol;
            a.spatialBlend = 0f;
            sfxPool.Add(a);
        }
    }

    void SetupBgmSource(AudioSource a)
    {
        a.playOnAwake = false;
        a.loop = true;
        a.volume = 0f;
        a.spatialBlend = 0f;
    }

    public void PlayTitleBgm() => CrossfadeTo(titleBgm);
    public void PlayGameBgm() => CrossfadeTo(gameBgm);

    public void PlayClick() => PlaySfx(sfxClick);
    public void PlayGum() => PlaySfx(sfxGum);
    public void PlayBreatheSuccess() => PlaySfx(sfxBreatheOK);
    public void PlayBreathePartial() => PlaySfx(sfxBreathePartial);
    public void PlaySpike() => PlaySfx(sfxSpike);
    public void PlayRebound() => PlaySfx(sfxRebound);
    public void PlayWin() => PlaySfx(sfxWin);
    public void PlayFail() => PlaySfx(sfxFail);

    public void SetMusicVol(float v)
    {
        musicVol = Mathf.Clamp01(v);
        PlayerPrefs.SetFloat(KEY_MUSIC_VOL, musicVol);
        PlayerPrefs.Save();
        (bgmUsingA ? bgmA : bgmB).volume = Mathf.Min((bgmUsingA ? bgmA : bgmB).volume, musicVol);
    }

    public void SetSfxVol(float v)
    {
        sfxVol = Mathf.Clamp01(v);
        PlayerPrefs.SetFloat(KEY_SFX_VOL, sfxVol);
        PlayerPrefs.Save();
        foreach (var a in sfxPool) a.volume = sfxVol;
    }

    void CrossfadeTo(AudioClip clip)
    {
        if (clip == null) return;

        AudioSource from = bgmUsingA ? bgmA : bgmB;
        AudioSource to = bgmUsingA ? bgmB : bgmA;
        bgmUsingA = !bgmUsingA;

        to.clip = clip;
        to.volume = 0f;
        to.Play();
        StopAllCoroutines();
        StartCoroutine(CoCrossfade(from, to, fadeTime));
    }

    IEnumerator CoCrossfade(AudioSource from, AudioSource to, float t)
    {
        float el = 0f;
        float startFrom = from.isPlaying ? from.volume : 0f;
        while (el < t)
        {
            el += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(el / t);
            if (from && from.isPlaying) from.volume = Mathf.Lerp(startFrom, 0f, k);
            if (to) to.volume = Mathf.Lerp(0f, musicVol, k);
            yield return null;
        }
        if (from) { from.Stop(); from.volume = 0f; }
        if (to) { to.volume = musicVol; }
    }

    void PlaySfx(AudioClip clip)
    {
        if (clip == null) return;
        AudioSource chan = null;
        foreach (var a in sfxPool)
        {
            if (!a.isPlaying) { chan = a; break; }
        }
        if (chan == null) chan = sfxPool[0];
        chan.volume = sfxVol;
        chan.PlayOneShot(clip);
    }
}
