using UnityEngine;
using UnityEngine.UI;

public class HairDecayUI : MonoBehaviour
{
    public NPCHairController target;
    public Image hairImage;
    public Sprite[] uiSprites;

    int _lastStage = -1;

    void LateUpdate()
    {
        if (target == null || hairImage == null || uiSprites == null || uiSprites.Length == 0)
            return;

        int stage = Mathf.Clamp(target.currentStage, 0, uiSprites.Length - 1);

        if (stage == _lastStage)
            return;

        _lastStage = stage;
        hairImage.sprite = uiSprites[stage];
        hairImage.enabled = hairImage.sprite != null;
    }
}