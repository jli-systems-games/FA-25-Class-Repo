using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

namespace GreatAchievement.Systems
{
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance { get; private set; }

        [Serializable]
        public class MusicGroup
        {
            [Tooltip("Music group name")]
            public string groupName;
            
            [Tooltip("Background music for this group")]
            public AudioClip musicClip;
            
            [Tooltip("Scene names that belong to this group")]
            public string[] sceneNames;
            
            [Tooltip("Music volume (0-1)")]
            [Range(0f, 1f)]
            public float volume = 1f;
        }

        [Header("Music Settings")]
        [Tooltip("Music groups - configure which scenes play which music")]
        public MusicGroup[] musicGroups;

        [Header("Transition Settings")]
        [Tooltip("Fade duration in seconds")]
        public float fadeDuration = 1.5f;

        [Header("Default Settings")]
        [Tooltip("Default music for unconfigured scenes (optional)")]
        public AudioClip defaultMusic;
        
        [Tooltip("Default volume")]
        [Range(0f, 1f)]
        public float defaultVolume = 0.7f;

        private AudioSource audioSourceA;
        private AudioSource audioSourceB;
        private AudioSource currentAudioSource;
        
        private MusicGroup currentMusicGroup;
        private Coroutine fadeCoroutine;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudioSources();
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void Start()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            PlayMusicForScene(currentSceneName, false);
        }

        private void InitializeAudioSources()
        {
            audioSourceA = gameObject.AddComponent<AudioSource>();
            audioSourceB = gameObject.AddComponent<AudioSource>();

            ConfigureAudioSource(audioSourceA);
            ConfigureAudioSource(audioSourceB);

            currentAudioSource = audioSourceA;
        }

        private void ConfigureAudioSource(AudioSource source)
        {
            source.loop = true;
            source.playOnAwake = false;
            source.volume = 0f;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            PlayMusicForScene(scene.name, true);
        }

        private void PlayMusicForScene(string sceneName, bool withFade)
        {
            MusicGroup targetGroup = GetMusicGroupForScene(sceneName);

            if (targetGroup == currentMusicGroup && currentAudioSource != null && currentAudioSource.isPlaying)
            {
                return;
            }

            if (withFade)
            {
                CrossFadeToMusic(targetGroup);
            }
            else
            {
                PlayMusicImmediate(targetGroup);
            }
        }

        private MusicGroup GetMusicGroupForScene(string sceneName)
        {
            if (musicGroups != null)
            {
                foreach (var group in musicGroups)
                {
                    if (group.sceneNames != null)
                    {
                        foreach (var scene in group.sceneNames)
                        {
                            if (scene == sceneName)
                            {
                                return group;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private void PlayMusicImmediate(MusicGroup group)
        {
            currentMusicGroup = group;

            AudioClip clipToPlay = group != null ? group.musicClip : defaultMusic;
            float targetVolume = group != null ? group.volume : defaultVolume;

            if (clipToPlay == null)
            {
                if (currentAudioSource != null)
                {
                    currentAudioSource.Stop();
                }
                return;
            }

            currentAudioSource.clip = clipToPlay;
            currentAudioSource.volume = targetVolume;
            currentAudioSource.time = 0f;
            currentAudioSource.Play();
        }

        private void CrossFadeToMusic(MusicGroup group)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(CrossFadeCoroutine(group));
        }

        private IEnumerator CrossFadeCoroutine(MusicGroup targetGroup)
        {
            AudioSource fadeOutSource = currentAudioSource;
            AudioSource fadeInSource = (currentAudioSource == audioSourceA) ? audioSourceB : audioSourceA;

            AudioClip clipToPlay = targetGroup != null ? targetGroup.musicClip : defaultMusic;
            float targetVolume = targetGroup != null ? targetGroup.volume : defaultVolume;

            if (clipToPlay == null)
            {
                yield return StartCoroutine(FadeOut(fadeOutSource, fadeDuration));
                currentMusicGroup = null;
                yield break;
            }

            fadeInSource.clip = clipToPlay;
            fadeInSource.volume = 0f;
            fadeInSource.time = 0f;
            fadeInSource.Play();

            float timer = 0f;
            float startVolumeOut = fadeOutSource.volume;

            while (timer < fadeDuration)
            {
                timer += Time.unscaledDeltaTime;
                float progress = timer / fadeDuration;

                if (fadeOutSource.isPlaying)
                {
                    fadeOutSource.volume = Mathf.Lerp(startVolumeOut, 0f, progress);
                }

                fadeInSource.volume = Mathf.Lerp(0f, targetVolume, progress);

                yield return null;
            }

            fadeOutSource.volume = 0f;
            fadeOutSource.Stop();
            fadeInSource.volume = targetVolume;

            currentAudioSource = fadeInSource;
            currentMusicGroup = targetGroup;
        }

        private IEnumerator FadeOut(AudioSource source, float duration)
        {
            float startVolume = source.volume;
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime;
                source.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
                yield return null;
            }

            source.volume = 0f;
            source.Stop();
        }

        public void SetMasterVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            
            if (currentAudioSource != null && currentAudioSource.isPlaying)
            {
                float targetVolume = currentMusicGroup != null ? currentMusicGroup.volume : defaultVolume;
                currentAudioSource.volume = targetVolume * volume;
            }
        }

        public void PauseMusic()
        {
            if (currentAudioSource != null)
            {
                currentAudioSource.Pause();
            }
        }

        public void ResumeMusic()
        {
            if (currentAudioSource != null)
            {
                currentAudioSource.UnPause();
            }
        }

        public void StopMusic()
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            if (audioSourceA != null) audioSourceA.Stop();
            if (audioSourceB != null) audioSourceB.Stop();
            
            currentMusicGroup = null;
        }

        public void PlayMusicGroup(string groupName, bool withFade = true)
        {
            MusicGroup targetGroup = null;
            
            if (musicGroups != null)
            {
                foreach (var group in musicGroups)
                {
                    if (group.groupName == groupName)
                    {
                        targetGroup = group;
                        break;
                    }
                }
            }

            if (targetGroup != null)
            {
                if (withFade)
                {
                    CrossFadeToMusic(targetGroup);
                }
                else
                {
                    PlayMusicImmediate(targetGroup);
                }
            }
        }
    }
}
