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

    // --- 关键修改：这里改成了 public，并且去掉了下划线 ---
    public float CurrentScore = 0f;

    private Stack<int> _stoneHistory = new Stack<int>();
    private Vector3 _initialScale;

    void Start()
    {
        if (WallMesh != null)
        {
            _initialScale = WallMesh.localScale;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CharacterHandleWeapon handleWeapon = other.GetComponent<CharacterHandleWeapon>();
        Character character = other.GetComponent<Character>();

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

                // 关键修改：使用 CurrentScore
                CurrentScore += scoreToAdd;

                UpdateWallHeight();
                handleWeapon.ChangeWeapon(null, "EmptyHands");
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Character character = other.GetComponent<Character>();
        if (character == null) return;

        if (character.PlayerID != OwnerID)
        {
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

        int lastStoneScore = _stoneHistory.Pop();

        // 关键修改：使用 CurrentScore
        CurrentScore -= lastStoneScore;

        UpdateWallHeight();

        CharacterHandleWeapon thiefWeaponHandle = thief.FindAbility<CharacterHandleWeapon>();
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

        // 关键修改：使用 CurrentScore
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