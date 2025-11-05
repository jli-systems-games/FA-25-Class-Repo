using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public class GachaPanelFX : MonoBehaviour
{
    [Header("Core Refs")]
    public GachaRoller roller;
    public Transform cardContainer;
    public GameObject gachaCardPrefab; 
    public InventoryUI inventoryUI;

    [Header("FX Overlay & Stars")]
    public CanvasGroup fxOverlay;
    public RectTransform starBurstRoot;
    public Sprite starSprite;

    [Header("Timings")]
    public float overlayIn = 0.15f;
    public float overlayOut = 0.2f;
    public float starsDuration = 0.3f;
    public int starsCount = 18;
    public float starsRadius = 220f;
    public float cardIntroDelay = 0.06f;
    public float cardIntroIn = 0.18f;
    public float cardIntroSettle = 0.06f;

    [Header("Rarity Glow Colors")]
    public Color commonGlow = new Color(1f, 1f, 1f, 0f);
    public Color rareGlow = new Color(0.35f, 0.7f, 1f, 0.6f);
    public Color epicGlow = new Color(1f, 0.6f, 0.2f, 0.75f);

    [Header("UI")]
    public Button rollButton;
    public TMP_Text toastText;

    [Header("Panel-wide Cleanup")]
    [SerializeField] Transform panelRoot;
    [SerializeField] string[] killSpriteNames = { "5.asset" };  
    private readonly List<GameObject> liveCards = new();
    private bool _alive;
    private bool _rolling;
    private Coroutine _toastCo;

    void OnEnable() { _alive = true; }
    void OnDisable() { _alive = false; _rolling = false; }

    public void OnClickRoll()
    {
        if (!isActiveAndEnabled || _rolling) return;
        StartCoroutine(DoRollSequence());
    }

    IEnumerator DoRollSequence()
    {
        _rolling = true;
        if (rollButton) rollButton.interactable = false;

        KillBySpriteNamesInPanel();
        ClearCardContainer();
        SetCardsVisible(false);

        yield return FadeCanvas(fxOverlay, 0f, 0.35f, overlayIn);
        yield return StarBurst(starBurstRoot, starSprite, starsCount, starsRadius, starsDuration);

        if (!_alive) { _rolling = false; yield break; }

        var picks = roller.RollFiveUnique();
        SpawnCards(picks);

        yield return PlayCardsIntro(liveCards, cardIntroDelay, cardIntroIn, cardIntroSettle);

        float from = fxOverlay ? fxOverlay.alpha : 0.35f;
        yield return FadeCanvas(fxOverlay, from, 0f, overlayOut);

        if (rollButton) rollButton.interactable = true;
        _rolling = false;
    }

    void ClearCardContainer()
    {
        liveCards.Clear();
        for (int i = cardContainer.childCount - 1; i >= 0; i--)
            Destroy(cardContainer.GetChild(i).gameObject);

        var rt = cardContainer as RectTransform;
        if (rt) LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
    }

    void KillBySpriteNamesInPanel()
    {
        if (!panelRoot || killSpriteNames == null || killSpriteNames.Length == 0) return;
        var imgs = panelRoot.GetComponentsInChildren<Image>(true);
        foreach (var img in imgs)
        {
            if (!img || !img.sprite) continue;
            var name = img.sprite.name;
            for (int i = 0; i < killSpriteNames.Length; i++)
            {
                if (name == killSpriteNames[i])
                {
                    img.sprite = null;
                    img.enabled = false;
                    break;
                }
            }
        }
    }

    void KillBySpriteNamesInTransform(Transform root)
    {
        if (!root || killSpriteNames == null || killSpriteNames.Length == 0) return;
        var imgs = root.GetComponentsInChildren<Image>(true);
        foreach (var img in imgs)
        {
            if (!img || !img.sprite) continue;
            var name = img.sprite.name;
            for (int i = 0; i < killSpriteNames.Length; i++)
            {
                if (name == killSpriteNames[i])
                {
                    img.sprite = null;
                    img.enabled = false;
                    break;
                }
            }
        }
    }

    void SpawnCards(List<StickerEntry> entries)
    {
        foreach (var entry in entries)
        {
            var go = Instantiate(gachaCardPrefab, cardContainer);
            go.transform.SetAsLastSibling(); 
            liveCards.Add(go);

            KillBySpriteNamesInTransform(go.transform);

            var cg = go.GetComponent<CanvasGroup>();
            if (!cg) cg = go.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            go.transform.localScale = Vector3.one * 0.8f;

            var icon = go.transform.Find("Icon")?.GetComponent<Image>();
            if (icon)
            {
                icon.sprite = entry.sprite;
                icon.preserveAspect = true;
                icon.color = Color.white;
            }

            var glowImg = go.transform.Find("Glow")?.GetComponent<Image>();
            if (glowImg)
            {
                var c = entry.rarity switch
                {
                    Rarity.Rare => rareGlow,
                    Rarity.Epic => epicGlow,
                    _ => commonGlow
                };
                glowImg.color = c;
            }

            var labelTMP = go.transform.Find("Label")?.GetComponent<TMP_Text>();
            if (labelTMP) labelTMP.text = entry.rarity.ToString();

            var btn = go.transform.Find("Btn_Add")?.GetComponent<Button>();
            if (btn)
            {
                bool already = inventoryUI && inventoryUI.Has(entry.id);
                btn.interactable = !already;

                var btnTxt = btn.GetComponentInChildren<TMP_Text>();
                if (btnTxt) btnTxt.text = already ? "Got" : "Add";

                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    if (inventoryUI && !inventoryUI.Has(entry.id))
                    {
                        inventoryUI.AddToInventory(entry);
                        btn.interactable = false;
                        if (btnTxt) btnTxt.text = "Got";
                        Toast("Added to inventory");
                    }
                    else
                    {
                        Toast("Already Got");
                    }
                });
            }
        }
    }

    void SetCardsVisible(bool on)
    {
        foreach (var go in liveCards)
        {
            if (!go) continue;
            var cg = go.GetComponent<CanvasGroup>() ?? go.AddComponent<CanvasGroup>();
            cg.alpha = on ? 1f : 0f;
            go.transform.localScale = on ? Vector3.one : Vector3.one * 0.8f;
        }
    }

    IEnumerator FadeCanvas(CanvasGroup cg, float from, float to, float dur)
    {
        if (!_alive || !cg || !cg.gameObject) yield break;
        cg.blocksRaycasts = true;
        cg.alpha = from;

        float t = 0f;
        while (t < dur)
        {
            if (!_alive || !cg || !cg.gameObject) yield break;
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / dur);
            yield return null;
        }
        if (cg && cg.gameObject)
        {
            cg.alpha = to;
            cg.blocksRaycasts = (to > 0.01f);
        }
    }

    IEnumerator StarBurst(RectTransform root, Sprite sprite, int count, float radius, float dur)
    {
        if (!_alive || !root || !root.gameObject || !sprite) yield break;

        var created = new List<GameObject>();
        for (int i = 0; i < count; i++)
        {
            if (!_alive || !root || !root.gameObject) break;

            var go = new GameObject("star", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
            go.transform.SetParent(root, false);

            var img = go.GetComponent<Image>(); img.sprite = sprite; img.preserveAspect = true;
            var rt = go.GetComponent<RectTransform>(); rt.sizeDelta = new Vector2(18, 18);
            var cg = go.GetComponent<CanvasGroup>(); cg.alpha = 0f;

            float ang = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang));
            Vector2 p0 = Vector2.zero;
            Vector2 p1 = dir * Random.Range(radius * 0.6f, radius);

            if (isActiveAndEnabled) StartCoroutine(AnimStar(rt, cg, p0, p1, dur * Random.Range(0.9f, 1.2f)));
            created.Add(go);
        }

        float wait = dur * 1.2f, w = 0f;
        while (w < wait)
        {
            if (!_alive) yield break;
            w += Time.unscaledDeltaTime;
            yield return null;
        }

        foreach (var g in created) if (g) Destroy(g);
    }

    IEnumerator AnimStar(RectTransform rt, CanvasGroup cg, Vector2 p0, Vector2 p1, float dur)
    {
        float t = 0f;
        while (t < dur)
        {
            if (!_alive || !rt || !cg || !rt.gameObject || !cg.gameObject) yield break;

            t += Time.unscaledDeltaTime;
            float u = t / dur;

            cg.alpha = (u < 0.5f) ? Mathf.Lerp(0, 1, u * 2f) : Mathf.Lerp(1, 0, (u - 0.5f) * 2f);
            rt.anchoredPosition = Vector2.Lerp(p0, p1, Mathf.SmoothStep(0, 1, u));
            float s = Mathf.Lerp(0.6f, 1.2f, Mathf.Sin(u * Mathf.PI));
            rt.localScale = Vector3.one * s;
            rt.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(0, 180f, u));
            yield return null;
        }
    }

    IEnumerator PlayCardsIntro(List<GameObject> cards, float stepDelay, float inDur, float settleDur)
    {
        foreach (var c in cards)
        {
            if (!_alive || !c) yield break;

            var cg = c.GetComponent<CanvasGroup>() ?? c.AddComponent<CanvasGroup>();
            var rt = c.GetComponent<RectTransform>();
            rt.localScale = Vector3.one * 0.8f;
            cg.alpha = 0f;

            float t = 0f;
            while (t < inDur)
            {
                if (!_alive || !rt || !cg || !rt.gameObject || !cg.gameObject) yield break;
                t += Time.unscaledDeltaTime;
                float u = t / inDur;
                rt.localScale = Vector3.one * Mathf.Lerp(0.8f, 1.06f, u);
                cg.alpha = u;
                yield return null;
            }
            t = 0f;
            while (t < settleDur)
            {
                if (!_alive || !rt || !rt.gameObject) yield break;
                t += Time.unscaledDeltaTime;
                rt.localScale = Vector3.one * Mathf.Lerp(1.06f, 1f, t / settleDur);
                yield return null;
            }

            float d = 0f;
            while (d < stepDelay)
            {
                if (!_alive) yield break;
                d += Time.unscaledDeltaTime;
                yield return null;
            }
        }
    }

    void Toast(string msg)
    {
        if (!toastText) return;
        if (_toastCo != null) StopCoroutine(_toastCo);
        _toastCo = StartCoroutine(ToastCo(msg));
    }

    IEnumerator ToastCo(string msg)
    {
        if (!_alive || !toastText) yield break;

        toastText.text = msg;
        var cg = toastText.GetComponent<CanvasGroup>() ?? toastText.gameObject.AddComponent<CanvasGroup>();
        if (!cg || !cg.gameObject) yield break;

        cg.alpha = 0f;
        float t = 0f, fin = 0.15f, stay = 0.6f, fout = 0.25f;

        while (t < fin)
        {
            if (!_alive || !cg || !cg.gameObject) yield break;
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(0, 1, t / fin);
            yield return null;
        }

        float s = 0f;
        while (s < stay)
        {
            if (!_alive) yield break;
            s += Time.unscaledDeltaTime;
            yield return null;
        }

        t = 0f;
        while (t < fout)
        {
            if (!_alive || !cg || !cg.gameObject) yield break;
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(1, 0, t / fout);
            yield return null;
        }
        if (cg && cg.gameObject) cg.alpha = 0f;
    }
}
