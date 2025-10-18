using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class PowerRevealManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text revealText;
    public TMP_Text descriptionText;
    public Button continueButton;
    public Image backgroundImage;

    private string mageType;

    void Start()
    {
        mageType = DetermineMageType();
        Data.Instance.data_playerMageType = mageType;

        string playerName = Data.Instance.data_playerName;
        revealText.text = $" You are a {mageType} Mage!";
        SetElementStyle(mageType);

        continueButton.interactable = false;
        Invoke("EnableButton", 2f);
    }

    void EnableButton() => continueButton.interactable = true;

    // Determine the majority element type among the four selections
    string DetermineMageType()
    {
        int[] elements = {
            Data.Instance.data_robeIndex,
            Data.Instance.data_hatIndex,
            Data.Instance.data_wandIndex,
            Data.Instance.data_accessoryIndex
        };

        int water = 0, earth = 0, fire = 0;
        foreach (int el in elements)
        {
            switch (el)
            {
                case 0: water++; break;
                case 1: earth++; break;
                case 2: fire++; break;
            }
        }

        // Find the majority
        if (water > earth && water > fire) return "Water";
        if (earth > water && earth > fire) return "Earth";
        if (fire > water && fire > earth) return "Fire";

        // Tiebreakers
        if (water == earth && water > fire) return "Water"; // Water/Earth tie
        if (water == fire && water > earth) return "Water"; // Water/Fire tie
        if (earth == fire && earth > water) return "Earth"; // Earth/Fire tie

        // All equal or 2-2-0, default to Water
        return "Water";
    }

    // Element color + text styling
    void SetElementStyle(string type)
    {
        switch (type)
        {
            case "Fire":
                descriptionText.text = "A pheonix rising from the flames, you are gifted with sparks igniting at your fingertips";
                revealText.color = new Color(1f, 0.5f, 0.2f); // orange-red
                if (backgroundImage) backgroundImage.color = new Color(0.15f, 0f, 0f);
                break;

            case "Water":
                descriptionText.text = "Flowing with the oceans, you have the ability to create mere droplets to harsh storms";
                revealText.color = new Color(0.4f, 0.7f, 1f); // blue
                if (backgroundImage) backgroundImage.color = new Color(0f, 0.1f, 0.2f);
                break;

            case "Earth":
                descriptionText.text = "One with nature, you're connection with the wild allows you to let life blossom.";
                revealText.color = new Color(0.3f, 0.8f, 0.3f); // green
                if (backgroundImage) backgroundImage.color = new Color(0.05f, 0.15f, 0.05f);
                break;
        }
    }

    public void OnContinuePressed()
    {
        switch (Data.Instance.data_playerMageType)
        {
            case "Fire": SceneManager.LoadScene("FireScene"); break;
            case "Water": SceneManager.LoadScene("WaterScene"); break;
            case "Earth": SceneManager.LoadScene("EarthScene"); break;
        }
    }
}