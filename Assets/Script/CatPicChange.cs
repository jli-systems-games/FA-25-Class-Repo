using UnityEngine;
using UnityEngine.UI;

public enum PetEmotion
{
    Ecstatic,
    Happy,
    Neutral,
    Sad,
    Depressed,
    Angry
}

public class PetEmotionController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public PetEmotion currentEmotion;
    public Sprite ecstaticSprite;
    public Sprite happySprite;
    public Sprite neutralSprite;
    public Sprite sadSprite;
    public Sprite depressedSprite;
    public Sprite angrySprite;
    public float checkInterval = 0.5f;

    float timer;

    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            timer = 0f;
            UpdateEmotionFromGameState();
        }
    }

    void UpdateEmotionFromGameState()
    {
        if (GameState.Instance == null) return;
        int happy = GameState.Instance.Happy;

        if (happy > 80) currentEmotion = PetEmotion.Ecstatic;
        else if (happy > 60) currentEmotion = PetEmotion.Happy;
        else if (happy > 40) currentEmotion = PetEmotion.Neutral;
        else if (happy > 20) currentEmotion = PetEmotion.Sad;
        else if (happy > 10) currentEmotion = PetEmotion.Angry;
        else currentEmotion = PetEmotion.Depressed;

        switch (currentEmotion)
        {
            case PetEmotion.Ecstatic: SetSprite(ecstaticSprite); break;
            case PetEmotion.Happy: SetSprite(happySprite); break;
            case PetEmotion.Neutral: SetSprite(neutralSprite); break;
            case PetEmotion.Sad: SetSprite(sadSprite); break;
            case PetEmotion.Angry: SetSprite(angrySprite); break;
            case PetEmotion.Depressed: SetSprite(depressedSprite); break;
        }
    }

    void SetSprite(Sprite newSprite)
    {
        if (!spriteRenderer || newSprite == null) return;

        spriteRenderer.sprite = newSprite;

        float targetHeight = 2.0f;
        float spriteHeight = newSprite.bounds.size.y;

        if (spriteHeight > 0)
        {
            float scale = targetHeight / spriteHeight;
            transform.localScale = new Vector3(scale, scale, 1f);
        }
    }
    
}