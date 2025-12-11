using UnityEngine;
using UnityEngine.UI;

public class ButtonClickSFX : MonoBehaviour
{
    public AudioClip clickSFX;
    public float volume = 1f;

    AudioSource source;

    void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
    }

    void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(PlaySFX);
        }
    }

    void PlaySFX()
    {
        if (clickSFX != null)
        {
            source.PlayOneShot(clickSFX, volume);
        }
    }
}
