using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WitchCustomizer : MonoBehaviour
{
    [Header("UI Dropdowns")]
    public TMP_Dropdown robeDropdown;
    public TMP_Dropdown hatDropdown;
    public TMP_Dropdown wandDropdown;
    public TMP_Dropdown accessoryDropdown;

    [Header("Preview Images")]
    public Image robe;
    public Image hat;
    public Image wand;
    public Image accessory;

    [Header("Sprites")]
    public Sprite[] robeSprites;
    public Sprite[] hatSprites;
    public Sprite[] wandSprites;
    public Sprite[] accessorySprites;

    [Header("Name Display")]
    public TMP_Text nameText;

    void Start()
    {
        nameText.text = Data.Instance.data_playerName;
        UpdateRobe(); UpdateHat(); UpdateWand(); UpdateAccessory();
    }

    public void UpdateRobe() => robe.sprite = robeSprites[robeDropdown.value];
    public void UpdateHat() => hat.sprite = hatSprites[hatDropdown.value];
    public void UpdateWand() => wand.sprite = wandSprites[wandDropdown.value];
    public void UpdateAccessory() => accessory.sprite = accessorySprites[accessoryDropdown.value];

    public void ConfirmChoices()
    {
        Data.Instance.data_robeIndex = robeDropdown.value;
        Data.Instance.data_hatIndex = hatDropdown.value;
        Data.Instance.data_wandIndex = wandDropdown.value;
        Data.Instance.data_accessoryIndex = accessoryDropdown.value;
        SceneManager.LoadScene("PowerRevealScene");
    }
}
