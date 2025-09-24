using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class DrumAutoHeuristic : MonoBehaviour
{
    [Header("可选：手动覆盖（留空就自动找）")]
    public Transform leftDrum;    // Q
    public Transform centerDrum;  // W
    public Transform rightDrum;   // E
    public Transform fxDrum;      // R
    public Transform tDrum;       // T（新增：额外一个鼓/效果件）

    [Header("声音（两种其一即可）")]
    public AudioSource srcQ, srcW, srcE, srcR, srcT;    // 独立声道
    public AudioSource sharedSource;
    public AudioClip qClip, wClip, eClip, rClip, tClip; // 或单声道多Clip
    public float volume = 1f;

    [Header("时序")]
    public float firstDelay = 0.5f;   // 第一次延迟
    public float repeatInterval = 1f; // 长按循环

    [Header("按下动效")]
    public float pressDepth = 0.04f;
    public float pressRotate = 6f;
    public float returnSpeed = 10f;
    public Vector3 pressLocalDir = new Vector3(0, -1, 0);

    DesertDriverAuto driver;

    struct Piece { public Transform t; public Vector3 restPos; public Quaternion restRot; }
    Piece L, C, R, FX, T;

    // 按键状态
    bool holdQ, holdW, holdE, holdR, holdT;
    float tQ = -1f, tW = -1f, tE = -1f, tR = -1f, tT = -1f;

    void Awake()
    {
#if UNITY_2023_1_OR_NEWER
        driver = FindAnyObjectByType<DesertDriverAuto>();
#else
        driver = FindObjectOfType<DesertDriverAuto>();
#endif

        if (!leftDrum || !centerDrum || !rightDrum || !fxDrum)
            AutoPick();

        if (leftDrum) { L.t = leftDrum; L.restPos = leftDrum.localPosition; L.restRot = leftDrum.localRotation; }
        if (centerDrum) { C.t = centerDrum; C.restPos = centerDrum.localPosition; C.restRot = centerDrum.localRotation; }
        if (rightDrum) { R.t = rightDrum; R.restPos = rightDrum.localPosition; R.restRot = rightDrum.localRotation; }
        if (fxDrum) { FX.t = fxDrum; FX.restPos = fxDrum.localPosition; FX.restRot = fxDrum.localRotation; }
        if (tDrum) { T.t = tDrum; T.restPos = tDrum.localPosition; T.restRot = tDrum.localRotation; }
    }

    void Update()
    {
        // Q
        if (Input.GetKeyDown(KeyCode.Q)) { holdQ = true; tQ = firstDelay; DoPressVisual(ref L); }
        if (Input.GetKeyUp(KeyCode.Q)) { holdQ = false; tQ = -1f; }

        // W
        if (Input.GetKeyDown(KeyCode.W)) { holdW = true; tW = firstDelay; DoPressVisual(ref C); }
        if (Input.GetKeyUp(KeyCode.W)) { holdW = false; tW = -1f; }

        // E
        if (Input.GetKeyDown(KeyCode.E)) { holdE = true; tE = firstDelay; DoPressVisual(ref R); }
        if (Input.GetKeyUp(KeyCode.E)) { holdE = false; tE = -1f; }

        // R
        if (Input.GetKeyDown(KeyCode.R)) { holdR = true; tR = firstDelay; DoPressVisual(ref FX); }
        if (Input.GetKeyUp(KeyCode.R)) { holdR = false; tR = -1f; }

        // T（新增）
        if (Input.GetKeyDown(KeyCode.T)) { holdT = true; tT = firstDelay; DoPressVisual(ref T); }
        if (Input.GetKeyUp(KeyCode.T)) { holdT = false; tT = -1f; }

        // 计时触发
        TickKey(ref holdQ, ref tQ, ref L, srcQ, qClip, driver != null ? (System.Action)(() => driver.TapLeft()) : null);
        TickKey(ref holdW, ref tW, ref C, srcW, wClip, driver != null ? (System.Action)(() => driver.TapCenter()) : null);
        TickKey(ref holdE, ref tE, ref R, srcE, eClip, driver != null ? (System.Action)(() => driver.TapRight()) : null);
        TickKey(ref holdR, ref tR, ref FX, srcR, rClip, driver != null ? (System.Action)(() => driver.TapFX()) : null);
        TickKey(ref holdT, ref tT, ref T, srcT, tClip, driver != null ? (System.Action)(() => Debug.Log("T pressed")) : null);

        // 回弹
        Return(ref L); Return(ref C); Return(ref R); Return(ref FX); Return(ref T);
    }

    void TickKey(ref bool holding, ref float timer, ref Piece p, AudioSource src, AudioClip clip, System.Action after)
    {
        if (!holding || timer < 0f) return;
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            PlaySound(src, clip);
            DoPressVisual(ref p);
            after?.Invoke();
            timer = repeatInterval;
        }
    }

    void AutoPick()
    {
        var all = GetComponentsInChildren<MeshRenderer>(true).Select(m => m.transform).Distinct().ToList();
        if (all.Count == 0) { Debug.LogWarning("DrumAutoHeuristic: 鼓下面没找到MeshRenderer"); return; }

        var localPositions = all.ToDictionary(
            t => t,
            t => transform.InverseTransformPoint(t.position)
        );

        Transform left = localPositions.OrderBy(p => p.Value.x).First().Key;
        Transform right = localPositions.OrderByDescending(p => p.Value.x).First().Key;
        Transform top = localPositions.OrderByDescending(p => p.Value.y).First().Key;
        Transform center = localPositions.OrderBy(p => Mathf.Abs(p.Value.x)).First().Key;

        if (center == left) center = localPositions.Where(p => p.Key != left).OrderBy(p => Mathf.Abs(p.Value.x)).First().Key;
        if (center == right) center = localPositions.Where(p => p.Key != right).OrderBy(p => Mathf.Abs(p.Value.x)).First().Key;
        if (top == left || top == right || top == center)
            top = localPositions.Where(p => p.Key != left && p.Key != right && p.Key != center)
                                .OrderByDescending(p => p.Value.y).FirstOrDefault().Key ?? top;

        if (!leftDrum) leftDrum = left;
        if (!rightDrum) rightDrum = right;
        if (!centerDrum) centerDrum = center;
        if (!fxDrum) fxDrum = top;
        // T 没有自动挑，建议手动指定
    }

    void PlaySound(AudioSource src, AudioClip clip)
    {
        if (src) { src.Play(); return; }
        if (sharedSource && clip) { sharedSource.PlayOneShot(clip, volume); return; }
    }

    void DoPressVisual(ref Piece p)
    {
        if (!p.t) return;
        Vector3 dir = pressLocalDir.sqrMagnitude < 1e-5f ? Vector3.down : pressLocalDir.normalized;
        p.t.localPosition += dir * pressDepth;
        p.t.localRotation = p.t.localRotation * Quaternion.Euler(pressRotate, 0f, 0f);
    }

    void Return(ref Piece p)
    {
        if (!p.t) return;
        p.t.localPosition = Vector3.Lerp(p.t.localPosition, p.restPos, Time.deltaTime * returnSpeed);
        p.t.localRotation = Quaternion.Slerp(p.t.localRotation, p.restRot, Time.deltaTime * returnSpeed);
    }
}
