using UnityEngine;
using UnityEngine.UI;

public class Torch : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite unlitSprite;
    public Sprite litSprite;

    [Header("Optional Effects")]
    public AudioSource flameSound; // optional: attach sound
    public ParticleSystem flameEffect; // optional: attach fire particles

    private bool isLit = false;
    private Image image;

    void Start()
    {
        image = GetComponent<Image>();

        // start unlit
        if (unlitSprite != null)
            image.sprite = unlitSprite;
    }

    // Called when player clicks the torch
    public void OnClickLight()
    {
        if (isLit) return; // prevents double click

        isLit = true;

        // Change sprite to lit version
        if (litSprite != null)
            image.sprite = litSprite;

        // Optional: play sound + particles
        if (flameSound != null) flameSound.Play();
        if (flameEffect != null) flameEffect.Play();

        // Notify game manager
        FireMiniGame fireGame = FindObjectOfType<FireMiniGame>();
        if (fireGame != null)
            fireGame.LightTorch();
    }
}
