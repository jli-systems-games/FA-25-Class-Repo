using System.Collections;
using UnityEngine;

public enum SpotAct
{
    Dance,
    Drink, Kick, Punch, Jump,
    SleepOn, SleepOff,
    SadOn, SadOff,
    InjuredOn, InjuredOff,
    DrunkOn, DrunkOff,
    Die, Revive,
    CustomTrigger
}

[AddComponentMenu("Pet/Trigger Spot ")]
public class PetTriggerSpot : MonoBehaviour
{
    [Header("Action on Enter")]
    public SpotAct action = SpotAct.Dance;

    [Tooltip("Dance 用到的索引（映射到 Animator 的 int 参数：DanceInd）")]
    public int danceIndex = 0;

    [Tooltip("CustomTrigger 时使用的 Trigger 名")]
    public string customTrigger = "YourTrigger";

    [Header("Optional Snap")]
    public Transform snapPoint;
    public bool alignToForward = true;

    [Header("Stage Mode Gate (可选)")]
    public bool setStageMode = true;
    public bool stageModeValue = true;     // 进入时是否置 true
    public float autoStageOffAfter = -1f;  // >0 则在若干秒后自动关掉 StageMode

    [Header("Safe Guard")]
    public float cooldown = 0.4f;          // 防抖
    public bool singleUse = false;         // 只触发一次

    float lastTime = -999f;
    bool used = false;

    void Reset()
    {
        // 确保有 Trigger Collider
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;

        // ☆ 关键：确保至少一方有 Rigidbody，这里给触发器自动加一个 Kinematic Rigidbody
        var rb = GetComponent<Rigidbody>();
        if (!rb)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (used) return;
        if (Time.time - lastTime < cooldown) return;

        // 找 Animator（优先从父级拿，适配子碰撞体）
        var anim = other.GetComponentInParent<Animator>();
        if (!anim) return;

        // 对齐站位（可选）
        if (snapPoint)
        {
            var t = anim.transform;
            t.position = snapPoint.position;
            if (alignToForward) t.rotation = Quaternion.LookRotation(snapPoint.forward, Vector3.up);
        }

        // 先开 StageMode（若需要）
        if (setStageMode) SafeSetBool(anim, "StageMode", stageModeValue);

        // 执行动作
        switch (action)
        {
            case SpotAct.Dance:
                SafeSetInt(anim, "DanceInd", danceIndex);
                break;

            case SpotAct.Drink: SafeTrigger(anim, "Drink"); break;
            case SpotAct.Kick: SafeTrigger(anim, "Kick"); break;
            case SpotAct.Punch: SafeTrigger(anim, "Punch"); break;
            case SpotAct.Jump: SafeTrigger(anim, "Jump"); break;

            case SpotAct.SleepOn: SafeSetBool(anim, "Sleep", true); break;
            case SpotAct.SleepOff: SafeSetBool(anim, "Sleep", false); break;

            case SpotAct.SadOn: SafeSetBool(anim, "Sad", true); break;
            case SpotAct.SadOff: SafeSetBool(anim, "Sad", false); break;

            case SpotAct.InjuredOn: SafeSetBool(anim, "IsInjured", true); break;
            case SpotAct.InjuredOff: SafeSetBool(anim, "IsInjured", false); break;

            case SpotAct.DrunkOn: SafeSetBool(anim, "IsDrunk", true); break;
            case SpotAct.DrunkOff: SafeSetBool(anim, "IsDrunk", false); break;

            case SpotAct.Die: SafeTrigger(anim, "Die"); break;
            case SpotAct.Revive: SafeTrigger(anim, "Revive"); break;

            case SpotAct.CustomTrigger:
                if (!string.IsNullOrEmpty(customTrigger))
                    SafeTrigger(anim, customTrigger);
                break;
        }

        // 定时把 StageMode 关掉（可选）
        if (setStageMode && autoStageOffAfter > 0f)
            StartCoroutine(AutoOff(anim, autoStageOffAfter));

        lastTime = Time.time;
        if (singleUse) used = true;
    }

    IEnumerator AutoOff(Animator anim, float t)
    {
        yield return new WaitForSeconds(t);
        SafeSetBool(anim, "StageMode", false);
    }

    // --- 安全设置（只有存在该参数才设置，避免报错） ---
    static void SafeTrigger(Animator anim, string name)
    {
        foreach (var p in anim.parameters)
            if (p.type == AnimatorControllerParameterType.Trigger && p.name == name)
            { anim.ResetTrigger(name); anim.SetTrigger(name); return; }
    }
    static void SafeSetBool(Animator anim, string name, bool v)
    {
        foreach (var p in anim.parameters)
            if (p.type == AnimatorControllerParameterType.Bool && p.name == name)
            { anim.SetBool(name, v); return; }
    }
    static void SafeSetInt(Animator anim, string name, int v)
    {
        foreach (var p in anim.parameters)
            if (p.type == AnimatorControllerParameterType.Int && p.name == name)
            { anim.SetInteger(name, v); return; }
    }

    void OnDrawGizmosSelected()
    {
        var col = GetComponent<Collider>();
        if (col)
        {
            Gizmos.color = new Color(1, .6f, .2f, .25f);
            var b = col.bounds;
            Gizmos.DrawCube(b.center, b.size);
        }
        if (snapPoint)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(snapPoint.position, 0.08f);
            Gizmos.DrawLine(snapPoint.position, snapPoint.position + snapPoint.forward * 0.35f);
        }
    }
}
