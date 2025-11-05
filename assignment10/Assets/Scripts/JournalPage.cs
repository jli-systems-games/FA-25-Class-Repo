using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class StickerState
{
    public string stickerId;
    public Vector2 pos;
    public float scale;
    public float rotZ;
    public int order;
}

public class JournalPage : MonoBehaviour
{
    public RectTransform pageCanvas;
    public GameObject stickerPrefab;
    public List<StickerState> states = new();

    public void AddSticker(Sprite sprite, string id)
    {
        var go = Instantiate(stickerPrefab, pageCanvas);
        var inst = go.GetComponent<StickerInstance>();
        inst.Init(sprite, id);
        go.GetComponent<RectTransform>().anchoredPosition = Random.insideUnitCircle * 40f;
        SaveSnapshot();
    }

    public void SaveSnapshot()
    {
        states.Clear();
        for (int i = 0; i < pageCanvas.childCount; i++)
        {
            var t = pageCanvas.GetChild(i) as RectTransform;
            var si = t.GetComponent<StickerInstance>();
            if (!si) continue;
            states.Add(new StickerState
            {
                stickerId = si.stickerId,
                pos = t.anchoredPosition,
                scale = t.localScale.x,
                rotZ = t.localEulerAngles.z,
                order = t.GetSiblingIndex()
            });
        }
    }

    public void LoadFromSnapshot(System.Func<string, Sprite> idToSprite)
    {
        for (int i = pageCanvas.childCount - 1; i >= 0; i--) Destroy(pageCanvas.GetChild(i).gameObject);
        states.Sort((a, b) => a.order.CompareTo(b.order));
        foreach (var s in states)
        {
            var go = Instantiate(stickerPrefab, pageCanvas);
            var inst = go.GetComponent<StickerInstance>();
            inst.Init(idToSprite(s.stickerId), s.stickerId);
            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = s.pos;
            rt.localScale = Vector3.one * s.scale;
            rt.localEulerAngles = new Vector3(0, 0, s.rotZ);
        }
    }
}
