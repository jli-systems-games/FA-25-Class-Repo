using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Stats")]
    public int desire = 5;
    public int health = 7;
    public int ecstacy = 0;
    public int token = 0;

    [Header("UI")]
    public Slider desireSlider;
    public Slider healthSlider;
    public Slider ecstacySlider;
    public TMP_Text tokenText;

    [Header("Buttons")]
    public Button playButton;
    public Button comfortButton;
    public Button kissButton;

    [Header("Extras")]
    public Board board;
    public GameObject healthWarning;
    public GameObject boredText;
    public GameObject idleDoll;
    public GameObject kneeDoll;

    [Header("SFX")]
    public AudioSource audioSource;
    public AudioSource backgroundMusic;
    public AudioClip kissTriggerSFX;
    public AudioClip backgroundMusicClip;
    private bool kissReadyPlayed = false;

    private int maxDesire = 10;
    private int maxHealth = 10;
    private int maxEcstacy = 3;


    void Start()
    {
        backgroundMusic.clip = backgroundMusicClip;
        backgroundMusic.loop = true;
        backgroundMusic.Play();

        desireSlider.maxValue = maxDesire;
        healthSlider.maxValue = maxHealth;
        ecstacySlider.maxValue = maxEcstacy;

        playButton.interactable = false;
        comfortButton.interactable = false;
        kissButton.interactable = false;

        UpdateUI();
    }

    private void Update()
    {
        playButton.interactable = token > 0;
        comfortButton.interactable = token > 0;


        bool canKiss = desire >= 7 && health >= 3 && health <= 7;
        kissButton.interactable = token > 0 && canKiss;

        if (canKiss && !kissReadyPlayed)
        {
            audioSource.PlayOneShot(kissTriggerSFX);
            idleDoll.SetActive(false);
            kneeDoll.SetActive(true);
            kissReadyPlayed = true;
        }
        else if (!canKiss && kissReadyPlayed)
        {
            idleDoll.SetActive(true);
            kneeDoll.SetActive(false);
            kissReadyPlayed = false;
        }
    }

    public void AddPoints()
    {
        token++;
        UpdateUI();
    }

    public void Play()
    {
        desire = Mathf.Min(desire + 3, maxDesire);
        health = Mathf.Max(health - 2, 0);
        token--;
        CheckState();
    }

    public void Comfort()
    {
        desire = Mathf.Max(desire - 1, 0);
        health = Mathf.Min(health + 3, maxHealth);
        token--;
        CheckState();
    }

    public void Kiss()
    {
        board.ClearAllTiles();
        ecstacy = Mathf.Min(ecstacy + 1, maxEcstacy);
        desire = Mathf.Max(desire - 4, 0);
        health = Mathf.Max(health - 2, 0);
        token--;
        CheckState();
    }

    void CheckState()
    {
        healthWarning.SetActive(health < 3);
        if (desire < 3)
            boredText.SetActive(true);

        if (desire <= 0 || health <= 0)
        {
            SceneManager.LoadScene("LoseScene");
        }
        if (ecstacy >= maxEcstacy)
        {
            SceneManager.LoadScene("WinScene");
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        desireSlider.value = desire;
        healthSlider.value = health;
        ecstacySlider.value = ecstacy;

        if (tokenText != null)
            tokenText.text = token.ToString();
    }
    public void SwitchToScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
