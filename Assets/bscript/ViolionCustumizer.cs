using UnityEngine;
using UnityEngine.SceneManagement;

public class ViolinCustomizer : MonoBehaviour
{
    [Header("Sprite Options")]
    public SpriteRenderer bodyRenderer;
    public Sprite[] bodySprites;

    public SpriteRenderer[] stringRenderers;
    public Sprite[] stringSprites;

    [Header("Sound Options")]
    public AudioClip[] stringAudioOptions;

    // Player selections
    private int selectedBodyIndex;
    private int[] selectedStringSprite = new int[4];
    private int[] selectedStringSound = new int[4];

    public void ChooseBody(int index)
    {
        selectedBodyIndex = index;
        bodyRenderer.sprite = bodySprites[index];
        CentralData.current.bodySpriteIndex = index;
    }

    public void ChooseStringSprite(int stringIndex, int spriteIndex)
    {
        selectedStringSprite[stringIndex] = spriteIndex;
        stringRenderers[stringIndex].sprite = stringSprites[spriteIndex];
        CentralData.current.stringSpriteIndices[stringIndex] = spriteIndex;
    }

    public void ChooseStringSound(int stringIndex, int soundIndex)
    {
        selectedStringSound[stringIndex] = soundIndex;
        CentralData.current.stringSoundIndices[stringIndex] = soundIndex;
    }

    public void GoToPlayScene()
    {
        SceneManager.LoadScene("PlayScene");
    }
}
