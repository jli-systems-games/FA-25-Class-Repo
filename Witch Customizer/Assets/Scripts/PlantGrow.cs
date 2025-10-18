using UnityEngine;
using UnityEngine.UI;

public class PlantGrow : MonoBehaviour
{
    [Header("Growth Sprites")]
    public Sprite[] growthStages; // 3 stages: Seed, Sprout, Bloom

    private int stage = 0;
    private Image image;
    private bool fullyGrown = false;

    void Start()
    {
        image = GetComponent<Image>();
        if (growthStages.Length > 0)
            image.sprite = growthStages[0]; // start as seed
    }

    public void OnClickGrow()
    {
        if (fullyGrown) return;

        stage++;
        if (stage >= growthStages.Length)
        {
            stage = growthStages.Length - 1;
            fullyGrown = true;
            FindObjectOfType<EarthMiniGame>().PlantGrown();
        }

        image.sprite = growthStages[stage];
    }
}
