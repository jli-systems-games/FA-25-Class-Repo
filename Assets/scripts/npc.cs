using UnityEngine;
using System.Collections;

public class NPCEmotion : MonoBehaviour
{
    public GameObject normalSprite;   // default
    public GameObject emotionSprite;  // alternate emotion
    public float emotionTime = 2f;

    public void ChangeEmotion()
    {
        StopAllCoroutines();
        StartCoroutine(ShowEmotion());
    }

    IEnumerator ShowEmotion()
    {
        if (normalSprite != null) normalSprite.SetActive(false);
        if (emotionSprite != null) emotionSprite.SetActive(true);

        yield return new WaitForSeconds(emotionTime);

        if (normalSprite != null) normalSprite.SetActive(true);
        if (emotionSprite != null) emotionSprite.SetActive(false);
    }
}
