using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Hierarchy Refs")]
    [Tooltip("Parent container for sticker icons (usually InventoryBar under Viewport).")]
    public Transform slotParent;

    [Tooltip("Small icon prefab (Button root, child Image named 'Icon').")]
    public GameObject stickerIconPrefab;

    [Tooltip("Sticker prefab placed on a journal page (UI Image + StickerInstance).")]
    public GameObject stickerItemPrefab;

    [Tooltip("Journal controller that provides the current page.")]
    public JournalController journal;

    [Header("Options")]
    [Tooltip("Play a tiny shake when trying to add a duplicate.")]
    public bool duplicateShakeFeedback = true;

    [Tooltip("Child name of the Image inside StickerIcon prefab.")]
    public string iconImageChildName = "Icon";

    private HashSet<string> owned = new HashSet<string>();

    private readonly Dictionary<string, GameObject> idToIcon = new Dictionary<string, GameObject>();

    public bool Has(string id) => owned.Contains(id);

    public IEnumerable<string> GetOwnedIds() => owned;

    public void LoadOwned(IEnumerable<string> ids)
    {
        owned = new HashSet<string>(ids ?? new List<string>());
    }

    public void ClearIcons()
    {
        idToIcon.Clear();
        for (int i = slotParent.childCount - 1; i >= 0; i--)
            Destroy(slotParent.GetChild(i).gameObject);
    }

    public void AddToInventory(StickerEntry entry)
    {
        if (entry == null || entry.sprite == null || string.IsNullOrEmpty(entry.id))
            return;

        if (owned.Contains(entry.id))
        {
            if (duplicateShakeFeedback && idToIcon.TryGetValue(entry.id, out var ico))
                SafeStart(PunchScale(ico.transform)); 
            return;
        }

        owned.Add(entry.id);

        var iconGO = Instantiate(stickerIconPrefab, slotParent);
        idToIcon[entry.id] = iconGO;

        var iconImgTr = iconGO.transform.Find(iconImageChildName);
        if (!iconImgTr)
        {
            Debug.LogWarning($"StickerIcon prefab is missing child Image named '{iconImageChildName}'.");
        }
        else
        {
            var img = iconImgTr.GetComponent<Image>();
            if (img)
            {
                img.sprite = entry.sprite;
                img.color = Color.white;
                img.preserveAspect = true;
            }
        }

        var btn = iconGO.GetComponentInChildren<Button>();
        if (btn)
        {
            btn.onClick.AddListener(() =>
            {
                var page = journal ? journal.CurrentPage : null;
                if (page != null)
                {
                    page.stickerPrefab = stickerItemPrefab;
                    page.AddSticker(entry.sprite, entry.id);
                }
            });
        }

        SafeStart(PopIn(iconGO.transform, 0.12f, 1.08f));
    }

    private void SafeStart(IEnumerator routine)
    {
        if (isActiveAndEnabled && routine != null)
            StartCoroutine(routine);
    }

    private IEnumerator PunchScale(Transform t, float duration = 0.18f, float strength = 1.12f)
    {
        if (!isActiveAndEnabled || !t) yield break;
        Vector3 baseS = t.localScale;
        float t0 = 0f;
        while (t0 < duration)
        {
            t0 += Time.unscaledDeltaTime;
            float u = t0 / duration;
            float k = Mathf.Sin(u * Mathf.PI);
            t.localScale = baseS * Mathf.Lerp(1f, strength, k);
            yield return null;
        }
        t.localScale = baseS;
    }

    private IEnumerator PopIn(Transform t, float duration, float peakScale)
    {
        if (!isActiveAndEnabled || !t) yield break;
        Vector3 baseS = Vector3.one;
        t.localScale = baseS * 0.85f;
        float t0 = 0f;
        while (t0 < duration)
        {
            t0 += Time.unscaledDeltaTime;
            float u = Mathf.Clamp01(t0 / duration);
            float s = (u < 0.6f)
                ? Mathf.Lerp(0.85f, peakScale, u / 0.6f)
                : Mathf.Lerp(peakScale, 1f, (u - 0.6f) / 0.4f);
            t.localScale = baseS * s;
            yield return null;
        }
        t.localScale = baseS;
    }
}
