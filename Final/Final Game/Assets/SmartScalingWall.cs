using UnityEngine;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;

public class SmartScalingWall : MonoBehaviour
{
    [Header("阵营设置")]
    public string OwnerID = "Player1";
    // 【新增】队友ID：如果是 P1 的墙，这里填 Player3
    public string PartnerID = "Player3";

    [Header("视觉设置")]
    public Transform WallMesh;
    public GameObject BreakEffect;

    [Header("Steal Controls")] // 这个能在Inspector里加个标题，好看点
    public KeyCode Player1StealKey = KeyCode.F;      // 默认 F 键
    public KeyCode Player2StealKey = KeyCode.Return; // 默认 回车键
    public KeyCode Player3StealKey = KeyCode.Joystick1Button0;
    public KeyCode Player4StealKey = KeyCode.Joystick1Button0;

    public enum Axis { X, Y, Z }
    [Header("调整生长方向")]
    public Axis GrowthAxis = Axis.Y;

    [Header("道具配置")]
    public Weapon SmallStoneWeapon;
    public Weapon BigStoneWeapon;

    [Header("数值规则")]
    public float HeightPerScore = 0.25f;

    // 公开变量，方便 UI 读取
    public float CurrentScore = 0f;

    // --- 偷窃冷却 (解决连点BUG) ---
    [Header("偷窃冷却")]
    public float StealCooldown = 1.0f;
    private float _lastStealTime = -100f;

    private Stack<int> _stoneHistory = new Stack<int>();
    private Vector3 _initialScale;

    void Start()
    {
        if (WallMesh != null)
        {
            _initialScale = WallMesh.localScale;
        }
    }

    // ------------------- 建造逻辑 -------------------
    private void OnTriggerEnter(Collider other)
    {
        CharacterHandleWeapon handleWeapon = other.GetComponentInParent<CharacterHandleWeapon>();
        Character character = other.GetComponentInParent<Character>();

        if (character == null || handleWeapon == null || handleWeapon.CurrentWeapon == null) return;

        // 【关键修改】只要是 Owner 或者 Partner，都可以建造
        if (character.PlayerID == OwnerID || character.PlayerID == PartnerID)
        {
            string weaponName = handleWeapon.CurrentWeapon.WeaponName;
            int scoreToAdd = 0;

            if (weaponName == "SmallStone") scoreToAdd = 1;
            else if (weaponName == "BigStone") scoreToAdd = 4;

            if (scoreToAdd > 0)
            {
                _stoneHistory.Push(scoreToAdd);
                CurrentScore += scoreToAdd;
                UpdateWallHeight();
                handleWeapon.ChangeWeapon(null, "EmptyHands");

                Debug.Log($"[建造] {character.PlayerID} 为 {OwnerID} 的墙增加砖块！");
            }
        }
    }

    // ------------------- 偷窃逻辑 -------------------
    private void OnTriggerStay(Collider other)
    {
        Character character = other.GetComponentInParent<Character>();
        if (character == null) return;

        string pID = character.PlayerID;

        // --- 1. 队伍判定逻辑 ---

        // 墙属于哪一队？(P1 & P3 是一队，P2 & P4 是一队)
        int wallTeam = 0;
        if (OwnerID == "Player1" || OwnerID == "Player3") wallTeam = 1;
        else if (OwnerID == "Player2" || OwnerID == "Player4") wallTeam = 2;

        // 玩家属于哪一队？
        int playerTeam = 0;
        if (pID == "Player1" || pID == "Player3") playerTeam = 1;
        else if (pID == "Player2" || pID == "Player4") playerTeam = 2;

        // 规则：如果是队友(team相同) 或者 无法识别队伍，则不能偷，直接退出
        if (wallTeam == 0 || playerTeam == 0 || wallTeam == playerTeam) return;

        // --- 2. 冷却与输入检测 ---

        // 冷却检查
        if (Time.time < _lastStealTime + StealCooldown) return;

        bool stealInput = false;

        // 检测各自的按键
        if (pID == "Player1" && Input.GetKeyDown(Player1StealKey)) stealInput = true;
        else if (pID == "Player2" && Input.GetKeyDown(Player2StealKey)) stealInput = true;
        else if (pID == "Player3" && Input.GetKeyDown(Player3StealKey)) stealInput = true;
        else if (pID == "Player4" && Input.GetKeyDown(Player4StealKey)) stealInput = true;

        // --- 3. 执行偷窃 ---
        if (stealInput)
        {
            PerformSteal(character);
        }
    }

    void PerformSteal(Character thief)
    {
        if (_stoneHistory.Count == 0) return;

        _lastStealTime = Time.time;

        int lastStoneScore = _stoneHistory.Pop();
        CurrentScore -= lastStoneScore;
        UpdateWallHeight();

        CharacterHandleWeapon thiefWeaponHandle = thief.GetComponentInParent<CharacterHandleWeapon>();
        if (thiefWeaponHandle != null)
        {
            if (lastStoneScore == 1) thiefWeaponHandle.ChangeWeapon(SmallStoneWeapon, "StolenSmallStone");
            else if (lastStoneScore == 4) thiefWeaponHandle.ChangeWeapon(BigStoneWeapon, "StolenBigStone");
        }

        if (BreakEffect != null)
        {
            Vector3 topPos = WallMesh.position;
            // 简单的位置计算，根据你的实际轴向可能需要微调
            if (GrowthAxis == Axis.Y) topPos += Vector3.up * WallMesh.localScale.y;
            else if (GrowthAxis == Axis.Z) topPos += Vector3.forward * WallMesh.localScale.z;
            else topPos += Vector3.right * WallMesh.localScale.x;

            GameObject fx = Instantiate(BreakEffect, topPos, Quaternion.identity);
            Destroy(fx, 2f);
        }

        Debug.Log($"[偷窃] {thief.PlayerID} 偷了 {OwnerID} 的墙！");
    }

    void UpdateWallHeight()
    {
        if (WallMesh == null) return;

        float addedValue = CurrentScore * HeightPerScore;
        Vector3 newScale = _initialScale;

        switch (GrowthAxis)
        {
            case Axis.X: newScale.x = _initialScale.x + addedValue; break;
            case Axis.Y: newScale.y = _initialScale.y + addedValue; break;
            case Axis.Z: newScale.z = _initialScale.z + addedValue; break;
        }

        WallMesh.localScale = newScale;
    }

    // --- 保留你原来的其他方法，防止报错 ---
    public void ForceUpdateHeight() { UpdateWallHeight(); }
    public void ShrinkWall(float amount) { /* ...保持你原来的逻辑... */ }
    public void UpdateWallSize(float newSize) { /* ...保持你原来的逻辑... */ }
}