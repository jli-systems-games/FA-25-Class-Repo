using UnityEngine;

public class FreezeUILayout : MonoBehaviour
{
    [System.Serializable]
    public class Target
    {
        public RectTransform rt;
        [HideInInspector] public Vector2 anchoredPos;
        [HideInInspector] public Vector2 sizeDelta;
        [HideInInspector] public Vector3 scale;
    }

    public Target[] targets;

    void Awake()
    {
        if (targets == null) return;
        for (int i = 0; i < targets.Length; i++)
        {
            var t = targets[i];
            if (t == null || t.rt == null) continue;
            t.anchoredPos = t.rt.anchoredPosition;
            t.sizeDelta = t.rt.sizeDelta;
            t.scale = t.rt.localScale;
        }
    }

    void LateUpdate()
    {
        if (targets == null) return;
        for (int i = 0; i < targets.Length; i++)
        {
            var t = targets[i];
            if (t == null || t.rt == null) continue;
            t.rt.anchoredPosition = t.anchoredPos;
            t.rt.sizeDelta = t.sizeDelta;
            t.rt.localScale = t.scale;
        }
    }
}
