using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class NoteJudge : MonoBehaviour
{
    [Header("判定设置")]
    public KeyCode targetKey = KeyCode.F;
    public float judgeWindow = 0.3f;

    [Header("可选UI")]
    public GameObject successUI;
    public GameObject failUI;

    public UnityEvent onSuccess;
    public UnityEvent onFail;

    private bool _isJudging = true;
    private double _startTime;

    void Start()
    {
        _startTime = Time.unscaledTimeAsDouble;
        StartCoroutine(JudgeRoutine());
    }

    private IEnumerator JudgeRoutine()
    {
        while (_isJudging && Time.unscaledTimeAsDouble - _startTime < judgeWindow)
        {
            if (Input.GetKeyDown(targetKey))
            {
                HandleResult(true);
                yield break;
            }
            yield return null;
        }
        HandleResult(false);
    }

    private void HandleResult(bool success)
    {
        _isJudging = false;

        if (success)
        {
            successUI?.SetActive(true);
            onSuccess?.Invoke();
        }
        else
        {
            failUI?.SetActive(true);
            onFail?.Invoke();
        }

        // 延迟销毁
        Destroy(gameObject, 0.3f);
    }
}