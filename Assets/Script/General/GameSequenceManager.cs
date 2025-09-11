using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSequenceManager : MonoBehaviour
{
    [Serializable]
    public class MiniGameSlot
    {
        [Tooltip("该序位的小游戏 Prefab（必须继承 MiniGameBase）")]
        public MiniGameBase prefab;

        [HideInInspector] public MiniGameBase instance;

        [Header("Next Step Delay")]
        public bool overrideNextDelay = false;    // 是否覆盖全局延时
        [Min(0f)] public float nextDelay = 0.75f; // 本关自定义延时（秒）
    }

    [Header("Sequence")]
    public List<MiniGameSlot> miniGames = new List<MiniGameSlot>();

    [Header("Spawn")]
    public Transform spawnParent;
    public bool autoStart = true;

    [Header("Global Next Delay")]
    [Min(0f)] public float defaultNextDelay = 0.75f; // 全局默认延时（秒）
    public bool useUnscaledDelay = false;            // 用不受 timeScale 影响的延时

    [Header("Timer (Optional)")]
    public CountdownTimer timer;    // 仍然是你之前的纯计时器；可为空

    private int _currentIndex = -1;
    private MiniGameBase _current;
    private Coroutine _delayRoutine;

    void OnEnable()
    {
        if (autoStart)
            StartSequence();
    }

    public void StartSequence()
    {
        // 清理旧的（如果你需要完全重开）
        if (_delayRoutine != null) { StopCoroutine(_delayRoutine); _delayRoutine = null; }
        if (_current != null)
        {
            Unhook(_current);
            _current.Cleanup();
            _current.gameObject.SetActive(false);
            _current = null;
        }
        _currentIndex = -1;
        LoadNext();
    }

    public void LoadNext()
    {
        // 收尾上一个
        if (_delayRoutine != null) { StopCoroutine(_delayRoutine); _delayRoutine = null; }

        if (_current != null)
        {
            Unhook(_current);
            _current.Cleanup();
            _current.gameObject.SetActive(false);
        }

        _currentIndex++;

        if (_currentIndex >= miniGames.Count)
        {
            Debug.Log("[GameSequenceManager] 全部小游戏完成。");
            _current = null;
            return;
        }

        var slot = miniGames[_currentIndex];
        if (slot == null || slot.prefab == null)
        {
            Debug.LogWarning($"[GameSequenceManager] 第 {_currentIndex} 个槽位没有 prefab，跳过。");
            LoadNext();
            return;
        }

        if (slot.instance == null)
        {
            slot.instance = Instantiate(slot.prefab, spawnParent ? spawnParent : transform);
            slot.instance.gameObject.name = $"{slot.prefab.name}_Instance";
        }

        _current = slot.instance;
        Hook(_current);

        _current.gameObject.SetActive(true);
        _current.Begin();
    }

    private void Hook(MiniGameBase mg)
    {
        if (mg == null) return;
        mg.Completed += OnMiniGameCompleted;
    }

    private void Unhook(MiniGameBase mg)
    {
        if (mg == null) return;
        mg.Completed -= OnMiniGameCompleted;
    }

    private void OnMiniGameCompleted(MiniGameBase mg)
    {
        // 成功后：可在这里重置你的全局计时器（如果还需要的话）
        if (timer != null)
            timer.ResetTo(7f, start: true);

        // 读取本关的延时设定（若覆盖则用本关值，否则用全局默认）
        var slot = miniGames[Mathf.Clamp(_currentIndex, 0, miniGames.Count - 1)];
        float delay = slot.overrideNextDelay ? Mathf.Max(0f, slot.nextDelay)
                                             : Mathf.Max(0f, defaultNextDelay);

        // 延时后进入下一关
        if (_delayRoutine != null) StopCoroutine(_delayRoutine);
        _delayRoutine = StartCoroutine(LoadNextAfterDelay(delay));
    }

    private IEnumerator LoadNextAfterDelay(float seconds)
    {
        if (seconds > 0f)
        {
            if (useUnscaledDelay)
            {
                float end = Time.unscaledTime + seconds;
                while (Time.unscaledTime < end) yield return null;
            }
            else
            {
                yield return new WaitForSeconds(seconds);
            }
        }
        LoadNext();
    }

    public void JumpToIndex(int targetIndex)
    {
        if (targetIndex < 0 || targetIndex >= miniGames.Count)
        {
            Debug.LogWarning("[GameSequenceManager] 目标索引越界。");
            return;
        }
        _currentIndex = targetIndex - 1;
        LoadNext();
    }
}