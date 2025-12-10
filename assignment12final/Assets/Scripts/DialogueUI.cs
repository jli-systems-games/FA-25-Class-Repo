using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;

    public static bool IsDialogueOpen { get; private set; }

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    [Header("Choice Buttons (for mouse click)")]
    public GameObject choice1Button;
    public GameObject choice2Button;
    public TextMeshProUGUI choice1Text;
    public TextMeshProUGUI choice2Text;

    [Header("Typewriter Settings")]
    public float charsPerSecond = 35f;

    private Coroutine lineRoutine;
    private bool isTyping;

    private Action currentLineCallback;

    private Action choice1Callback;
    private Action choice2Callback;

    private enum DialogueMode
    {
        None,
        Line,
        Choices
    }

    private DialogueMode currentMode = DialogueMode.None;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (choice1Button != null) choice1Button.SetActive(false);
        if (choice2Button != null) choice2Button.SetActive(false);

        IsDialogueOpen = false;
    }

    private void Update()
    {
        if (currentMode == DialogueMode.Line && !isTyping && dialoguePanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space) ||
                Input.GetKeyDown(KeyCode.Return) ||
                Input.GetKeyDown(KeyCode.E))
            {
                EndLineAndCallback();
            }
        }

        if (currentMode == DialogueMode.Choices && !isTyping && dialoguePanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) ||
                Input.GetKeyDown(KeyCode.Keypad1))
            {
                SelectChoice(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) ||
                     Input.GetKeyDown(KeyCode.Keypad2))
            {
                SelectChoice(2);
            }
        }
    }

    public void ShowLine(string text, Action onFinished = null)
    {
        if (lineRoutine != null)
        {
            StopCoroutine(lineRoutine);
            lineRoutine = null;
        }

        IsDialogueOpen = true;

        currentLineCallback = onFinished;
        lineRoutine = StartCoroutine(RunLineRoutine(text));
    }

    public void ShowChoices(string line,
                            string option1, Action option1Action,
                            string option2, Action option2Action)
    {
        if (lineRoutine != null)
        {
            StopCoroutine(lineRoutine);
            lineRoutine = null;
        }

        IsDialogueOpen = true;

        choice1Callback = option1Action;
        choice2Callback = option2Action;

        lineRoutine = StartCoroutine(RunChoicesRoutine(line, option1, option2));
    }

    public void OnClickChoice1()
    {
        if (currentMode == DialogueMode.Choices && !isTyping)
        {
            SelectChoice(1);
        }
    }

    public void OnClickChoice2()
    {
        if (currentMode == DialogueMode.Choices && !isTyping)
        {
            SelectChoice(2);
        }
    }


    IEnumerator RunLineRoutine(string text)
    {
        currentMode = DialogueMode.Line;

        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (choice1Button != null) choice1Button.SetActive(false);
        if (choice2Button != null) choice2Button.SetActive(false);

        yield return StartCoroutine(TypeLine(text));
    }

    IEnumerator RunChoicesRoutine(string line, string option1, string option2)
    {
        currentMode = DialogueMode.Choices;

        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (choice1Button != null) choice1Button.SetActive(false);
        if (choice2Button != null) choice2Button.SetActive(false);

        yield return StartCoroutine(TypeLine(line));

        if (choice1Button != null) choice1Button.SetActive(true);
        if (choice2Button != null) choice2Button.SetActive(true);

        if (choice1Text != null) choice1Text.text = option1;
        if (choice2Text != null) choice2Text.text = option2;

    }

    IEnumerator TypeLine(string fullText)
    {
        isTyping = true;

        if (dialogueText == null)
        {
            Debug.LogWarning("DialogueUI: dialogueText is not assigned.");
            yield break;
        }

        dialogueText.text = "";

        float t = 0f;
        int charIndex = 0;
        int length = fullText.Length;

        while (charIndex < length)
        {
            if (Input.GetKeyDown(KeyCode.Space) ||
                Input.GetKeyDown(KeyCode.Return))
            {
                dialogueText.text = fullText;
                break;
            }

            t += Time.deltaTime * charsPerSecond;
            int newIndex = Mathf.FloorToInt(t);

            if (newIndex != charIndex)
            {
                charIndex = Mathf.Clamp(newIndex, 0, length);
                dialogueText.text = fullText.Substring(0, charIndex);
            }

            yield return null;
        }

        dialogueText.text = fullText;
        isTyping = false;
    }

    void EndLineAndCallback()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (choice1Button != null) choice1Button.SetActive(false);
        if (choice2Button != null) choice2Button.SetActive(false);

        currentMode = DialogueMode.None;

        IsDialogueOpen = false;

        var cb = currentLineCallback;
        currentLineCallback = null;
        cb?.Invoke();
    }

    void SelectChoice(int index)
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (choice1Button != null) choice1Button.SetActive(false);
        if (choice2Button != null) choice2Button.SetActive(false);

        currentMode = DialogueMode.None;

        IsDialogueOpen = false;

        Action cb = null;
        if (index == 1) cb = choice1Callback;
        else if (index == 2) cb = choice2Callback;

        choice1Callback = null;
        choice2Callback = null;

        cb?.Invoke();
    }
}
