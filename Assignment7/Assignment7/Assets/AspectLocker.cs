using UnityEngine;

[RequireComponent(typeof(Camera))]
public class AspectLocker : MonoBehaviour
{
    public float targetAspect = 16f / 9f;
    public bool keepWidth = true;
    Camera cam; float lw, lh;
    void Awake() { cam = GetComponent<Camera>(); Apply(); }
    void Update() { if (lw != Screen.width || lh != Screen.height) Apply(); }
    void Apply()
    {
        lw = Screen.width; lh = Screen.height;
        float w = (float)Screen.width / Screen.height;
        if (keepWidth)
        {
            float s = w / targetAspect;
            if (s < 1f) { float h = s; float p = (1f - h) * .5f; cam.rect = new Rect(0f, p, 1f, h); }
            else { float rw = 1f / s; float p = (1f - rw) * .5f; cam.rect = new Rect(p, 0f, rw, 1f); }
        }
        else
        {
            float s = targetAspect / w;
            if (s < 1f) { float rw = s; float p = (1f - rw) * .5f; cam.rect = new Rect(p, 0f, rw, 1f); }
            else { float h = 1f / s; float p = (1f - h) * .5f; cam.rect = new Rect(0f, p, 1f, h); }
        }
    }
}
