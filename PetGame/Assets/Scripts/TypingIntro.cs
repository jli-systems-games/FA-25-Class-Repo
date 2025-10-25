using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class TypingIntro : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text introText;
    public GameObject continueButton;

    [Header("Typing Settings")]
    [TextArea(3, 6)]
    public string fullText = "Hello! Congrats on being a Panda Nanny! Time to see if you can keep this baby panda happy!";
    public float typingSpeed = 0.05f;

    [Header("Next Scene")]
    public string nextSceneName = "Main"; // ✅ your actual gameplay scene

    void Start()
    {
        introText.text = "";
        continueButton.SetActive(false);
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        foreach (char c in fullText)
        {
            introText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Show the button when typing finishes
        continueButton.SetActive(true);
    }

    public void ContinueToGame()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}

