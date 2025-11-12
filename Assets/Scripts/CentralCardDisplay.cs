using UnityEngine;
using UnityEngine.UI;

public class CentralCardDisplay : MonoBehaviour
{
    public Sprite backSprite;
    public Sprite[] faceSprites;
    private Image img;
    private bool isFaceUp = false;
    private int cardIndex;

    void Awake()
    {
        img = GetComponent<Image>();
    }

    public void SetCard(int index)
    {
        cardIndex = index;
        ShowBack();
    }

    public void Reveal()
    {
        if (!isFaceUp)
        {
            img.sprite = faceSprites[cardIndex];
            isFaceUp = true;
        }
    }

    public void ShowBack()
    {
        img.sprite = backSprite;
        isFaceUp = false;
    }
}
