using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KeyPressButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Refs")]
    public Image targetImage;
    public Sprite upSprite;
    public Sprite downSprite;

    [Header("Sound")]
    public AudioSource audioSrc;  
    public AudioClip downSound;
    public AudioClip upSound;

    void Reset()
    {
        targetImage = GetComponent<Image>();
        audioSrc = GetComponent<AudioSource>();
    }

    void Awake()
    {
        if (targetImage == null) targetImage = GetComponent<Image>();
        if (audioSrc == null)
        {
            audioSrc = gameObject.AddComponent<AudioSource>();
            audioSrc.playOnAwake = false;
        }

        if (targetImage != null && upSprite != null)
            targetImage.sprite = upSprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (targetImage != null && downSprite != null)
            targetImage.sprite = downSprite;

        if (audioSrc != null && downSound != null)
            audioSrc.PlayOneShot(downSound);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (targetImage != null && upSprite != null)
            targetImage.sprite = upSprite;

        if (audioSrc != null && upSound != null)
            audioSrc.PlayOneShot(upSound);
    }
}