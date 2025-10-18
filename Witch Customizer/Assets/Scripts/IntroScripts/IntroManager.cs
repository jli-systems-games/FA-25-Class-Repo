using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text loreText;
    public TMP_Text titleText;

    private string fullText =
        "Greeting, dear Mage...\n\n" +
        "I am Celeste Nyx, High Witch of the Enchanted Moors and Head Professor of the Namrah Institute.\n\n" +
        "Welcome to Namrah, an ancient academy founded in the 2nd Century — a place reserved only for those who display rare and extraordinary potential.\n\n" +
        "You have been chosen because your aura flickers with untamed power.\n\n" +
        "As a new apprentice, you may feel uncertain. But do not fret — your path will soon reveal itself through your own intuition.\n\n" +
        "To begin your magical journey, we will start simply...";

    void Start()
    {
        StartCoroutine(TypeText());
    }

    // ✨ Typewriter effect for your lore text
    System.Collections.IEnumerator TypeText()
    {
        loreText.text = "";
        foreach (char c in fullText)
        {
            loreText.text += c;
            yield return new WaitForSeconds(0.02f);
        }
    }

    // Called by the Continue button
    public void OnContinuePressed()
    {
        SceneManager.LoadScene("CustomizerScene");
    }
}
