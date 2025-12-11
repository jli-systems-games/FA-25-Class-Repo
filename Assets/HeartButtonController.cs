using UnityEngine;
using UnityEngine.UI;

public class HeartButtonController : MonoBehaviour
{
    public Button button;
    public WaterShooter shooter;
    public GameManager gameManager;

    public PowerupManager powerupManager;

    public AudioSource audioSource;
    public AudioClip clickSound;
    public float volume = 1f;

    public int cost = 5000;

    void Start()
    {
        button.onClick.AddListener(PlaySound);

        if (button == null)
            button = GetComponent<Button>();
        powerupManager = PowerupManager.Instance;
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
        if (gameManager.CurrentScore < cost)
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }
    }

    public void OnHeartButtonPressed()
    {
        if (gameManager.CurrentScore >= cost)
        {
            gameManager.SpendScoreInstant(cost);
            powerupManager.EquipHeartDrop();
        }
    }
}
