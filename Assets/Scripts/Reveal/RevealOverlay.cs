using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RevealOverlay : MonoBehaviour
{
    public ScreenFader fader;
    public Image p1Img;
    public Image p2Img;
    public TMP_Text outcomeText;

    public Sprite rockSprite;
    public Sprite paperSprite;
    public Sprite scissorSprite;

    public float fadeIn = 0.25f;
    public float hold = 1.5f;
    public float fadeOut = 0.25f;

    void Start()
    {
        ApplySnapshot();
        StartCoroutine(Sequence());
    }

    void ApplySnapshot()
    {
        var data = CentralData.I.reveal;

        if (p1Img) p1Img.sprite = ToSprite(data.p1Play);
        if (p2Img) p2Img.sprite = ToSprite(data.p2Play);

        if (outcomeText)
        {
            string n1 = CentralData.I.p1Name;
            string n2 = CentralData.I.p2Name;
            outcomeText.text = data.result == 0 ? "Draw"
                                : (data.result > 0 ? $"{n1} wins this round" : $"{n2} wins this round");
        }

        if (fader) fader.SetAlpha(0f);
    }

    IEnumerator Sequence()
    {
        if (fader) yield return fader.FadeTo(1f, fadeIn);
        yield return new WaitForSecondsRealtime(hold);
        if (fader) yield return fader.FadeTo(0f, fadeOut);

        CentralData.I.reveal.continued = true;
    }

    Sprite ToSprite(HandType h)
    {
        switch (h)
        {
            case HandType.Rock: return rockSprite;
            case HandType.Paper: return paperSprite;
            case HandType.Scissor: return scissorSprite;
        }
        return rockSprite;
    }
}