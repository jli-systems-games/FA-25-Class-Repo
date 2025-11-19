using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FixAspectRatio : MonoBehaviour
{
    // 目标宽高比，例如 16:9
    public Vector2 targetAspect = new Vector2(16f, 9f);

    void Start()
    {
        ApplyAspect();
    }

    void OnValidate()
    {
        // 在编辑器调参数时也能实时更新（游戏运行中才有效）
        if (Application.isPlaying)
        {
            ApplyAspect();
        }
    }

    void ApplyAspect()
    {
        Camera cam = GetComponent<Camera>();

        float target = targetAspect.x / targetAspect.y;
        float window = (float)Screen.width / Screen.height;

        // 和目标几乎一样，就直接铺满
        if (Mathf.Approximately(target, window))
        {
            cam.rect = new Rect(0f, 0f, 1f, 1f);
            return;
        }

        // 当前屏幕比目标更宽 → 左右黑边
        if (window > target)
        {
            float normalizedWidth = target / window;
            float x = (1f - normalizedWidth) * 0.5f;
            cam.rect = new Rect(x, 0f, normalizedWidth, 1f);
        }
        // 当前屏幕比目标更窄/更高 → 上下黑边
        else
        {
            float normalizedHeight = window / target;
            float y = (1f - normalizedHeight) * 0.5f;
            cam.rect = new Rect(0f, y, 1f, normalizedHeight);
        }
    }
}
