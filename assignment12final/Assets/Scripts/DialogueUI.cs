using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;

    [Header("Root")]
    public GameObject dialoguePanel;

    [Header("Text")]
    public TextMeshProUGUI dialogueText;

    [Header("Buttons")]
    public Button nextButton;
    public Button choiceAButton;
    public Button choiceBButton;
    public TextMeshProUGUI choiceAText;
    public TextMeshProUGUI choiceBText;

    private Action onNextCallback;
    private Action onChoiceACallback;
    private Action onChoiceBCallback;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        HideAll();
    }

    public void HideAll()
    {
        dialoguePanel.SetActive(false);
        nextButton.gameObject.SetActive(false);
        choiceAButton.gameObject.SetActive(false);
        choiceBButton.gameObject.SetActive(false);
    }

    public void ShowLine(string text, Action onNext)
    {
        dialoguePanel.SetActive(true);
        dialogueText.text = text;

        nextButton.gameObject.SetActive(true);
        choiceAButton.gameObject.SetActive(false);
        choiceBButton.gameObject.SetActive(false);

        nextButton.onClick.RemoveAllListeners();
        onNextCallback = onNext;

        nextButton.onClick.AddListener(() =>
        {
            dialoguePanel.SetActive(false);
            nextButton.gameObject.SetActive(false);
            onNextCallback?.Invoke();
        });
    }

    public void ShowChoices(string text,
        string optionAText, Action onA,
        string optionBText, Action onB)
    {
        dialoguePanel.SetActive(true);
        dialogueText.text = text;

        nextButton.gameObject.SetActive(false);
        choiceAButton.gameObject.SetActive(true);
        choiceBButton.gameObject.SetActive(true);

        choiceAText.text = optionAText;
        choiceBText.text = optionBText;

        choiceAButton.onClick.RemoveAllListeners();
        choiceBButton.onClick.RemoveAllListeners();

        onChoiceACallback = onA;
        onChoiceBCallback = onB;

        choiceAButton.onClick.AddListener(() =>
        {
            dialoguePanel.SetActive(false);
            onChoiceACallback?.Invoke();
        });

        choiceBButton.onClick.AddListener(() =>
        {
            dialoguePanel.SetActive(false);
            onChoiceBCallback?.Invoke();
        });
    }
}
