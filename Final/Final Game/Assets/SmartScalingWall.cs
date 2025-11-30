using UnityEngine;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;

public class SmartScalingWall : MonoBehaviour
{
    [Header("阵营设置")]
    public string OwnerID = "Player1";

    [Header("视觉设置")]
    public Transform WallMesh;
    public GameObject BreakEffect;

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
    public float StealCooldown = 1.0f; // 1秒只能偷一次
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
        // 【关键修复】使用 GetComponentInParent，防止只碰到手或武器没反应
        CharacterHandleWeapon handleWeapon = other.GetComponentInParent<CharacterHandleWeapon>();
        Character character = other.GetComponentInParent<Character>();

        if (character == null || handleWeapon == null || handleWeapon.CurrentWeapon == null) return;

        if (character.PlayerID == OwnerID)
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
            }
        }
    }

    // ------------------- 偷窃逻辑 (带冷却 + 修复判定) -------------------
    private void OnTriggerStay(Collider other)
    {
        // 【关键修复】使用 GetComponentInParent，让判定360度无死角
        Character character = other.GetComponentInParent<Character>();
        if (character == null) return;

        if (character.PlayerID != OwnerID)
        {
            // 冷却检查：如果还没冷却好，直接无视按键
            if (Time.time < _lastStealTime + StealCooldown) return;

            bool stealInput = false;
            if (character.PlayerID == "Player1" && Input.GetKeyDown(KeyCode.F)) stealInput = true;
            else if (character.PlayerID == "Player2" && Input.GetKeyDown(KeyCode.Return)) stealInput = true;

            if (stealInput)
            {
                PerformSteal(character);
            }
        }
    }

    void PerformSteal(Character thief)
    {
        if (_stoneHistory.Count == 0) return;

        // 记录偷窃时间，开始冷却
        _lastStealTime = Time.time;

        int lastStoneScore = _stoneHistory.Pop();
        CurrentScore -= lastStoneScore;
        UpdateWallHeight();

        // 同样使用 GetComponentInParent 确保能找到武器组件
        CharacterHandleWeapon thiefWeaponHandle = thief.GetComponentInParent<CharacterHandleWeapon>();
        if (thiefWeaponHandle != null)
        {
            if (lastStoneScore == 1) thiefWeaponHandle.ChangeWeapon(SmallStoneWeapon, "StolenSmallStone");
            else if (lastStoneScore == 4) thiefWeaponHandle.ChangeWeapon(BigStoneWeapon, "StolenBigStone");
        }

        if (BreakEffect != null)
        {
            Vector3 topPos = WallMesh.position;
            if (GrowthAxis == Axis.Y) topPos += Vector3.up * WallMesh.localScale.y;
            else if (GrowthAxis == Axis.Z) topPos += Vector3.forward * WallMesh.localScale.z;
            else topPos += Vector3.right * WallMesh.localScale.x;

            GameObject fx = Instantiate(BreakEffect, topPos, Quaternion.identity);
            Destroy(fx, 2f);
        }
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
}