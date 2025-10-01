using UnityEngine;

public class PlayerHideState : MonoBehaviour
{
    public bool IsHidden { get; private set; }
    public CanvasGroup vignette;

    public void SetHidden(bool hidden)
    {
        IsHidden = hidden;
        if (vignette) vignette.alpha = hidden ? 0.8f : 0.0f;
    }
}
