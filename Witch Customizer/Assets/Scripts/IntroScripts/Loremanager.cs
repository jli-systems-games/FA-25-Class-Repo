using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoreManager : MonoBehaviour
{
    public TMP_Text storyText;
    public TMP_InputField nameInput;
    public GameObject namePanel;

    private string[] lines = {
        "Long ago, witches studied the elements to restore balance to the world.",
        "Now, under the crimson moon, a new apprentice awakens...",
        "But first, they must remember their name."
    };

    private int currentLine = 0;

    void Start()
    {
        storyText.text = lines[currentLine];
        namePanel.SetActive(false);
    }

    public void NextLine()
    {
        currentLine++;
        if (currentLine < lines.Length)
            storyText.text = lines[currentLine];
        else
            namePanel.SetActive(true);
    }

    public void OnContinuePressed()
    {
        string playerName = nameInput.text;
        if (string.IsNullOrWhiteSpace(playerName)) playerName = "Apprentice";
        Data.Instance.data_playerName = playerName;
        SceneManager.LoadScene("CustomizerScene");
    }
}
