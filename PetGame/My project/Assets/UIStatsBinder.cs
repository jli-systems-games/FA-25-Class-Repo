using UnityEngine;
using UnityEngine.UI;

public class UIStatsBinder : MonoBehaviour
{
    public PetController pet;
    public Slider hp;
    public Slider mood;
    public Slider energy;
    public Text hpText;
    public Text moodText;
    public Text energyText;
    public InventorySpawner spawner;
    public Button spawnBeerButton;
    public Button toggleStageButton;
    public Button resetButton;

    void Start()
    {
        if (spawnBeerButton) spawnBeerButton.onClick.AddListener(() => { if (spawner) spawner.SpawnBeer(); });
        if (toggleStageButton) toggleStageButton.onClick.AddListener(() => { if (pet) pet.ToggleStageMode(); });
        if (resetButton) resetButton.onClick.AddListener(() => { if (pet) pet.ResetPet(); });
    }

    void Update()
    {
        if (!pet) return;
        if (hp) hp.value = pet.vitals.hp / 100f;
        if (mood) mood.value = pet.vitals.mood / 100f;
        if (energy) energy.value = pet.vitals.energy / 100f;
        if (hpText) hpText.text = pet.vitals.hp.ToString();
        if (moodText) moodText.text = pet.vitals.mood.ToString();
        if (energyText) energyText.text = pet.vitals.energy.ToString();
    }
}
