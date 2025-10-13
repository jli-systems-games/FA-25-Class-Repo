using UnityEngine;

public class PixelRTScaler : MonoBehaviour
{
    public RenderTexture rt;
    public int baseWidth = 480;
    public int baseHeight = 270;

    void Start()
    {
        if (!rt) return;
        int scaleW = Mathf.Max(1, Screen.width / baseWidth);
        int scaleH = Mathf.Max(1, Screen.height / baseHeight);
        int scale = Mathf.Min(scaleW, scaleH);

        rt.filterMode = FilterMode.Point;
        rt.Release();
        rt.width = baseWidth;
        rt.height = baseHeight;
        rt.Create();

        // 可选：设置屏幕分辨率为整数倍（仅 PC 窗口有用）
        // Screen.SetResolution(baseWidth*scale, baseHeight*scale, FullScreenMode.Windowed);
    }
}
