using UnityEngine;
using System.Collections.Generic;

public class CarColor : MonoBehaviour
{
    [SerializeField] List<Renderer> renderers = new List<Renderer>();
    [SerializeField] Color color = Color.red;
    [SerializeField] string colorProperty = "_BaseColor"; // Built-in이면 "_Color"로 바꾸세요

    MaterialPropertyBlock mpb;

    void Awake()
    {
        mpb = new MaterialPropertyBlock();
    }

    public void ApplyColor(Color c)
    {
        foreach (var r in renderers)
        {
            if (!r) continue;
            r.GetPropertyBlock(mpb);
            mpb.SetColor(colorProperty, c);
            r.SetPropertyBlock(mpb);
        }
        color = c;
    }

    // 에디터에서 미리보기 원하면 인스펙터의 color 값을 적용
    void OnValidate()
    {
        if (renderers == null || renderers.Count == 0) return;
        if (mpb == null) mpb = new MaterialPropertyBlock();
        ApplyColor(color);
    }
}
