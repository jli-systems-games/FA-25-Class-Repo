using UnityEngine;
using UnityEngine.UI;

public class AimButtonController : MonoBehaviour
{
    public Button button;
    public WaterShooter shooter;
    public GameManager gameManager;

    public AudioSource audioSource;
    public AudioClip clickSound;
    public float volume = 1f;

    public PowerupManager powerupManager;

    public int cost = 5000;

    void Start()
    {
        button.onClick.AddListener(PlaySound);

        if (button == null)
            button = GetComponent<Button>();
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

    public void OnAimButtonPressed()
    {
        if (gameManager.CurrentScore >= cost)
        {
            gameManager.SpendScoreInstant(cost);
            powerupManager.EquipAimDrop();
        }
    }
}
