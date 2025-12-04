using TMPro;
using UnityEngine;

public class AchievementUI : MonoBehaviour
{
    public TMP_Text titleText;
    public AnimationClip showClip;

    public Color commonColor = Color.black;
    public Color rareColor = new Color(1f, 0.2f, 0.2f);
    public Color legendaryColor = new Color(0.6f, 0f, 0.8f);

    public void Setup(string title, Rarity rarity)
    {

        switch (rarity)
        {
            case Rarity.Common:
                titleText.color = commonColor;
                titleText.text = title + " +123";
                break;

            case Rarity.Rare:
                titleText.color = rareColor;
                titleText.text = title + " +234";

                break;

            case Rarity.Legendary:
                titleText.color = legendaryColor;
                titleText.text = title + " +456";

                break;
        }

        Destroy(gameObject, showClip.length);
    }
    private void OnDestroy()
    {
        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.OnAchievementUIClosed(this);
        }
    }
}
