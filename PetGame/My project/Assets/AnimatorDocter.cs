using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

public class AnimatorDoctor : MonoBehaviour
{
    public Animator anim;
    public KeyCode toggleKey = KeyCode.F9;
    bool show = true;

    string cur, next;
    readonly List<string> lines = new List<string>();
    float lastUpdate;
    readonly Dictionary<string, float> fired = new Dictionary<string, float>();

    void Awake() { if (!anim) anim = GetComponentInChildren<Animator>(); }
    void Start() { AuditAnimator(); }

    void Update()
    {
        var s0 = anim.GetCurrentAnimatorStateInfo(0);
        cur = s0.shortNameHash.ToString();
        next = "";
        if (anim.IsInTransition(0))
        {
            var t = anim.GetAnimatorTransitionInfo(0);
            next = "-> " + t.userNameHash.ToString();
        }

        if (Input.GetKeyDown(toggleKey)) show = !show;

        if (Time.time - lastUpdate > 0.2f)
        {
            lastUpdate = Time.time;
            lines.Clear();
            lines.Add("Layer0: " + StateName(0, s0));
            if (anim.IsInTransition(0)) lines.Add("Transitioning...");
            lines.Add("Speed=" + anim.GetFloat("Speed").ToString("0.00"));

            AppendBool("IsDrunk");
            AppendBool("IsInjured");
            AppendBool("Sleep");
            AppendBool("Sad");
            AppendBool("StageMode");
            AppendInt("DanceInd");

            Touch("Jump");
            Touch("Drink");
            Touch("Kick");
            Touch("Punch");
            Touch("Die");
            Touch("Revive");
        }
    }

    void AppendBool(string n) { if (HasParam(n, AnimatorControllerParameterType.Bool)) lines.Add(n + "=" + anim.GetBool(n)); }
    void AppendInt(string n) { if (HasParam(n, AnimatorControllerParameterType.Int)) lines.Add(n + "=" + anim.GetInteger(n)); }

    void Touch(string trig)
    {
        if (!HasParam(trig, AnimatorControllerParameterType.Trigger)) return;
        if (Input.GetKeyDown(KeyCode.None)) { }
        if (!fired.ContainsKey(trig)) fired[trig] = -999f;
        if (Input.GetKeyDown(KeyCode.None)) { }
    }

    bool HasParam(string name, AnimatorControllerParameterType t)
    {
        foreach (var p in anim.parameters) if (p.name == name && p.type == t) return true;
        return false;
    }

    string StateName(int layer, AnimatorStateInfo s)
    {
#if UNITY_EDITOR
        var ac = anim.runtimeAnimatorController as AnimatorController;
        if (ac != null)
        {
            foreach (var st in ac.layers[layer].stateMachine.states)
                if (s.shortNameHash == st.state.nameHash) return st.state.name;
        }
#endif
        return s.shortNameHash.ToString();
    }

#if UNITY_EDITOR
    void AuditAnimator()
    {
        var ac = anim.runtimeAnimatorController as AnimatorController;
        if (ac == null) { Debug.LogWarning("[AnimatorDoctor] Not an AnimatorController, skip audit."); return; }

        var sm = ac.layers[0].stateMachine;

        Debug.Log("=== [AnimatorDoctor] Any State Transitions ===");
        foreach (var tr in sm.anyStateTransitions)
        {
            var msg = $"Any -> {tr.destinationState?.name ?? "(subSM)"}  HET:{tr.hasExitTime}  Dur:{tr.duration:0.00}";
            foreach (var c in tr.conditions) msg += $" | {c.parameter} {c.mode} {c.threshold}";
            Debug.Log(msg);
        }

        CheckReturn("Crew_Jump", sm);
        CheckReturn("Crew_Drink", sm);
        CheckReturn("Crew_MmaKick", sm);
        CheckReturn("Crew_JabCross", sm);
        CheckReturnBool("Crew_SleepLay", sm, "Sleep", false);
        CheckReturnBool("Crew_DrunkWalk", sm, "IsDrunk", false);
        CheckReturnBool("Crew_InjuredWalk", sm, "IsInjured", false);
        CheckReturnBool("Crew_SadIdle", sm, "Sad", false);

        bool hasRevive = false;
        foreach (var tr in sm.anyStateTransitions)
            if (tr.destinationState != null && tr.destinationState.name == "Crew_CrouchToStand")
                foreach (var c in tr.conditions) if (c.parameter == "Revive") hasRevive = true;
        if (!hasRevive) Debug.LogWarning("[AnimatorDoctor] Missing Any->Crew_CrouchToStand by Revive.");

        var cs = FindState(sm, "Crew_CrouchToStand");
        if (cs != null)
        {
            bool back = false;
            foreach (var t in cs.transitions)
                if (t.destinationState != null && t.destinationState.name == "Locomotion") back = true;
            if (!back) Debug.LogWarning("[AnimatorDoctor] Crew_CrouchToStand should go back to Locomotion with ExitTime~0.95.");
        }

        Debug.Log("=== [AnimatorDoctor] Audit End ===");
    }

    AnimatorState FindState(AnimatorStateMachine sm, string name)
    {
        foreach (var s in sm.states) if (s.state.name == name) return s.state; return null;
    }

    void CheckReturn(string stateName, AnimatorStateMachine sm)
    {
        var s = FindState(sm, stateName);
        if (s == null) return;
        bool back = false;
        foreach (var t in s.transitions)
            if (t.destinationState != null && t.destinationState.name == "Locomotion") back = true;
        if (!back) Debug.LogWarning($"[AnimatorDoctor] {stateName} should return to Locomotion (HasExitTime ON, Dur~0.05).");
    }

    void CheckReturnBool(string stateName, AnimatorStateMachine sm, string boolName, bool toValue)
    {
        var s = FindState(sm, stateName);
        if (s == null) return;
        bool ok = false;
        foreach (var t in s.transitions)
        {
            bool toLoc = t.destinationState != null && t.destinationState.name == "Locomotion";
            bool cond = false;
            foreach (var c in t.conditions)
                if (c.parameter == boolName && c.mode == AnimatorConditionMode.If && toValue == true) cond = true;
                else if (c.parameter == boolName && c.mode == AnimatorConditionMode.IfNot && toValue == false) cond = true;
            if (toLoc && cond) ok = true;
        }
        if (!ok) Debug.LogWarning($"[AnimatorDoctor] {stateName} should return to Locomotion when {boolName}=={toValue}.");
    }
#else
    void AuditAnimator(){}
#endif

    void OnGUI()
    {
        if (!show) return;
        var r = new Rect(10, 10, 380, 300);
        GUI.Box(r, "AnimatorDoctor");
        GUILayout.BeginArea(new Rect(r.x + 10, r.y + 25, r.width - 20, r.height - 35));
        GUILayout.Label("State: " + cur + " " + next);
        foreach (var s in lines) GUILayout.Label(s);
        GUILayout.Label("Triggers: Jump/Drink/Kick/Punch/Die/Revive use AnimDebugKeys to fire if needed.");
        GUILayout.EndArea();
    }
}
