using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RareHintUI : MonoBehaviour
{
    public KeyCode toggleKey = KeyCode.P;

    [Header("UI")]
    public CanvasGroup group;
    public TMP_Text titleText;
    public TMP_Text bodyText;
    public RawImage topViewImage;   // A
    public RawImage lowViewImage;   // B
    public RawImage sideViewImage;  // C

    [Header("3D Preview")]
    public int previewSize = 256;
    public Color previewBg = new Color(0.08f, 0.08f, 0.1f, 1f);
    public string previewLayerName = "UIModel";

    [Header("Isometric-ish (Perspective)")]
    public float isoElevationDeg = 35f;   // 抬头角
    public float isoDistanceMul = 2.2f;  // 距离 = size * 这个系数
    public float isoFov = 40f;   // 透视FOV
    public float yawA = 30f, yawB = 150f, yawC = 270f;

    Transform previewRoot;
    Camera camA, camB, camC;
    RenderTexture rtA, rtB, rtC;
    GameObject instA, instB, instC;
    int previewLayer;

    void Awake()
    {
        if (!group) group = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        SetVisible(true);

        previewLayer = LayerMask.NameToLayer(previewLayerName);
        if (previewLayer < 0) previewLayer = 0;

        var root = new GameObject("RarePreviewRoot");
        root.transform.position = new Vector3(0, -1000, 0);
        previewRoot = root.transform;

        camA = BuildCam("Cam_A");
        camB = BuildCam("Cam_B");
        camC = BuildCam("Cam_C");

        rtA = new RenderTexture(previewSize, previewSize, 16);
        rtB = new RenderTexture(previewSize, previewSize, 16);
        rtC = new RenderTexture(previewSize, previewSize, 16);
        camA.targetTexture = rtA; camB.targetTexture = rtB; camC.targetTexture = rtC;

        if (topViewImage) topViewImage.texture = rtA;
        if (lowViewImage) lowViewImage.texture = rtB;
        if (sideViewImage) sideViewImage.texture = rtC;
    }

    Camera BuildCam(string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(previewRoot, false);
        var cam = go.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = previewBg;
        cam.orthographic = false;
        cam.fieldOfView = isoFov;
        cam.cullingMask = (previewLayer == 0) ? ~0 : (1 << previewLayer);

        var lightGO = new GameObject(name + "_Light");
        lightGO.transform.SetParent(go.transform, false);
        var l = lightGO.AddComponent<Light>();
        l.type = LightType.Directional;
        l.intensity = 1.1f;

        return cam;
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            SetVisible(group.alpha < 0.5f);
    }

    public void SetVisible(bool on)
    {
        group.alpha = on ? 1f : 0f;
        group.interactable = on;
        group.blocksRaycasts = on;
    }

    public void UpdateHint()
    {
        GameObject rare = null;
        var planar = FindObjectOfType<PlanarNonsenseBuilder>();
        var onlyUp = FindObjectOfType<OnlyUpNonsenseBuilder>();
        if (planar && planar.currentRarePrefab) rare = planar.currentRarePrefab;
        else if (onlyUp && onlyUp.currentRarePrefab) rare = onlyUp.currentRarePrefab;

        if (titleText) titleText.text = "Objective";
        if (bodyText)
        {
            var name = rare ? rare.name : "the rare piece";
            bodyText.text =
                $"Find and photograph: <b>{name}</b>\n" +
                "Controls: WASD to move, Space to jump, Mouse to look\n" +
                "Hold <b>F</b> to take a photo, <b>E</b> to hide/show this hint, <b>R</b> to restart.\n" 
          ;

        }

        DestroyIfAny(instA); DestroyIfAny(instB); DestroyIfAny(instC);
        if (!rare) return;

        instA = Instantiate(rare, previewRoot);
        instB = Instantiate(rare, previewRoot);
        instC = Instantiate(rare, previewRoot);
        if (previewLayer != 0)
        {
            SetLayerRecursively(instA, previewLayer);
            SetLayerRecursively(instB, previewLayer);
            SetLayerRecursively(instC, previewLayer);
        }

        PoseIso(instA, instB, instC);
    }

    void PoseIso(GameObject a, GameObject b, GameObject c)
    {
        var r = a.GetComponentInChildren<Renderer>();
        if (!r) return;

        var bnds = r.bounds;
        var size = Mathf.Max(bnds.size.x, bnds.size.y, bnds.size.z);
        var center = bnds.center;
        var dist = Mathf.Max(0.1f, size * isoDistanceMul);

        camA.fieldOfView = isoFov;
        camB.fieldOfView = isoFov;
        camC.fieldOfView = isoFov;

        void Place(Camera cam, float yawDeg)
        {
            float yaw = yawDeg * Mathf.Deg2Rad;
            float elev = isoElevationDeg * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(
                Mathf.Cos(elev) * Mathf.Cos(yaw),
                Mathf.Sin(elev),
                Mathf.Cos(elev) * Mathf.Sin(yaw)
            );
            cam.transform.position = center + dir * dist;
            cam.transform.LookAt(center, Vector3.up);
        }

        Place(camA, yawA);
        Place(camB, yawB);
        Place(camC, yawC);
    }

    static void DestroyIfAny(GameObject go) { if (go) Object.Destroy(go); }
    static void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform ch in go.transform) SetLayerRecursively(ch.gameObject, layer);
    }
}
