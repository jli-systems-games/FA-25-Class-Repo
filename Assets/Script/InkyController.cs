using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InkyController : MonoBehaviour
{
    public TextAsset inkJSONAsset;

    public TMP_Text textUI;
    public Transform choicesParent;
    public Button choiceButtonPrefab;

    public List<SignalAction> signals = new();

    public List<SignalPause> signalPauses = new();

    public List<EndingRoute> endingRoutes = new();

    [Serializable] public class SignalAction
    {
        public string key;
        public List<GameObject> toActivate;
        public List<GameObject> toDeactivate;
        public bool caseInsensitive = true;
    }
    [Serializable] public class SignalPause
    {
        public string key;
        public float seconds = 0f;
        public bool caseInsensitive = true;
    }
    [Serializable] public class EndingRoute
    {
        public string signalKey;
        public string sceneName;
        public float delay = 0f;
        public bool caseInsensitive = true;
    }

    private Story _story;
    private readonly List<Button> _spawnedChoices = new();
    private bool _waitingPause = false;
    private string _lastChosenText = null;

    void Start()
    {
        if (!inkJSONAsset) { Debug.LogError("InkyController: 缺少 Ink JSON"); enabled = false; return; }
        _story = new Story(inkJSONAsset.text);
        RefreshView();
    }

    public void RefreshView()
    {
        if (_waitingPause) return;

        ClearChoices();
        var sb = new StringBuilder();
        while (_story.canContinue)
        {
            string text = _story.Continue().TrimEnd('\n', '\r');

            if (string.IsNullOrEmpty(text))
            {
                break;
            }

            if (!string.IsNullOrEmpty(text))
            {
                if (sb.Length > 0) sb.Append('\n');
                sb.Append(text);
            }

            HandleTags(_story.currentTags);
            if (_waitingPause) break;
        }

        if (textUI) textUI.text = sb.ToString();
        if (_story.currentChoices.Count > 0)
        {
            for (int i = 0; i < _story.currentChoices.Count; i++)
            {
                var choice = _story.currentChoices[i];
                var btn = Instantiate(choiceButtonPrefab, choicesParent);
                var label = btn.GetComponentInChildren<TMP_Text>();
                if (label) label.text = choice.text;

                int idx = i;
                btn.onClick.AddListener(() =>
                {
                    _lastChosenText = _story.currentChoices[idx].text;
                    OnClickChoice(idx);
                });
                _spawnedChoices.Add(btn);
            }
        }
        else
        {
            if (!_story.canContinue && !_waitingPause)
                Debug.Log("InkyController: Story finished.");
        }
    }

    void OnClickChoice(int index)
    {
        _story.ChooseChoiceIndex(index);
        RefreshView();
    }

    void ClearChoices()
    {
        foreach (var b in _spawnedChoices) if (b) Destroy(b.gameObject);
        _spawnedChoices.Clear();
    }

    void HandleTags(List<string> tags)
    {
        if (tags == null || tags.Count == 0) return;

        foreach (var raw in tags)
        {
            string tag = raw.Trim();

            if (tag.StartsWith("SIGNAL:", StringComparison.OrdinalIgnoreCase))
            {
                string key = tag.Substring(7).Trim();

                TriggerSignal(key);
                TryRouteBySignal(key);
                TryPauseForSignal(key);

                if (_waitingPause) return;
            }

            if (tag.StartsWith("PAUSE:", StringComparison.OrdinalIgnoreCase))
            {
                if (float.TryParse(tag.Substring(6).Trim(), out var sec) && sec > 0f)
                    StartCoroutine(ResumeAfterDelay(sec));
            }
        }
    }

    void TriggerSignal(string key)
    {
        foreach (var s in signals)
        {
            bool match = s.caseInsensitive
                ? string.Equals(s.key, key, StringComparison.OrdinalIgnoreCase)
                : s.key == key;

            if (!match) continue;

            if (s.toActivate != null) foreach (var go in s.toActivate) if (go) go.SetActive(true);
            if (s.toDeactivate != null) foreach (var go in s.toDeactivate) if (go) go.SetActive(false);
        }
    }

    void TryRouteBySignal(string key)
    {
        foreach (var r in endingRoutes)
        {
            bool match = r.caseInsensitive
                ? string.Equals(r.signalKey, key, StringComparison.OrdinalIgnoreCase)
                : r.signalKey == key;

            if (match && !string.IsNullOrEmpty(r.sceneName))
            {
                StartCoroutine(LoadSceneAfter(r.sceneName, r.delay));
                return;
            }
        }
    }

    void TryPauseForSignal(string key)
    {
        float delay = 0f;

        foreach (var p in signalPauses)
        {
            bool match = p.caseInsensitive
                ? string.Equals(p.key, key, StringComparison.OrdinalIgnoreCase)
                : p.key == key;

            if (match && p.seconds > 0f) { delay = p.seconds; break; }
        }

        if (delay <= 0f)
        {
            if (key.EndsWith("_Pre", StringComparison.OrdinalIgnoreCase)) delay = 2f;
            else if (key.EndsWith("_Post", StringComparison.OrdinalIgnoreCase)) delay = 1f;
        }

        if (delay > 0f) StartCoroutine(ResumeAfterDelay(delay));
    }

    IEnumerator ResumeAfterDelay(float seconds)
    {
        _waitingPause = true;
        yield return new WaitForSecondsRealtime(seconds);
        _waitingPause = false;
        RefreshView();
    }

    IEnumerator LoadSceneAfter(string scene, float delay)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(scene);
    }

    public void ContinueStoryFromAnimation()
    {
        if (_waitingPause) { _waitingPause = false; }
        RefreshView();
    }
}