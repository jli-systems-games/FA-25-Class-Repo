using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Bubble : MonoBehaviour
{
    void Start()
    {
        if (BubbleManager.Instance != null)
            BubbleManager.Instance.RegisterBubble();
    }
    [Header("UI References")]
    public Button mainButton;
    public Image mainImage;
    

    [Header("Sprites (顺序)")]
    public Sprite initialSprite;
    public Sprite[] progressSprites;

    [Header("Audio")]
    public AudioSource sfx;
    public AudioClip[] popClips;
    [Range(0f,1f)] public float volume = 0.9f;
    public Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    [Header("Press FX")]
    public float pressScale = 1.15f;
    public float pressTime  = 0.08f;
    public float recoverTime= 0.08f;

    int index = 0;
    bool busyAnimating = false;
    Vector3 baseScale;

    void Awake()
    {
        if (mainImage == null && mainButton != null)
            mainImage = mainButton.GetComponent<Image>();

        baseScale = mainButton.transform.localScale;

        mainButton.onClick.AddListener(OnMainPressed);
    

        if (initialSprite != null)
            mainImage.sprite = initialSprite;
        else if (progressSprites != null && progressSprites.Length > 0)
            mainImage.sprite = progressSprites[0];

        mainButton.interactable = true;
    
        index = 0;
    }

    void OnMainPressed()
    {
        if (!busyAnimating) StartCoroutine(AdvanceOneStep());
    }

    IEnumerator AdvanceOneStep()
    {
        busyAnimating = true;

        
        if (sfx && popClips != null && popClips.Length > 0)
        {
            sfx.pitch = Random.Range(pitchRange.x, pitchRange.y);
            var clip = popClips[Random.Range(0, popClips.Length)];
            sfx.PlayOneShot(clip, volume);
        }

        
        yield return ScaleTo(mainButton.transform, baseScale * pressScale, pressTime);

        
        if (index < progressSprites.Length)
        {
            int next = Mathf.Min(index, progressSprites.Length - 1);
            mainImage.sprite = progressSprites[next];
            index++;
        }

        
        yield return ScaleTo(mainButton.transform, baseScale, recoverTime);

    if (index >= 4)
        {
            
            if (BubbleManager.Instance != null)
                BubbleManager.Instance.BubbleCleared();
            
            mainButton.interactable = false;
            gameObject.SetActive(false);
        }

        busyAnimating = false;
    }

    IEnumerator ScaleTo(Transform t, Vector3 target, float dur)
    {
        Vector3 from = t.localScale;
        float tmr = 0f;
        while (tmr < dur)
        {
            tmr += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(tmr / dur);
            t.localScale = Vector3.Lerp(from, target, k);
            yield return null;
        }
        t.localScale = target;
    }

    public void ResetAll()
    {
        StopAllCoroutines();
        busyAnimating = false;

        if (initialSprite != null)
            mainImage.sprite = initialSprite;
        else if (progressSprites != null && progressSprites.Length > 0)
            mainImage.sprite = progressSprites[0];

        index = 0;
        mainButton.transform.localScale = baseScale;
        mainButton.interactable = true;
    
    }
}