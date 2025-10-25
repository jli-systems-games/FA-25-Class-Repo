using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class PandaManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider hungerBar;
    public Slider energyBar;
    public Slider funBar;
    public Slider cleanlinessBar;
    public TMP_Text moodText;

    [Header("Pet Data")]
    public PandaData pandaData; // ScriptableObject holding rates and values

    private float hunger;
    private float energy;
    private float fun;
    private float cleanliness;

    private PandaState currentState;

    void Start()
    {
        // Safety: warn if references missing
        if (pandaData == null)
            Debug.LogWarning("PandaData not assigned on PandaManager.");

        if (hungerBar == null || energyBar == null || funBar == null || cleanlinessBar == null)
            Debug.LogWarning("One or more Slider references on PandaManager are not assigned.");

        // Ensure sliders use 0..100 range to match your 0-100 stat values.
        ConfigureSliderRange(hungerBar);
        ConfigureSliderRange(energyBar);
        ConfigureSliderRange(funBar);
        ConfigureSliderRange(cleanlinessBar);

        // Initialize values from ScriptableObject (guard for null)
        hunger = pandaData != null ? pandaData.startHunger : 100f;
        energy = pandaData != null ? pandaData.startEnergy : 100f;
        fun = pandaData != null ? pandaData.startFun : 100f;
        cleanliness = pandaData != null ? pandaData.startCleanliness : 100f;

        UpdateUI();
        UpdateState();
    }

    void Update()
    {
        if (pandaData != null)
        {
            // Drain stats over time
            hunger -= pandaData.hungerDrain * Time.deltaTime;
            energy -= pandaData.energyDrain * Time.deltaTime;
            fun -= pandaData.funDrain * Time.deltaTime;
            cleanliness -= pandaData.cleanlinessDrain * Time.deltaTime;
        }

        ClampStats();
        UpdateState();
        UpdateUI();

        // 🐼 Check for Game Over condition
        if (hunger <= 0 || energy <= 0 || fun <= 0 || cleanliness <= 0)
        {
            StartCoroutine(GameOverTransition());
        }
    }

    // Player actions
    public void Feed()
    {
        hunger += (pandaData != null ? pandaData.feedGain : 10f);
        ClampStats();
        UpdateState();
        UpdateUI();
    }

    public void Sleep()
    {
        energy += (pandaData != null ? pandaData.sleepGain : 10f);
        ClampStats();
        UpdateState();
        UpdateUI();
    }

    public void Play()
    {
        fun += (pandaData != null ? pandaData.playGain : 10f);
        ClampStats();
        UpdateState();
        UpdateUI();
    }

    public void Wash()
    {
        cleanliness += (pandaData != null ? pandaData.washGain : 10f);
        ClampStats();
        UpdateState();
        UpdateUI();
    }

    // Keep all values in valid range
    void ClampStats()
    {
        hunger = Mathf.Clamp(hunger, 0, 100);
        energy = Mathf.Clamp(energy, 0, 100);
        fun = Mathf.Clamp(fun, 0, 100);
        cleanliness = Mathf.Clamp(cleanliness, 0, 100);
    }

    // Decide Panda mood
    void UpdateState()
    {
        if (hunger < 25)
            currentState = PandaState.Hungry;
        else if (energy < 25)
            currentState = PandaState.Tired;
        else if (fun < 25)
            currentState = PandaState.Bored;
        else if (cleanliness < 25)
            currentState = PandaState.Dirty;
        else
            currentState = PandaState.Happy;

        if (moodText != null)
            moodText.text = "Mood: " + currentState.ToString();
    }

    // Update all slider UI
    void UpdateUI()
    {
        // We set sliders to use 0..100 range, so assign direct values.
        if (hungerBar != null)
            hungerBar.value = hunger;
        if (energyBar != null)
            energyBar.value = energy;
        if (funBar != null)
            funBar.value = fun;
        if (cleanlinessBar != null)
            cleanlinessBar.value = cleanliness;
    }

    // Configure slider to 0..100 range safely
    void ConfigureSliderRange(Slider s)
    {
        if (s == null) return;
        s.minValue = 0f;
        s.maxValue = 100f;
        s.wholeNumbers = false; // allow smooth changes
    }

    // 🕹️ Smooth transition to GameOver
    IEnumerator GameOverTransition()
    {
        enabled = false; // stop updates

        // Optional: flash the screen or play sad sound
        Debug.Log("💔 Panda is unhappy — Game Over!");

        yield return new WaitForSeconds(1.5f); // delay before switching
        SceneManager.LoadScene("GameOver");
    }

    // Accessor for other scripts (like animation)
    public PandaState CurrentState
    {
        get { return currentState; }
    }
}