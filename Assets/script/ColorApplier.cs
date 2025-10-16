using UnityEngine;
using System.Collections.Generic;

public class ColorApplier_ByGroups : MonoBehaviour
{
    [Header("A 组")]
    public List<GameObject> groupA = new List<GameObject>();

    [Header("B 组")]
    public List<GameObject> groupB = new List<GameObject>();

    [Header("C 组")]
    public List<GameObject> groupC = new List<GameObject>();

    void Start()
    {
        ApplyColors();
    }

    void ApplyColors()
    {
        List<string> colorValues = new List<string>(Data.colorRecords.Values);
        int count = colorValues.Count;

        if (count == 0)
        {
            return;
        }

        if (count == 3)
        {
            Debug.Log("嗯！这个是检测到3组的时候，因为这时候要对应耳朵 就足够三组吗");
            ApplyColorToGroupList(groupA, colorValues[0]);
            ApplyColorToGroupList(groupB, colorValues[1]);
            ApplyColorToGroupList(groupC, colorValues[2]);
        }
        else if (count == 2)
        {
            Debug.Log("这个是就是你不是兽人的时候，这个时候就对应剩下俩");
            ApplyColorToGroupList(groupB, colorValues[0]);
            ApplyColorToGroupList(groupC, colorValues[1]);
        }
        else
        {
            Debug.LogWarning($"色号没检测到出错了啊啊啊");
        }
    }

    void ApplyColorToGroupList(List<GameObject> groupList, string colorCode)
    {
        if (!ColorUtility.TryParseHtmlString(colorCode, out Color parsedColor))
        {
            return;
        }

        foreach (var obj in groupList)
        {
            if (obj == null) continue;

            SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var r in renderers)
            {
                r.color = parsedColor;
            }
        }
    }
}
