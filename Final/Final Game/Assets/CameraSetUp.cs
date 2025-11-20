using UnityEngine;
using Unity.Cinemachine;
using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraSetup : MonoBehaviour
{
    public CinemachineTargetGroup TargetGroup;
    public CinemachineCamera MVCam; // 拖入 CM vcam1

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);

        // 1. 找到 P1 和 P2
        Character[] players = FindObjectsByType<Character>(FindObjectsSortMode.None);
        if (players.Length == 0) yield return new WaitForSeconds(0.2f);
        players = FindObjectsByType<Character>(FindObjectsSortMode.None);

        // 2. 填入 Target Group
        List<CinemachineTargetGroup.Target> newTargets = new List<CinemachineTargetGroup.Target>();
        foreach (var p in players)
        {
            var t = new CinemachineTargetGroup.Target();
            t.Object = p.transform;
            t.Weight = 1f;
            t.Radius = 3f;
            newTargets.Add(t);
        }

        // 赋值给 Target Group
        TargetGroup.Targets = newTargets;

        Debug.Log("双人摄像机配置完成！");
    }

    // --- 新增：核弹级修复 ---
    // LateUpdate 会在所有 TDE 的逻辑执行完之后运行
    // 无论 TDE 怎么把摄像机抢走，我们都在这里把它抢回来
    void LateUpdate()
    {
        if (MVCam != null && TargetGroup != null)
        {
            // 只有当摄像机没看 TargetGroup 时，才强行赋值
            if (MVCam.Follow != TargetGroup.transform)
            {
                MVCam.Follow = TargetGroup.transform;
                MVCam.LookAt = TargetGroup.transform;
            }
        }
    }
}