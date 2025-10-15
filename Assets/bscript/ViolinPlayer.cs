using UnityEngine;

public class ViolinPlayer : MonoBehaviour
{
    [Header("Renderers")]
    public SpriteRenderer bodyRenderer;
    public SpriteRenderer[] stringRenderers;

    [Header("Assets")]
    public Sprite[] bodySprites;
    public Sprite[] stringSprites;
    public AudioClip[] stringAudioOptions;

    [Header("Audio")]
    public AudioSource[] stringAudioSources;

    void Start()
    {
        // Apply body
        int bodyIndex = CentralData.current.bodySpriteIndex;
        bodyRenderer.sprite = bodySprites[bodyIndex];

        // Apply strings
        for (int i = 0; i < 4; i++)
        {
            int sIndex = CentralData.current.stringSpriteIndices[i];
            stringRenderers[i].sprite = stringSprites[sIndex];

            int sndIndex = CentralData.current.stringSoundIndices[i];
            stringAudioSources[i].clip = stringAudioOptions[sndIndex];
        }
    }

    public void PlayString(int stringIndex)
    {
        stringAudioSources[stringIndex].Play();
    }
}
