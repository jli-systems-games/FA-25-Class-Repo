using System.Collections;
using UnityEngine;

public class SimpleDrumDrive : MonoBehaviour
{
    [Header("Refs")]
    public Transform carRoot;          // 车辆父物体（包含 Main Camera/Drumset）——拖 CarRig
    public Camera cam;                 // 主相机 —— 拖 Main Camera

    [Header("World Mode (可选)")]
    public bool moveWorldInstead = false; // 勾上 = 车辆不动，WorldRoot 反向移动
    public Transform worldRoot;           // 勾上时，拖你的 WorldRoot

    [Header("Move Parameters")]
    public float delayBeforeMove = 0.5f;  // 先出声，延迟再动
    public float moveDistance = 2f;       // 每次推进的距离（米）
    public float moveTime = 0.35f;        // 推动持续时间（秒）
    public AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1); // 平滑曲线

    [Header("Audio（两种其一即可）")]
    public AudioSource qSrc, wSrc, eSrc, rSrc;        // 四个独立 AudioSource
    public AudioSource sharedSrc;                     // 或一个 Source + 四个 Clip
    public AudioClip qClip, wClip, eClip, rClip;

    void Reset()
    {
        cam = Camera.main;
        if (cam) carRoot = cam.transform.root;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) Trigger(Vector2.left);   // 左
        if (Input.GetKeyDown(KeyCode.W)) Trigger(Vector2.up);     // 前
        if (Input.GetKeyDown(KeyCode.E)) Trigger(Vector2.right);  // 右
        if (Input.GetKeyDown(KeyCode.R)) Trigger(Vector2.down);   // 后
    }

    void Trigger(Vector2 dir2D)
    {
        // 1) 先发声
        PlaySound(dir2D);
        // 2) 0.5s 后推进
        StartCoroutine(MoveAfterDelay(dir2D));
    }

    IEnumerator MoveAfterDelay(Vector2 dir2D)
    {
        yield return new WaitForSeconds(delayBeforeMove);

        if (!cam) yield break;

        // 基于相机的水平前/右向
        Vector3 fwd = cam.transform.forward; fwd.y = 0; fwd.Normalize();
        Vector3 right = cam.transform.right; right.y = 0; right.Normalize();

        Vector3 dir3D = (dir2D.y * fwd + dir2D.x * right).normalized;
        Vector3 delta = dir3D * moveDistance;

        // 选移动目标：车 or 世界
        bool worldMode = moveWorldInstead;
        Transform target = worldMode ? worldRoot : carRoot;
        if (!target) yield break;

        Vector3 start = target.position;
        Vector3 end = start + (worldMode ? -delta : delta);

        float t = 0f;
        while (t < moveTime)
        {
            t += Time.deltaTime;
            float k = ease.Evaluate(Mathf.Clamp01(t / moveTime));
            target.position = Vector3.LerpUnclamped(start, end, k);
            yield return null;
        }
        target.position = end;
    }

    void PlaySound(Vector2 dir2D)
    {
        // 左(Q)、前(W)、右(E)、后(R)
        if (dir2D == Vector2.left) { if (qSrc) qSrc.Play(); else if (sharedSrc && qClip) { sharedSrc.clip = qClip; sharedSrc.Play(); } }
        if (dir2D == Vector2.up) { if (wSrc) wSrc.Play(); else if (sharedSrc && wClip) { sharedSrc.clip = wClip; sharedSrc.Play(); } }
        if (dir2D == Vector2.right) { if (eSrc) eSrc.Play(); else if (sharedSrc && eClip) { sharedSrc.clip = eClip; sharedSrc.Play(); } }
        if (dir2D == Vector2.down) { if (rSrc) rSrc.Play(); else if (sharedSrc && rClip) { sharedSrc.clip = rClip; sharedSrc.Play(); } }
    }
}
