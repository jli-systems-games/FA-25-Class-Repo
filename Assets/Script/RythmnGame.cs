using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SignalJudgment : MonoBehaviour
{
    [System.Serializable]
    public class LaneConfig
    {
        public KeyCode key = KeyCode.F;
        public GameObject prefab;
        public Vector3 spawnPosition;
        public UnityEvent onSuccess;
    }

    
    public float judgeWindow = 0.3f;
    public bool useUnscaledTime = true;

    
    public LaneConfig leftLane = new LaneConfig { key = KeyCode.F, spawnPosition = new Vector3(-200, 0, 0) };
    public LaneConfig rightLane = new LaneConfig { key = KeyCode.J, spawnPosition = new Vector3(200, 0, 0) };
    public Transform spawnParent;
    public bool alternateSpawnPoints = true;

    
    public GameObject failUI;
    public float uiAutoHideTime = 0.3f;
    public UnityEvent onFail;

  
    public bool destroySpawnedOnResult = true;

    private bool _useLeftNext = true;

    public void Trigger()
    {
        LaneConfig lane = alternateSpawnPoints && _useLeftNext ? leftLane : rightLane;
        if (alternateSpawnPoints) _useLeftNext = !_useLeftNext;

        if (lane.prefab == null) return;

        GameObject note = Instantiate(lane.prefab, spawnParent);
        note.transform.localPosition = lane.spawnPosition;

        var runner = note.AddComponent<NoteRunner>();
        runner.Init(this, lane);
    }

    private class NoteRunner : MonoBehaviour
    {
        private SignalJudgment _parent;
        private LaneConfig _lane;
        private double _deadline;
        private GameObject _localSuccess, _localFail;

        public void Init(SignalJudgment parent, LaneConfig lane)
        {
            _parent = parent;
            _lane = lane;
            _deadline = Now() + _parent.judgeWindow;

            _localSuccess = FindChild("Success");
            _localFail = FindChild("Fail");
            if (_localSuccess) _localSuccess.SetActive(false);
            if (_localFail) _localFail.SetActive(false);

            StartCoroutine(Judge());
        }

        private IEnumerator Judge()
        {
            while (Now() < _deadline)
            {
                if (Input.GetKeyDown(_lane.key))
                {
                    OnResult(true);
                    yield break;
                }
                yield return null;
            }
            OnResult(false);
        }

        private void OnResult(bool success)
        {
            if (_localSuccess) _localSuccess.SetActive(success);
            if (_localFail) _localFail.SetActive(!success);

            if (success)
            {
                _lane.onSuccess?.Invoke();
            }
            else
            {
                if (_parent.failUI) _parent.failUI.SetActive(true);
                _parent.onFail?.Invoke();
                if (_parent.uiAutoHideTime > 0) StartCoroutine(HideUIAfter(_parent.uiAutoHideTime));
            }

            if (_parent.destroySpawnedOnResult) Destroy(gameObject);
            else Destroy(this);
        }

        private IEnumerator HideUIAfter(float t)
        {
            yield return _parent.useUnscaledTime ? new WaitForSecondsRealtime(t) : new WaitForSeconds(t);
            if (_parent.failUI) _parent.failUI.SetActive(false);
        }

        private double Now() => _parent.useUnscaledTime ? Time.unscaledTimeAsDouble : Time.timeAsDouble;

        private GameObject FindChild(string name)
        {
            Transform t = transform.Find(name);
            if (t) return t.gameObject;
            foreach (Transform c in transform)
            {
                GameObject r = FindChild(c, name);
                if (r) return r;
            }
            return null;
        }

        private GameObject FindChild(Transform root, string name)
        {
            Transform t = root.Find(name);
            if (t) return t.gameObject;
            foreach (Transform c in root)
            {
                GameObject r = FindChild(c, name);
                if (r) return r;
            }
            return null;
        }
    }
}