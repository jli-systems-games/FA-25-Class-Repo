using UnityEngine;

[DisallowMultipleComponent]
public class BallTeamSkin : MonoBehaviour
{
    [Header("目标渲染器（不填会自动找自身或子物体）")]
    public Renderer targetRenderer;

    [Header("使用哪个材质槽位（materials 下标，从 0 开始）")]
    [Min(0)] public int materialSlot = 0;

    [Header("预设材质")]
    public Material dayMaterial;    // 日
    public Material nightMaterial;  // 月

   

    [Header("当前阵营（-1=未染，0=日，1=月）")]
    public int teamId = -1;

    void Awake()
    {
        if (!targetRenderer) targetRenderer = GetComponentInChildren<Renderer>();

    }

    /// 设置阵营 & 切换材质
    public void SetTeam(int team)
    {
        teamId = Mathf.Clamp(team, -1, 1);

        // 1) 切指定槽位材质
        if (targetRenderer && teamId >= 0)
        {
            var mats = targetRenderer.materials;  // 运行时实例（符合你“更改预设槽位”的需求）
            if (materialSlot >= 0 && materialSlot < mats.Length)
            {
                mats[materialSlot] = (teamId == 0 ? dayMaterial : nightMaterial);
                targetRenderer.materials = mats;  // 重新赋回去才能生效
            }
            else
            {
                Debug.LogWarning($"[BallTeamSkin] materialSlot={materialSlot} 超范围（共有 {mats.Length} 个槽）。");
            }
        }

    }
}