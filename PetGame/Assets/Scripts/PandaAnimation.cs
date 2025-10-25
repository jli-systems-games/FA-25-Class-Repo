using UnityEngine;
using UnityEngine.UI;

public class PandaAnimation : MonoBehaviour
{
    public PandaManager manager;
    public Image pandaImage; // <- UI Image component instead of SpriteRenderer

    public Sprite happySprite;
    public Sprite hungrySprite;
    public Sprite tiredSprite;
    public Sprite dirtySprite;
    public Sprite boredSprite;

    void Update()
    {
        if (!manager || !pandaImage) return;

        switch (manager.CurrentState)
        {
            case PandaState.Happy:
                pandaImage.sprite = happySprite;
                break;
            case PandaState.Hungry:
                pandaImage.sprite = hungrySprite;
                break;
            case PandaState.Tired:
                pandaImage.sprite = tiredSprite;
                break;
            case PandaState.Dirty:
                pandaImage.sprite = dirtySprite;
                break;
            case PandaState.Bored:
                pandaImage.sprite = boredSprite;
                break;
        }
    }
}
