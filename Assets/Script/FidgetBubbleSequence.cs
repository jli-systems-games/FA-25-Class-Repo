using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class FidgetBubbleItem : MonoBehaviour, IPointerClickHandler
{
    [Header("UI")]
    public Image bubbleImage;
    public Button bubbleButton;

    [Header("Sprites")]
    public Sprite initialSprite;
    public Sprite poppedSprite;

    [Header("FX")]
    public float pressScale = 1.12f;
    public float pressTime  = 0.07f;
    public float recoverTime= 0.07f;

    [Header("Audio")]
    public AudioSource sfx;
    public AudioClip[] popClips;
    [Range(0,1)] public float volume = 0.9f;
    public Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    [Header("Manager")]
    public FidgetBubbleManager manager;

    bool popped = false;
    bool animBusy = false;
    Vector3 baseScale;

    void Reset()
    {
        bubbleImage  = GetComponent<Image>();
        bubbleButton = GetComponent<Button>();
        sfx = GetComponent<AudioSource>();
    }

    void Awake()
    {
        if (bubbleImage  == null) bubbleImage  = GetComponent<Image>();
        if (bubbleButton == null) bubbleButton = GetComponent<Button>();
        if (sfx == null)
        {
            sfx = gameObject.AddComponent<AudioSource>();
            sfx.playOnAwake = false;
        }
        baseScale = transform.localScale;
        ApplyInitial();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!popped && !animBusy)
            StartCoroutine(PopOnce());
    }

    IEnumerator PopOnce()
    {
        animBusy = true;

        
        if (sfx && popClips != null && popClips.Length > 0)
        {
            sfx.pitch = Random.Range(pitchRange.x, pitchRange.y);
            sfx.PlayOneShot(popClips[Random.Range(0, popClips.Length)], volume);
        }

        
        yield return ScaleTo(baseScale * pressScale, pressTime);

        
        if (poppedSprite != null) bubbleImage.sprite = poppedSprite;
        popped = true;

        
        yield return ScaleTo(baseScale, recoverTime);

        
        if (bubbleButton) bubbleButton.interactable = false;
        

        
        if (manager) manager.OnBubblePopped(this);

        animBusy = false;
    }

    IEnumerator ScaleTo(Vector3 target, float dur)
    {
        Vector3 from = transform.localScale;
        float t = 0f;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            transform.localScale = Vector3.Lerp(from, target, Mathf.Clamp01(t / dur));
            yield return null;
        }
        transform.localScale = target;
    }

    public void ApplyInitial()
    {
        popped = false;
        animBusy = false;
        transform.localScale = baseScale;

        if (bubbleImage && initialSprite) bubbleImage.sprite = initialSprite;
        if (bubbleButton) bubbleButton.interactable = true;
        
    }

    public bool IsPopped() => popped;
}