using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering; // GraphicsFormat

[ExecuteAlways]
public class PixelatedRenderTexture : MonoBehaviour
{
    [Header("Refs")]
    public Camera sourceCamera;
    public RawImage targetImage;

    [Header("Pixelation")]
    [Range(1, 16)] public int downscale = 4;
    public bool matchWindowEveryFrame = true;

    [Header("Color/Filtering")]
    public bool useSRGB = true;  
    public bool useMipMaps = false;

    private RenderTexture rt;
    private Vector2Int lastSize;

    void OnEnable()
    {
        if (!sourceCamera) sourceCamera = Camera.main;
        BuildRT();
        ApplyToTargets();
        FitRawImageFullScreen();
    }

    void OnDisable()
    {
        Cleanup();
    }

    void Update()
    {
        if (!matchWindowEveryFrame) return;
        var now = new Vector2Int(Screen.width, Screen.height);
        if (now != lastSize || !rt || rt.width <= 0 || rt.height <= 0)
        {
            BuildRT();
            ApplyToTargets();
            FitRawImageFullScreen();
        }
    }

    void BuildRT()
    {
        Cleanup();

        int w = Mathf.Max(1, Screen.width / Mathf.Max(1, downscale));
        int h = Mathf.Max(1, Screen.height / Mathf.Max(1, downscale));

        var desc = new RenderTextureDescriptor(w, h);
        desc.depthBufferBits = 0;
        desc.msaaSamples = 1;                 
        desc.useMipMap = useMipMaps;
        desc.autoGenerateMips = false;
        
        desc.graphicsFormat = useSRGB
            ? GraphicsFormat.R8G8B8A8_SRGB
            : GraphicsFormat.R8G8B8A8_UNorm;

        rt = new RenderTexture(desc);
        rt.filterMode = FilterMode.Point;    
        rt.wrapMode = TextureWrapMode.Clamp;
        rt.name = $"RT_Pixel_{w}x{h}";

        lastSize = new Vector2Int(Screen.width, Screen.height);
    }

    void ApplyToTargets()
    {
        if (sourceCamera) sourceCamera.targetTexture = rt;
        if (targetImage) targetImage.texture = rt;
    }

    void FitRawImageFullScreen()
    {
        if (!targetImage) return;
        var t = targetImage.rectTransform;
        t.anchorMin = Vector2.zero;
        t.anchorMax = Vector2.one;
        t.offsetMin = Vector2.zero;
        t.offsetMax = Vector2.zero;

        if (targetImage.texture) targetImage.texture.filterMode = FilterMode.Point;
        targetImage.material = null; 
    }

    void Cleanup()
    {
        if (sourceCamera && sourceCamera.targetTexture == rt) sourceCamera.targetTexture = null;
        if (targetImage && targetImage.texture == rt) targetImage.texture = null;
        if (rt)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) DestroyImmediate(rt);
            else Destroy(rt);
#else
            Destroy(rt);
#endif
            rt = null;
        }
    }
}
