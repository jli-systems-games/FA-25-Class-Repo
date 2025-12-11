using UnityEngine;

/// <summary>
/// 石头的来源：石堆 or 墙
/// </summary>
public enum RockOrigin
{
    Pile,   // 石堆里拿出来的，可以扔
    Wall    // 墙上拆下来的，只能搬运，不能扔
}

/// <summary>
/// 挂在石头上的小标记
/// </summary>
public class RockSource : MonoBehaviour
{
    [Header("来源设置")]
    public RockOrigin Origin = RockOrigin.Pile;

    [Tooltip("这块石头最初属于哪个玩家的墙/石堆，比如 Player1, Player2 ...")]
    public string OwnerPlayerID;

    /// <summary>
    /// 只有石堆来的石头才允许投掷
    /// </summary>
    public bool CanBeThrown
    {
        get { return Origin == RockOrigin.Pile; }
    }
}
