using UnityEngine;
using UnityEngine.UI;

public class MainPowerupButtonController : MonoBehaviour
{
    public Button button;
    public GameManager gameManager;
    public PowerupManager powerupManager;

    public AudioSource audioSource;
    public AudioClip clickSound;
    public float volume = 1f;

    public MainPowerup powerupType;
    public int cost = 1000;

    void Start()
    {
        button.onClick.AddListener(PlaySound);

        if (button == null)
            button = GetComponent<Button>();

        if (gameManager == null)
            gameManager = FindAnyObjectByType<GameManager>();

        if (powerupManager == null)
            powerupManager = PowerupManager.Instance ?? FindAnyObjectByType<PowerupManager>();
    }

    void PlaySound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound, volume);
        }
    }

    void Update()
    {
        if (button == null || gameManager == null)
            return;

        button.interactable = gameManager.CurrentScore >= cost;
    }

    public void OnPowerupButtonPressed()
    {
        gameManager.AddScore(-cost, true);

        powerupManager.EquipMain(powerupType);
    }
}
