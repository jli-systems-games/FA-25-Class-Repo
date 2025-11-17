using UnityEngine;

public enum CellOwner { None, A, B }

public class ClaimOverlay : MonoBehaviour
{
    public Color colorHidden = new Color(0.5f, 0.5f, 0.5f, 0.35f);
    public Color colorA = new Color(1.0f, 0.25f, 0.25f, 0.9f);
    public Color colorB = new Color(0.25f, 0.55f, 1.0f, 0.9f);

    MeshRenderer _mr;
    Material _material;

    void Awake()
    {
        if (GetComponent<MeshFilter>() == null) gameObject.AddComponent<MeshFilter>();
        _mr = GetComponent<MeshRenderer>();
        if (_mr == null) _mr = gameObject.AddComponent<MeshRenderer>();

        var shader = Shader.Find("Universal Render Pipeline/Lit");
        _material = new Material(shader);
        _material.SetFloat("_Surface", 1);
        _material.SetFloat("_ZWrite", 0);
        _material.SetOverrideTag("RenderType", "Transparent");
        _material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        if (_material.HasProperty("_Cull")) _material.SetFloat("_Cull", 0);
        
        _mr.material = _material;
        SetOwner(CellOwner.None);
    }

    public void SetOwner(CellOwner owner)
    {
        Color c = colorHidden;
        switch (owner)
        {
            case CellOwner.A:         c = colorA; break;
            case CellOwner.B:         c = colorB; break;
            case CellOwner.None:      c = colorHidden; break;
        }

        if (_material.HasProperty("_BaseColor"))
            _material.SetColor("_BaseColor", c);
        else if (_material.HasProperty("_Color"))
            _material.SetColor("_Color", c);
    }
}