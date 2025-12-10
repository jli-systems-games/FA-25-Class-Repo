using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip normalTheme;
    public AudioClip finalTheme;

    [Header("Fade Settings")]
    public float fadeDuration = 2f;

    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
        }
    }

    private void Start()
    {
        if (normalTheme != null)
        {
            PlayNormalTheme(true);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "title")
        {
            PlayNormalTheme(true);
        }
    }


    public void PlayNormalTheme(bool instant = false)
    {
        if (normalTheme == null) return;
        SwitchTrack(normalTheme, instant);
    }

    public void PlayFinalTheme(bool instant = false)
    {
        if (finalTheme == null) return;
        SwitchTrack(finalTheme, instant);
    }

    void SwitchTrack(AudioClip targetClip, bool instant)
    {
        if (audioSource.clip == targetClip && audioSource.isPlaying) return;

        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        if (instant || fadeDuration <= 0f)
        {
            audioSource.clip = targetClip;
            audioSource.volume = 1f;
            audioSource.Play();
        }
        else
        {
            fadeRoutine = StartCoroutine(FadeToClip(targetClip));
        }
    }

    IEnumerator FadeToClip(AudioClip newClip)
    {
        float startVolume = audioSource.volume;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        audioSource.clip = newClip;
        audioSource.Play();

        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = 1f;
        fadeRoutine = null;
    }
}
