using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class AspectKeeper : MonoBehaviour
{
    public Vector2 targetAspect = new Vector2(16, 9);
    Camera cam;

    void OnEnable() { cam = GetComponent<Camera>(); Apply(); }
    void OnValidate() { Apply(); }
    void Update() { if (!Application.isPlaying) Apply(); }

    void Apply()
    {
        if (cam == null) return;
        float target = targetAspect.x / targetAspect.y;
        float window = (float)Screen.width / Screen.height;

        if (window > target)
        {
            float scale = target / window;
            float x = (1f - scale) * 0.5f;
            cam.rect = new Rect(x, 0f, scale, 1f);   // ×óÓÒºÚ±ß
        }
        else
        {
            float scale = window / target;
            float y = (1f - scale) * 0.5f;
            cam.rect = new Rect(0f, y, 1f, scale);   // ÉÏÏÂºÚ±ß
        }
    }
}
