using UnityEngine;

public class FidgetBubbleManager : MonoBehaviour
{
    [Header("Bubbles")]
    public FidgetBubbleItem[] bubbles;
    public int resetAfterCount = 10;

    int poppedCount = 0;

    void Awake()
    {
        
        foreach (var b in bubbles)
        {
            if (b != null) b.manager = this;
        }
        ResetAll();
    }

    public void OnBubblePopped(FidgetBubbleItem item)
    {
        poppedCount++;
        if (poppedCount >= resetAfterCount)
        {
            ResetAll();
        }
    }

    public void ResetAll()
    {
        poppedCount = 0;
        foreach (var b in bubbles)
        {
            if (b != null) b.ApplyInitial();
        }
    }
}