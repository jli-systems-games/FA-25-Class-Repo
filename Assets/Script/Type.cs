using System.Collections;
using UnityEngine;
using TMPro;

public class Type : MonoBehaviour
{
    [Header("Text Settings")]
    public TMP_Text textUI;
    [TextArea] public string fullText;
    public float speed = 0.05f;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip typeSound;

    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        Play();
    }

    void Update()
    {
        if (isTyping && Input.anyKeyDown)
        {
            Skip();
        }
    }

    public void Play()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        isTyping = true;

        // 设置完整文字（包含 <color> 标签）
        textUI.text = fullText;
        textUI.maxVisibleCharacters = 0;

        int totalVisibleCharacters = fullText.Length;
        int counter = 0;

        while (counter <= totalVisibleCharacters)
        {
            textUI.maxVisibleCharacters = counter;

            // 播放音效（忽略空格和换行）
            if (counter < fullText.Length)
            {
                char c = fullText[counter];
                if (!char.IsWhiteSpace(c) && audioSource != null && typeSound != null)
                {
                    audioSource.PlayOneShot(typeSound);
                }
            }

            counter++;
            yield return new WaitForSeconds(speed);
        }

        isTyping = false;
    }

    public void Skip()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        textUI.maxVisibleCharacters = fullText.Length;
        isTyping = false;
    }
}