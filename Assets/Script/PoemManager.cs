using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

public class PoemInputController : MonoBehaviour
{

    public TMP_InputField inputField;
    public TMP_Text currentAuthorText;
    public TMP_Text lastLineText;


    public int totalLines = 3;
    public bool trimWhitespace = true;


    public PoemData poemData;

    //upon finish writing put all lines together to diplay
    public UnityEvent onPoemFinished;

    private List<PlayerData> players;
    private int curPlayerIdx = 0;
    private int curLineIndex = 0;
    private bool isReady = false;

    private void Awake()
    {
        if (!inputField) inputField = GetComponent<TMP_InputField>();
        inputField.onSubmit.AddListener(_ => { if (isReady) SubmitCurrentLine(); });
        lastLineText.text = "";
        inputField.interactable = false; 
        currentAuthorText.text = "Ready...";
    }

    
    public void Initialize(int totalLinesFromMode)
    {
        totalLines = totalLinesFromMode;
        Initialize();
    }

    public void Initialize()
    {
        // player names loading
        players = RuntimePlayersRegistry.Instance?.players?
                    .Where(p => p != null)
                    .OrderBy(p => p.index)
                    .ToList();

        if (players == null || players.Count == 0)
        {
            Debug.LogError("[PoemInputController] Initialize 失败：没有玩家数据。");
            return;
        }

        // Storage
        var store = PoemDataStore.Instance;
        if (store == null)
        {
            Debug.LogError("PoemDataStore 未放入场景（需要有一个带 PoemDataStore 的物体且 DontDestroyOnLoad）。");
            return;
        }

        // can start new poem
        if (store.Current == null)
            store.StartNewPoem(totalLines);
        else
            store.ReloadPoem(totalLines);

        poemData = store.Current;   

        // ui and stuff
        curPlayerIdx = 0;
        curLineIndex = poemData.lines.Count; 
        isReady = true;

        inputField.interactable = true;
        UpdateCurrentAuthorLabel();
        FocusInput();
     
    }
    // put new names on the placeholder
    private void UpdateCurrentAuthorLabel()
    {
        string name = players[curPlayerIdx].playerName;
        currentAuthorText.text = string.IsNullOrEmpty(name) ? "Player" : name;
    }

    private void FocusInput()
    {
        inputField.text = "";
        inputField.ActivateInputField();
        inputField.Select();
        inputField.caretPosition = 0;
    }

    public void SubmitCurrentLine()
    {
        if (!isReady) return;
        if (curLineIndex >= totalLines) return;

        string raw = inputField.text ?? "";
        string text = trimWhitespace ? raw.Trim() : raw;
        if (string.IsNullOrEmpty(text))
        {
            FocusInput();
            return;
        }

        var line = ScriptableObject.CreateInstance<PoemLine>();
        line.authorName = players[curPlayerIdx].playerName;
        line.text = text;
        line.lineIndex = curLineIndex;

        poemData.lines.Add(line);
        lastLineText.text = text;

        //next round
        curLineIndex++;
        curPlayerIdx = (curPlayerIdx + 1) % players.Count;

        if (curLineIndex >= totalLines)
        {
            inputField.interactable = false;
            onPoemFinished?.Invoke();
            return;
        }

        UpdateCurrentAuthorLabel();
        FocusInput();
    }
}