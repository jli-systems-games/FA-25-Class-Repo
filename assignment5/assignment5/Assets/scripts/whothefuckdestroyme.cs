using UnityEngine;
using SD = System.Diagnostics;

public class LifeGuard : MonoBehaviour
{
    void OnDisable()
    {
        UnityEngine.Debug.LogWarning("[LifeGuard] DISABLED\n" + new SD.StackTrace(true));
    }
    void OnDestroy()
    {
        UnityEngine.Debug.LogError("[LifeGuard] DESTROYED\n" + new SD.StackTrace(true));
    }
}
