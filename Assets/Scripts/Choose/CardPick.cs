using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardPick : MonoBehaviour
{
    public Button rockBtn;
    public Button paperBtn;
    public Button scissorBtn;

    public TMP_Text totalText;
    public TMP_Text rockText;
    public TMP_Text paperText;
    public TMP_Text scissorText;

    public Button clearBtn;
    public Button readyBtn;

    const int TOTAL = 9;
    int r, p, s;

    public System.Action<int, int, int> OnReady;

    void Start()
    {
        rockBtn.onClick.AddListener(() => Adjust(ref r, +1));
        paperBtn.onClick.AddListener(() => Adjust(ref p, +1));
        scissorBtn.onClick.AddListener(() => Adjust(ref s, +1));

        if (clearBtn)
            clearBtn.onClick.AddListener(() => ResetCounts());

        if (readyBtn)
            readyBtn.onClick.AddListener(() => {
                if (r + p + s == TOTAL)
                    OnReady?.Invoke(r, p, s);
            });

        ResetCounts();
    }

    void Adjust(ref int v, int delta)
    {
        int sum = r + p + s;
        if (delta > 0 && sum >= TOTAL) return;
        v += delta;
        Refresh();
    }

    void Refresh()
    {
        if (rockText) rockText.text = $"Rock: {r}";
        if (paperText) paperText.text = $"Paper: {p}";
        if (scissorText) scissorText.text = $"Scissor: {s}";
        if (totalText) totalText.text = $"Total: {r + p + s} / {TOTAL}";

        if (readyBtn) readyBtn.interactable = (r + p + s == TOTAL);
    }

    public void ResetCounts()
    {
        r = p = s = 0;
        Refresh();
    }
}