using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class RunGameFlow : MonoBehaviour
{
    public KeyCode driveKey = KeyCode.Space;
    public float cameraMargin = 0.6f;
    public float groundYPercent = -0.25f;
    public float vehicleScale = 0.5f;
    public float rightBounce = 0.65f;
    public float rightFriction = 0.05f;
    public float minSpeed = 180f;
    public float maxSpeed = 1200f;
    public float accelPerSecond = 800f;
    public float releaseDecayPerSecond = 1000f;
    public float spawnDropHeight = 1.2f;
    public float sfxInterval = 0.7f;
    public float directionFlipPeriod = 5f;
    public TMP_FontAsset boldFont;

    TMP_Text distText, hintText;
    VehicleController2D vehicle;
    Rigidbody2D playerRb;

    float xMin, xMax, yMin, yMax, groundY, startX;
    float currentSpeedMag;
    PhysicsMaterial2D groundMat;
    bool clockwise;
    float[] wheelSfxTimers;
    float flipTimer;

    void Awake()
    {
        AudioHub.Ensure();
        SetupWorldFromCamera();
        BuildUI();
        BuildTrack();
        SpawnVehicle();
        startX = playerRb.position.x;
        clockwise = false;
        flipTimer = 0f;
        wheelSfxTimers = new float[vehicle.hinges.Count];
    }

    void SetupWorldFromCamera()
    {
        var cam = Camera.main;
        if (!cam)
        {
            var go = new GameObject("Main Camera", typeof(Camera));
            cam = go.GetComponent<Camera>();
        }
        cam.orthographic = true;
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
        Vector3 c = cam.transform.position;
        xMin = c.x - halfW + cameraMargin;
        xMax = c.x + halfW - cameraMargin;
        yMin = c.y - halfH + cameraMargin;
        yMax = c.y + halfH + cameraMargin;
        groundY = Mathf.Lerp(yMin, yMax, Mathf.Clamp01(0.5f + groundYPercent));
        var bg = GameObject.Find("Background");
        if (bg == null) bg = new GameObject("Background", typeof(SpriteRenderer));
        var sr = bg.GetComponent<SpriteRenderer>();
        if (!sr.sprite)
        {
            var tex = Texture2D.whiteTexture;
            sr.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
            sr.color = new Color(0.35f, 0.55f, 0.9f, 1f);
        }
        bg.transform.position = new Vector3(c.x, c.y, 10f);
        float w = halfW * 2f; float h = halfH * 2f;
        Vector2 sp = sr.sprite.bounds.size;
        float s = Mathf.Max(w / sp.x, h / sp.y);
        bg.transform.localScale = new Vector3(s, s, 1f);
        sr.sortingOrder = -100;
        groundMat = new PhysicsMaterial2D("GroundMat");
        groundMat.friction = 0.6f;
        groundMat.bounciness = 0f;
    }

    void ApplyTextStyle(TMP_Text t, int size, TextAlignmentOptions align)
    {
        t.font = boldFont ? boldFont : t.font;
        t.fontSize = size;
        t.enableAutoSizing = true;
        t.color = Color.black;
        t.alignment = align;
    }

    void BuildUI()
    {
        var canvas = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var c = canvas.GetComponent<Canvas>(); c.renderMode = RenderMode.ScreenSpaceOverlay;
        var s = canvas.GetComponent<CanvasScaler>(); s.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; s.referenceResolution = new Vector2(1920, 1080); s.matchWidthOrHeight = 0.5f;
        distText = MakeText(canvas.transform, new Vector2(0.5f, 1f), new Vector2(0f, -80), "0.0m", 36, TextAlignmentOptions.Center);
        hintText = MakeText(canvas.transform, new Vector2(0.5f, 0f), new Vector2(0f, 30), "SPACE accelerate | R = Back to SampleScene | Auto flip 5s", 28, TextAlignmentOptions.Center);
    }

    TMP_Text MakeText(Transform parent, Vector2 anchor, Vector2 offset, string text, int size, TextAlignmentOptions align)
    {
        var go = new GameObject("Text", typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>(); rt.anchorMin = anchor; rt.anchorMax = anchor; rt.pivot = anchor; rt.anchoredPosition = offset;
        var t = go.GetComponent<TextMeshProUGUI>(); t.text = text;
        ApplyTextStyle(t, size, align);
        return t;
    }

    void BuildTrack()
    {
        var ground = new GameObject("Ground", typeof(EdgeCollider2D));
        var gc = ground.GetComponent<EdgeCollider2D>();
        gc.points = new Vector2[] { new Vector2(xMin, groundY), new Vector2(xMax, groundY) };
        gc.sharedMaterial = groundMat;

        var leftWall = new GameObject("LeftWall", typeof(EdgeCollider2D));
        leftWall.GetComponent<EdgeCollider2D>().points = new Vector2[] { new Vector2(xMin, yMin), new Vector2(xMin, yMax) };

        var rightWall = new GameObject("RightWall", typeof(EdgeCollider2D));
        var rc = rightWall.GetComponent<EdgeCollider2D>();
        rc.points = new Vector2[] { new Vector2(xMax, yMin), new Vector2(xMax, yMax) };
        rc.sharedMaterial = MakeMat(rightFriction, Mathf.Clamp01(rightBounce));

        var top = new GameObject("TopBound", typeof(EdgeCollider2D));
        top.GetComponent<EdgeCollider2D>().points = new Vector2[] { new Vector2(xMin, yMax), new Vector2(xMax, yMax) };

        var bottom = new GameObject("BottomBound", typeof(EdgeCollider2D));
        bottom.GetComponent<EdgeCollider2D>().points = new Vector2[] { new Vector2(xMin, yMin), new Vector2(xMax, yMin) };
    }

    PhysicsMaterial2D MakeMat(float friction, float bounce)
    {
        var m = new PhysicsMaterial2D("RightWallMat");
        m.friction = Mathf.Clamp01(friction);
        m.bounciness = Mathf.Clamp01(bounce);
        return m;
    }

    void SpawnVehicle()
    {
        var root = new GameObject("Vehicle");
        vehicle = root.AddComponent<VehicleController2D>();
        vehicle.globalScale = Mathf.Clamp(vehicleScale, 0.2f, 1.0f);
        vehicle.driveMaxTorque = 1800f;
        Vector2 start = new Vector2(Mathf.Lerp(xMin, xMax, 0.15f), groundY + spawnDropHeight);
        vehicle.BuildFromData(start);
        playerRb = vehicle.bodyRb;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("SampleScene");
            return;
        }

        bool holding = Input.GetKey(driveKey);
        if (holding)
        {
            if (currentSpeedMag < minSpeed) currentSpeedMag = minSpeed;
            currentSpeedMag += accelPerSecond * Time.deltaTime;
            currentSpeedMag = Mathf.Clamp(currentSpeedMag, 0f, maxSpeed);
            vehicle.SetMotor(true);
        }
        else
        {
            currentSpeedMag = Mathf.MoveTowards(currentSpeedMag, 0f, releaseDecayPerSecond * Time.deltaTime);
            vehicle.SetMotor(currentSpeedMag > 0.01f);
        }

        flipTimer += Time.deltaTime;
        if (flipTimer >= Mathf.Max(0.05f, directionFlipPeriod))
        {
            clockwise = !clockwise;
            flipTimer = 0f;
        }

        if (clockwise) vehicle.SetMotorSpeedCW(currentSpeedMag); else vehicle.SetMotorSpeedCCW(currentSpeedMag);

        if (holding) TickWheelSfx(Time.deltaTime);

        float dist = Mathf.Max(0f, playerRb.position.x - startX);
        
    }

    void TickWheelSfx(float dt)
    {
        if (wheelSfxTimers == null || wheelSfxTimers.Length != vehicle.hinges.Count) wheelSfxTimers = new float[vehicle.hinges.Count];
        for (int i = 0; i < vehicle.hinges.Count; i++)
        {
            if (!vehicle.driveMask[i]) continue;
            wheelSfxTimers[i] += dt;
            if (wheelSfxTimers[i] >= sfxInterval)
            {
                wheelSfxTimers[i] = 0f;
                int idx = Mathf.Clamp(vehicle.wheelItemIndices[i], 0, Data.lib.wheelItems.Count - 1);
                var clip = Data.lib.wheelItems[idx].runClip ? Data.lib.wheelItems[idx].runClip : Data.lib.wheelItems[idx].selectClip;
                if (clip) AudioHub.Play2D(clip, 0.9f);
            }
        }
    }
}
